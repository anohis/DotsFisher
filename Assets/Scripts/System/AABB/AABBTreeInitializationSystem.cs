namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct AABBTreeInitializationSystem : ISystem
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
            state.EntityManager.AddComponentData(entity, new AABBTreeComponent
            {
                Tree = new AABBTree(1, Allocator.Persistent),
            });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            SystemAPI.GetSingleton<AABBTreeComponent>().Tree.Dispose();
        }
    }
}