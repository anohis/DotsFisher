namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Utils;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
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
            var query = SystemAPI
                .QueryBuilder()
                .WithAll<BulletComponent, TransformComponent, CircleColliderComponent>()
                .Build();

            var transforms = query.ToComponentDataArray<TransformComponent>(state.WorldUpdateAllocator);
            var colliders = query.ToComponentDataArray<CircleColliderComponent>(state.WorldUpdateAllocator);

            state.Dependency = new DrawBulletJob
            {
                Transforms = transforms,
                Colliders = colliders,
            }.Schedule(transforms.Length, 32, state.Dependency);

            state.Dependency = transforms.Dispose(state.Dependency);
            state.Dependency = colliders.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public struct DrawBulletJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<TransformComponent> Transforms;
        [ReadOnly] public NativeArray<CircleColliderComponent> Colliders;

        public void Execute(int index)
        {
            DebugUtils.DrawWireCircle(
                Transforms[index].Position.ToPosition3D(),
                Colliders[index].Radius,
                Color.red,
                segments: 8);
        }
    }
}