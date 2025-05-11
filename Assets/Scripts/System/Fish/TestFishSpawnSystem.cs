namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Entities;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct TestFishSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            for (int i = 0; i < 10000; i++)
            {
                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(entity, new FishSpawnRequestComponent());
            }
        }
    }
}
