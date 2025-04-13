namespace DotsFisher.Conponent
{
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct BulletSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletSpawnRequestComponent>();
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
                var random = Random.CreateFromIndex((uint)i);

                var rotation = math.radians(random.NextFloat(0, 360f));
                var direction = CoordinateUtils.RadiansToDirection(rotation);

                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(entity, new MovementComponent
                {
                    Speed = 1,
                    Direction = direction,
                });
                state.EntityManager.AddComponentData(entity, new TransformComponent
                {
                    Position = float2.zero,
                    Rotation = rotation,
                });
            }

            state.EntityManager.RemoveComponent<BulletSpawnRequestComponent>(newSpawnQuery);
        }
    }
}
