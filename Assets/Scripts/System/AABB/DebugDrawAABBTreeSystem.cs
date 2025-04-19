namespace DotsFisher.Component
{
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using UnityEngine;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct DebugDrawAABBTreeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AABBTreeComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var aabbTree = SystemAPI.GetSingleton<AABBTreeComponent>();
            var list = new NativeList<AABB>(state.WorldUpdateAllocator);

            unsafe
            {
                var aabbEnumerator = aabbTree.Tree.GetIterator(Allocator.Temp);
                while (aabbEnumerator.MoveNext())
                {
                    list.Add(aabbEnumerator.Current.AABB);
                }
                aabbEnumerator.Dispose();
            }

            state.Dependency = new DrawAABBJob
            {
                AABBs = list,
            }.Schedule(list.Length, 32, state.Dependency);

            state.Dependency = list.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public struct DrawAABBJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<AABB> AABBs;

        public void Execute(int index)
        {
            var aabb = AABBs[index];
            DebugUtils.DrawWireRect(
               aabb.Min,
               aabb.Max,
               Color.green);
        }
    }
}