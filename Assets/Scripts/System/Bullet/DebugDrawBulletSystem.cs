namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
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
            state.RequireForUpdate<BulletComponent>();
            state.RequireForUpdate<CircleColliderComponent>();
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
        private void Execute(
            in TransformComponent transform,
            in CircleColliderComponent collider)
        {
            DebugUtils.DrawWireCircle(
                transform.Position.ToPosition3D(),
                collider.Radius,
                Color.red,
                segments: 8);
        }
    }
}