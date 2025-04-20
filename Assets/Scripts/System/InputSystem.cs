namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var direction = float2.zero;

            if (Input.GetKey(KeyCode.W))
            {
                direction += new float2(0, 1);
            }

            if (Input.GetKey(KeyCode.A))
            {
                direction += new float2(-1, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                direction += new float2(0, -1);
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction += new float2(1, 0);
            }

            if (math.all(direction == float2.zero))
            {
                return;
            }

            var bulletEntity = SystemAPI.GetSingletonEntity<BulletComponent>();
            var transformLookup = SystemAPI.GetComponentLookup<TransformComponent>();
            ref var transform = ref transformLookup.GetRefRW(bulletEntity).ValueRW;

            transform.Position = transform.Position + math.normalize(direction) * 5 * state.World.Time.DeltaTime;
        }
    }
}