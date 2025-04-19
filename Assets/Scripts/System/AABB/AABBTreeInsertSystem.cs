namespace DotsFisher.Component
{
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;

    [UpdateInGroup(typeof(AABBTreeSystemGroup))]
    [UpdateAfter(typeof(AABBTreeUpdateSystem))]
    public partial struct AABBTreeInsertSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AABBTreeComponent>();
            state.RequireForUpdate<TransformComponent>();
            state.RequireForUpdate<CircleColliderComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var aabbTree = ref SystemAPI.GetSingletonRW<AABBTreeComponent>().ValueRW;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, collider, entity)
                in SystemAPI
                    .Query<RefRO<TransformComponent>, RefRO<CircleColliderComponent>>()
                    .WithNone<AABBNodeComponent>()
                    .WithEntityAccess())
            {
                var aabb = new AABB
                {
                    Min = transform.ValueRO.Position - collider.ValueRO.Radius,
                    Max = transform.ValueRO.Position + collider.ValueRO.Radius,
                };

                var entryId = aabbTree.Tree.Insert(aabb);

                ecb.AddComponent(entity, new AABBNodeComponent
                {
                    EntryId = entryId,
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}