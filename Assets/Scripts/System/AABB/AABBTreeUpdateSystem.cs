namespace DotsFisher.Component
{
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Entities;

    [UpdateInGroup(typeof(AABBTreeSystemGroup))]
    public partial struct AABBTreeUpdateSystem : ISystem
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

            foreach (var (transform, collider, aabbNode)
                in SystemAPI
                    .Query<RefRO<TransformComponent>, RefRO<CircleColliderComponent>, RefRO<AABBNodeComponent>>())
            {
                var aabb = new AABB
                {
                    Min = transform.ValueRO.Position - collider.ValueRO.Radius,
                    Max = transform.ValueRO.Position + collider.ValueRO.Radius,
                };
                aabbTree.Tree.Update(aabbNode.ValueRO.EntryId, aabb);
            }
        }
    }
}