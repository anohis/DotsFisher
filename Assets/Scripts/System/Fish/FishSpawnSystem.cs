namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct FishSpawnSystem : ISystem
    {
        private Random _rnd;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FishSpawnRequestComponent>();
            _rnd = Random.CreateFromIndex(0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in SystemAPI
                .Query<RefRO<FishSpawnRequestComponent>>()
                .WithEntityAccess())
            {
                var rotation = math.radians(_rnd.NextFloat(0, 360f));
                var direction = CoordinateUtils.RadiansToDirection(rotation);

                ecb.AddComponent(entity, new MovementComponent
                {
                    Speed = 1,
                    Direction = direction,
                });
                ecb.AddComponent(entity, new TransformComponent
                {
                    Position = _rnd.NextFloat2(new float2(-10, -10), new float2(10, 10)),
                    Rotation = rotation,
                });
                ecb.AddComponent(entity, new CircleColliderComponent
                {
                    Radius = 1f,
                });
                ecb.AddComponent(entity, new FishComponent());
                ecb.AddComponent(entity, new AABBInsertRequestComponent());
                ecb.RemoveComponent<FishSpawnRequestComponent>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
