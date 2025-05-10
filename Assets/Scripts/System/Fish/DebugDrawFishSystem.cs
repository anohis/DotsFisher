namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Mono;
    using DotsFisher.Utils;
    using System.Linq;
    using Unity.Entities;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class DebugDrawFishSystem : SystemBase
    {
        private FishDebugDrawer _drawer;

        public void Initialize(FishDebugDrawer drawer)
        {
            _drawer = drawer;
        }

        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<TransformComponent>();
            CheckedStateRef.RequireForUpdate<FishComponent>();
            CheckedStateRef.RequireForUpdate<CircleColliderComponent>();
        }

        protected override void OnUpdate()
        {
            var query = SystemAPI
                .QueryBuilder()
                .WithAll<FishComponent, TransformComponent, CircleColliderComponent>()
                .Build();

            var transforms = query.ToComponentDataArray<TransformComponent>(CheckedStateRef.WorldUpdateAllocator);
            var colliders = query.ToComponentDataArray<CircleColliderComponent>(CheckedStateRef.WorldUpdateAllocator);

            _drawer.Draw(transforms.Select((transform, index) =>
            {
                return new FishDebugDrawer.Fish
                {
                    Position = transform.Position.ToPosition3D(),
                    Radius = colliders[index].Radius,
                };
            }));

            CheckedStateRef.Dependency = transforms.Dispose(CheckedStateRef.Dependency);
            CheckedStateRef.Dependency = colliders.Dispose(CheckedStateRef.Dependency);
        }
    }
}