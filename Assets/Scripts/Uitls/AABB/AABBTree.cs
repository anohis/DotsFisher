namespace DotsFisher.Utils
{
    using DotsFisher.Utils.Native;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs.LowLevel.Unsafe;
    using Unity.Mathematics;

    public unsafe struct AABBTree : IDisposable
    {
        private const int RootIndex = 0;
        private const uint InvalidEntryId = 0;

        public struct TreeNode
        {
            public bool IsValid;
            public uint EntryId;
            public AABB AABB;

            public bool IsLeaf => EntryId != InvalidEntryId;
        }

        public unsafe struct Enumerator : IEnumerator<TreeNode>
        {
            private readonly AABBTree* _tree;
            private readonly NativeStack<int> _stack;

            public Enumerator(AABBTree* tree, int index, Allocator allocator)
            {
                Current = default;
                _tree = tree;
                _stack = new NativeStack<int>(allocator);
                if (_tree->HasNode(index))
                {
                    _stack.Push(index);
                }
            }

            public TreeNode Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _stack.Dispose();
            }

            public bool MoveNext()
            {
                if (_stack.Length <= 0)
                {
                    return false;
                }

                var index = _stack.Pop();
                var node = _tree->GetNode(index);

                Current = *node;

                if (node->IsLeaf)
                {
                    return true;
                }

                var leftIndex = GetLeftIndex(index);
                var rightIndex = GetRightIndex(index);

                if (_tree->HasNode(leftIndex))
                {
                    _stack.Push(leftIndex);
                }
                if (_tree->HasNode(rightIndex))
                {
                    _stack.Push(rightIndex);
                }

                return true;
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }
        }

        private readonly Allocator _allocator;

        private TreeNode* _nodes;
        private int _capacity;
        private uint _serialNumber;
        private NativeHashMap<uint, int> _nodeMap;

        public AABBTree(int capacity, Allocator allocator)
        {
            _allocator = allocator;
            _serialNumber = 0;
            _capacity = MathUtils.NextPowerOfTwo(capacity);
            _nodeMap = new NativeHashMap<uint, int>(_capacity, _allocator);
            _nodes = (TreeNode*)UnsafeUtility.Malloc(sizeof(TreeNode) * _capacity, JobsUtility.CacheLineSize, allocator);
            for (int i = 0; i < _capacity; i++)
            {
                *GetNode(i) = new TreeNode
                {
                    IsValid = false,
                };
            }
        }

        public uint Insert(AABB aabb)
        {
            var entryId = ++_serialNumber;
            Insert(entryId, aabb);
            return entryId;
        }

        public void Delete(uint entryId)
        {
            if (!_nodeMap.TryGetValue(entryId, out var nodeIndex))
            {
                return;
            }

            _nodeMap.Remove(entryId);
            RemoveNode(nodeIndex);

            if (nodeIndex == RootIndex)
            {
                return;
            }

            var parentIndex = GetParentIndex(nodeIndex);
            var leftIndex = GetLeftIndex(parentIndex);
            var otherIndex = nodeIndex == leftIndex
                ? GetRightIndex(parentIndex)
                : leftIndex;
            MoveNodeUpBFS(otherIndex, parentIndex);

            if (parentIndex == RootIndex)
            {
                return;
            }

            UpdateAABBUp(GetParentIndex(parentIndex));
        }

        public void Update(uint entryId, AABB aabb)
        {
            Delete(entryId);
            Insert(entryId, aabb);
        }

        public Enumerator GetIterator(Allocator allocator)
        {
            fixed (AABBTree* tree = &this)
            {
                return new Enumerator(tree, RootIndex, allocator);
            }
        }

        public void Dispose()
        {
            UnsafeUtility.Free(_nodes, _allocator);
            _nodes = null;
            _nodeMap.Dispose();
        }

        private void CheckCapacity(int index)
        {
            var requiredCapacity = index + 1;
            if (requiredCapacity <= _capacity)
            {
                return;
            }

            var newCapacity = MathUtils.NextPowerOfTwo(requiredCapacity);
            var newNodes = (TreeNode*)UnsafeUtility.Malloc(sizeof(TreeNode) * newCapacity, JobsUtility.CacheLineSize, _allocator);

            for (int i = 0; i < _capacity; i++)
            {
                var node = GetNode(_nodes, i);
                *GetNode(newNodes, i) = new TreeNode
                {
                    IsValid = node->IsValid,
                    EntryId = node->EntryId,
                    AABB = node->AABB,
                };
            }

            for (int i = _capacity; i < newCapacity; i++)
            {
                *GetNode(newNodes, i) = new TreeNode
                {
                    IsValid = false,
                };
            }

            UnsafeUtility.Free(_nodes, _allocator);
            _nodes = newNodes;
            _capacity = newCapacity;

            var newNodeMap = new NativeHashMap<uint, int>(newCapacity, _allocator);

            foreach (var v in _nodeMap)
            {
                newNodeMap.Add(v.Key, v.Value);
            }

            _nodeMap.Dispose();
            _nodeMap = newNodeMap;
        }

        private void Insert(uint entryId, AABB aabb)
        {
            if (!HasNode(RootIndex))
            {
                CheckCapacity(RootIndex);
                CreateLeafNode(RootIndex, entryId, aabb);
                return;
            }

            var selectedIndex = SelectInsertParent(aabb);

            var leftIndex = GetLeftIndex(selectedIndex);
            var rightIndex = GetRightIndex(selectedIndex);

            CheckCapacity(rightIndex);

            var selectedNode = GetNode(selectedIndex);
            CreateLeafNode(leftIndex, selectedNode->EntryId, selectedNode->AABB);
            selectedNode->EntryId = InvalidEntryId;

            CreateLeafNode(rightIndex, entryId, aabb);

            UpdateAABBUp(selectedIndex);
        }

        private void UpdateAABBUp(int index)
        {
            while (index > RootIndex)
            {
                UpdateAABB(index);
                index = GetParentIndex(index);
            }
            UpdateAABB(index);
        }

        private void UpdateAABB(int index)
        {
            var left = GetNode(GetLeftIndex(index));
            var right = GetNode(GetRightIndex(index));
            GetNode(index)->AABB = left->AABB + right->AABB;
        }

        private void MoveNodeUpBFS(int fromIndex, int toIndex)
        {
            var queue = new NativeQueue<(int, int)>(Allocator.Temp);
            queue.Enqueue((fromIndex, toIndex));
            while (queue.Count > 0)
            {
                (fromIndex, toIndex) = queue.Dequeue();

                var isLeaf = GetNode(fromIndex)->IsLeaf;
                MoveNodeUp(fromIndex, toIndex);
                if (!isLeaf)
                {
                    queue.Enqueue((GetLeftIndex(fromIndex), GetLeftIndex(toIndex)));
                    queue.Enqueue((GetRightIndex(fromIndex), GetRightIndex(toIndex)));
                }
            }
        }

        private void MoveNodeUp(int fromIndex, int toIndex)
        {
            var from = GetNode(fromIndex);
            var to = GetNode(toIndex);

            to->IsValid = true;
            to->EntryId = from->EntryId;
            to->AABB = from->AABB;

            if (from->IsLeaf)
            {
                _nodeMap[from->EntryId] = toIndex;
            }

            RemoveNode(fromIndex);
        }

        private void CreateLeafNode(int index, uint entryId, AABB aabb)
        {
            var nodePtr = GetNode(index);
            *nodePtr = new TreeNode
            {
                IsValid = true,
                EntryId = entryId,
                AABB = aabb,
            };
            _nodeMap[entryId] = index;
        }

        private void RemoveNode(int index)
        {
            var nodePtr = GetNode(index);
            *nodePtr = new TreeNode
            {
                IsValid = false,
            };
        }

        private int SelectInsertParent(AABB aabb)
        {
            var selectedIndex = 0;

            while (!GetNode(selectedIndex)->IsLeaf)
            {
                selectedIndex = SelectChild(selectedIndex, aabb);
            }

            return selectedIndex;
        }

        private int SelectChild(int parentIndex, AABB aabb)
        {
            var leftIndex = GetLeftIndex(parentIndex);
            var rightIndex = GetRightIndex(parentIndex);

            var left = GetNode(leftIndex);
            var right = GetNode(rightIndex);

            var costLeft = (left->AABB + aabb).Area + right->AABB.Area;
            var costRight = left->AABB.Area + (right->AABB + aabb).Area;

            if (costLeft < costRight)
            {
                return leftIndex;
            }
            else if (costLeft > costRight)
            {
                return rightIndex;
            }

            var toLeftVector = math.abs(left->AABB.Min + left->AABB.Max - aabb.Min - aabb.Max);
            var toRightVector = math.abs(right->AABB.Min + right->AABB.Max - aabb.Min - aabb.Max);
            costLeft = toLeftVector.x + toLeftVector.y;
            costRight = toRightVector.x + toRightVector.y;

            return costLeft < costRight ? leftIndex : rightIndex;
        }

        private bool HasNode(int index)
        {
            return TryGetNode(index, out _);
        }

        private bool TryGetNode(int index, out TreeNode* node)
        {
            if (index >= _capacity)
            {
                node = null;
                return false;
            }

            node = GetNode(index);
            return node->IsValid;
        }

        private TreeNode* GetNode(int index)
        {
            return GetNode(_nodes, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TreeNode* GetNode(TreeNode* nodes, int index)
        {
            return nodes + index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLeftIndex(int index)
        {
            return (index << 1) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetRightIndex(int index)
        {
            return (index << 1) + 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetParentIndex(int index)
        {
            return (index - 1) >> 1;
        }
    }
}