namespace DotsFisher.Conponent
{
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct DebugDrawBulletSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TransformComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new DrawBulletJob().ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct DrawBulletJob : IJobEntity
    {
        private void Execute(in TransformComponent transform)
        {
            DebugUtils.DrawWireCircle(
                transform.Position.ToPosition3D(),
                1,
                Color.red);
        }
    }
}