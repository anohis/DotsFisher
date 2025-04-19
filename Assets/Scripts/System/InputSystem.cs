namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct InputSystem : ISystem
    {
        private double _time;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (state.World.Time.ElapsedTime - _time < 0.1)
            {
                return;
            }

            if (!Input.GetMouseButton(0))
            {
                return;
            }

            _time = state.World.Time.ElapsedTime;

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<BulletSpawnRequestComponent>(entity);
        }
    }
}