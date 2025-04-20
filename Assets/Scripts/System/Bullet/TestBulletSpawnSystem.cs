namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TestBulletSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new TransformComponent
            {
                Position = float2.zero,
                Rotation = 0,
            });
            state.EntityManager.AddComponentData(entity, new CircleColliderComponent
            {
                Radius = 1f,
            });
            state.EntityManager.AddComponentData(entity, new BulletComponent());
            state.EntityManager.AddComponentData(entity, new AABBQueryRequestComponent());
        }
    }
}