namespace DotsFisher.Conponent
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct TestDestroySystem : ISystem
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

            if (!Input.GetMouseButton(1))
            {
                return;
            }

            _time = state.World.Time.ElapsedTime;

            var query = SystemAPI
                .QueryBuilder()
                .WithAll<AABBNodeComponent>()
                .WithNone<DestroyRequestComponent>()
                .Build();
            var entities = query.ToEntityArray(Allocator.Temp);

            if (entities.Length > 0)
            {
                var entity = entities[0];
                state.EntityManager.AddComponent<DestroyRequestComponent>(entity);
            }
        }
    }
}