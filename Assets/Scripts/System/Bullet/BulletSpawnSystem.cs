namespace DotsFisher.Conponent
{
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct BulletSpawnSystem : ISystem
    {
        private Random _rnd;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletSpawnRequestComponent>();
            _rnd = Random.CreateFromIndex(0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var newSpawnQuery = SystemAPI
                .QueryBuilder()
                .WithAll<BulletSpawnRequestComponent>()
                .Build();

            var count = newSpawnQuery.CalculateEntityCount();

            for (int i = 0; i < count; i++)
            {
                var rotation = math.radians(_rnd.NextFloat(0, 360f));
                var direction = CoordinateUtils.RadiansToDirection(rotation);

                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(entity, new MovementComponent
                {
                    Speed = 0,
                    Direction = direction,
                });
                state.EntityManager.AddComponentData(entity, new TransformComponent
                {
                    Position = _rnd.NextFloat2(new float2(-10, -10), new float2(10, 10)),
                    Rotation = rotation,
                });
                state.EntityManager.AddComponentData(entity, new CircleColliderComponent
                {
                    Radius = 1f,
                });
                state.EntityManager.AddComponentData(entity, new BulletComponent());
            }

            state.EntityManager.RemoveComponent<BulletSpawnRequestComponent>(newSpawnQuery);
        }
    }
}
