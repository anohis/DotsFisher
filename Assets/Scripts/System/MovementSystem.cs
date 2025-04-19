namespace DotsFisher.Component
{
    using Unity.Burst;
    using Unity.Entities;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct MovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TransformComponent>();
            state.RequireForUpdate<MovementComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref TransformComponent transform, in MovementComponent movement)
        {
            transform.Position = transform.Position + movement.Direction * movement.Speed * DeltaTime;
        }
    }
}