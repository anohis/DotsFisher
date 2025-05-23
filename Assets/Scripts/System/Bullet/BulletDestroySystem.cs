namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;

    [UpdateInGroup(typeof(GameSystemGroup))]
    [UpdateAfter(typeof(AABBTreeSystemGroup))]
    public partial struct BulletDestroySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
            state.RequireForUpdate<DestroyRequestComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = SystemAPI
                .QueryBuilder()
                .WithAll<BulletComponent, DestroyRequestComponent>()
                .Build()
                .ToEntityArray(Allocator.Temp);
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.DestroyEntity(entities);
            ecb.Playback(state.EntityManager);
        }
    }
}