namespace DotsFisher.Conponent
{
    using Unity.Burst;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<BulletSpawnRequestComponent>(entity);
        }
    }
}