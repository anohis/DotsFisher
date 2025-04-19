namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;

    [UpdateInGroup(typeof(AABBTreeSystemGroup))]
    public partial struct AABBTreeDeleteSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AABBTreeComponent>();
            state.RequireForUpdate<AABBNodeComponent>();
            state.RequireForUpdate<DestroyRequestComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var aabbTree = ref SystemAPI.GetSingletonRW<AABBTreeComponent>().ValueRW;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (aabbNode, entity)
                in SystemAPI
                    .Query<RefRO<AABBNodeComponent>>()
                    .WithAll<DestroyRequestComponent>()
                    .WithEntityAccess())
            {
                aabbTree.Tree.Delete(aabbNode.ValueRO.EntryId);

                ecb.RemoveComponent<AABBNodeComponent>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}