namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
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
            var list = new NativeList<AABB>(Allocator.Temp);

            aabbTree.Tree.Query(ref list);

            foreach (var v in list)
            {
                DebugUtils.DrawWireRect(
                   v.Min,
                   v.Max,
                   Color.green);
            }

            list.Dispose();
        }
    }
}