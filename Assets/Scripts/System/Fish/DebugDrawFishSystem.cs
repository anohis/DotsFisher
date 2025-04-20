namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct DebugDrawFishSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TransformComponent>();
            state.RequireForUpdate<FishComponent>();
            state.RequireForUpdate<CircleColliderComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new DrawFishJob().ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct DrawFishJob : IJobEntity
    {
        private void Execute(
            in TransformComponent transform,
            in CircleColliderComponent collider)
        {
            DebugUtils.DrawWireCircle(
                transform.Position.ToPosition3D(),
                collider.Radius,
                Color.yellow,
                segments: 8);
        }
    }
}