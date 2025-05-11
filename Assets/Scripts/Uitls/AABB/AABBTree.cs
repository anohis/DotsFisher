namespace DotsFisher.Utils
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Mathematics;

    public unsafe struct AABBTree : IDisposable
    {
        private const uint InvalidEntryId = 0;

        private struct TreeNode
        {
            public uint EntryId;
            public AABB AABB;
            public TreeNode* Parent;
            public TreeNode* Left;
            public TreeNode* Right;

            public bool IsLeaf => Left == null && Right == null;
        }

        private readonly Allocator _allocator;

        private TreeNode* _root;
        private uint _serialNumber;
        private NativeHashMap<uint, IntPtr> _nodeMap;

        public AABBTree(int capacity, Allocator allocator)
        {
            _root = null;
            _allocator = allocator;
            _serialNumber = 0;
            _nodeMap = new NativeHashMap<uint, IntPtr>(capacity, _allocator);
        }

        public uint Insert(AABB aabb)
        {
            var entryId = ++_serialNumber;
            Insert(entryId, aabb);
            return entryId;
        }

        public void Delete(uint entryId)
        {
            if (!_nodeMap.TryGetValue(entryId, out var nodePtr))
            {
                return;
            }

            _nodeMap.Remove(entryId);
            Delete((TreeNode*)nodePtr);
        }

        public void Update(uint entryId, AABB aabb)
        {
            if (_nodeMap.TryGetValue(entryId, out var nodePtr)
                && AABB.IsContains(((TreeNode*)nodePtr)->AABB, aabb))
            {
                return;
            }

            Insert(entryId, aabb);

            var newNode = (TreeNode*)_nodeMap[entryId];
            var neighbor = GetNeighbor(newNode);
            var nodeToDelete = neighbor->EntryId == entryId
                ? neighbor
                : (TreeNode*)nodePtr;
            Delete(nodeToDelete);
        }

        public void Query(AABB aabb, ref NativeList<uint> result)
        {
            if (_root == null)
            {
                return;
            }

            using var queue = new NativeQueue<IntPtr>(Allocator.Temp);
            queue.Enqueue((IntPtr)_root);

            while (queue.Count > 0)
            {
                var node = (TreeNode*)queue.Dequeue();
                if (!AABB.IsOverlap(node->AABB, aabb))
                {
                    continue;
                }

                if (node->IsLeaf)
                {
                    result.Add(node->EntryId);
                }
                else
                {
                    queue.Enqueue((IntPtr)node->Left);
                    queue.Enqueue((IntPtr)node->Right);
                }
            }
        }

        public void Query(ref NativeList<AABB> result)
        {
            if (_root == null)
            {
                return;
            }

            using var queue = new NativeQueue<IntPtr>(Allocator.Temp);
            queue.Enqueue((IntPtr)_root);

            while (queue.Count > 0)
            {
                var node = (TreeNode*)queue.Dequeue();

                result.Add(node->AABB);

                if (!node->IsLeaf)
                {
                    queue.Enqueue((IntPtr)node->Left);
                    queue.Enqueue((IntPtr)node->Right);
                }
            }
        }

        public void Query(AABB aabb, ref NativeList<AABB> result)
        {
            if (_root == null)
            {
                return;
            }

            using var queue = new NativeQueue<IntPtr>(Allocator.Temp);
            queue.Enqueue((IntPtr)_root);

            while (queue.Count > 0)
            {
                var node = (TreeNode*)queue.Dequeue();

                if (!AABB.IsOverlap(node->AABB, aabb))
                {
                    continue;
                }

                result.Add(node->AABB);

                if (!node->IsLeaf)
                {
                    queue.Enqueue((IntPtr)node->Left);
                    queue.Enqueue((IntPtr)node->Right);
                }
            }
        }

        public void Dispose()
        {
            using var queue = new NativeQueue<IntPtr>(Allocator.Temp);
            queue.Enqueue((IntPtr)_root);
            while (queue.Count > 0)
            {
                var node = (TreeNode*)queue.Dequeue();

                if (!node->IsLeaf)
                {
                    queue.Enqueue((IntPtr)node->Left);
                    queue.Enqueue((IntPtr)node->Right);
                }

                ReleaseNode(node);
            }
            _root = null;
            _nodeMap.Dispose();
        }

        private void Insert(uint entryId, AABB aabb)
        {
            aabb = AABB.Expand(aabb, 1);

            if (_root == null)
            {
                _root = CreateLeafNode(entryId, aabb);
                return;
            }

            var selectedNode = SelectInsertParent(_root, aabb);

            var left = CreateLeafNode(selectedNode->EntryId, selectedNode->AABB);
            left->Parent = selectedNode;
            selectedNode->Left = left;

            var right = CreateLeafNode(entryId, aabb);
            right->Parent = selectedNode;
            selectedNode->Right = right;

            selectedNode->EntryId = InvalidEntryId;

            UpdateAABBUp(selectedNode);

            static TreeNode* SelectInsertParent(TreeNode* root, AABB aabb)
            {
                var selectedNode = root;

                while (!selectedNode->IsLeaf)
                {
                    selectedNode = SelectChild(selectedNode, aabb);
                }

                return selectedNode;
            }

            static TreeNode* SelectChild(TreeNode* parent, AABB aabb)
            {
                var left = parent->Left;
                var right = parent->Right;

                var costLeft = (left->AABB + aabb).Area + right->AABB.Area;
                var costRight = left->AABB.Area + (right->AABB + aabb).Area;

                if (costLeft < costRight)
                {
                    return left;
                }
                else if (costLeft > costRight)
                {
                    return right;
                }

                var toLeftVector = math.abs(left->AABB.Min + left->AABB.Max - aabb.Min - aabb.Max);
                var toRightVector = math.abs(right->AABB.Min + right->AABB.Max - aabb.Min - aabb.Max);
                costLeft = toLeftVector.x + toLeftVector.y;
                costRight = toRightVector.x + toRightVector.y;

                return costLeft < costRight ? left : right;
            }
        }

        private void Delete(TreeNode* node)
        {
            if (node != _root)
            {
                var parent = node->Parent;
                var neighbor = GetNeighbor(node);
                if (parent == _root)
                {
                    _root = neighbor;
                    neighbor->Parent = null;
                }
                else
                {
                    if (parent->Parent->Left == parent)
                    {
                        parent->Parent->Left = neighbor;
                    }
                    else
                    {
                        parent->Parent->Right = neighbor;
                    }
                    neighbor->Parent = parent->Parent;
                    UpdateAABBUp(neighbor->Parent);
                }
                ReleaseNode(parent);
            }
            else
            {
                _root = null;
            }
            ReleaseNode(node);
        }

        private void UpdateAABBUp(TreeNode* start)
        {
            while (start != _root)
            {
                start->AABB = start->Left->AABB + start->Right->AABB;
                start = start->Parent;
            }
            start->AABB = start->Left->AABB + start->Right->AABB;
        }

        private TreeNode* CreateLeafNode(uint entryId, AABB aabb)
        {
            var node = (TreeNode*)UnsafeUtility.Malloc(sizeof(TreeNode), 0, _allocator);
            node->EntryId = entryId;
            node->AABB = aabb;
            node->Parent = null;
            node->Left = null;
            node->Right = null;
            _nodeMap[entryId] = (IntPtr)node;
            return node;
        }

        private void ReleaseNode(TreeNode* node)
        {
            UnsafeUtility.Free(node, _allocator);
        }

        private static TreeNode* GetNeighbor(TreeNode* node)
        {
            return node->Parent->Left == node
                ? node->Parent->Right
                : node->Parent->Left;
        }
    }
}