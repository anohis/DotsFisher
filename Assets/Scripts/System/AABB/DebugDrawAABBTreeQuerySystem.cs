namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(DebugDrawAABBTreeSystem))]
    public partial struct DebugDrawAABBTreeQuerySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AABBTreeComponent>();
            state.RequireForUpdate<AABBQueryRequestComponent>();
            state.RequireForUpdate<TransformComponent>();
            state.RequireForUpdate<CircleColliderComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var aabbTree = SystemAPI.GetSingleton<AABBTreeComponent>();
            var transformLookup = SystemAPI.GetComponentLookup<TransformComponent>(true);
            var colliderLookup = SystemAPI.GetComponentLookup<CircleColliderComponent>(true);
            var entity = SystemAPI.GetSingletonEntity<AABBQueryRequestComponent>();
            var transform = transformLookup[entity];
            var collider = colliderLookup[entity];
            var aabb = new AABB
            {
                Min = transform.Position - collider.Radius,
                Max = transform.Position + collider.Radius,
            };

            var list = new NativeList<AABB>(Allocator.Temp);

            aabbTree.Tree.Query(aabb, ref list);

            foreach (var v in list)
            {
                DebugUtils.DrawWireRect(
                   v.Min,
                   v.Max,
                   Color.red);
            }

            list.Dispose();
        }
    }
}