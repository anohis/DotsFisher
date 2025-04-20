namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(AABBTreeSystemGroup))]
    public partial struct AABBTreeQuerySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AABBTreeComponent>();
            state.RequireForUpdate<AABBQueryRequestComponent>();
            state.RequireForUpdate<TransformComponent>();
            state.RequireForUpdate<CircleColliderComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var aabbTree = SystemAPI.GetSingleton<AABBTreeComponent>();

            var entityMap = new NativeHashMap<uint, (Entity, float2, float)>(32, Allocator.Temp);
            foreach (var (aabb, transform, collider, entity)
                in SystemAPI
                    .Query<RefRO<AABBNodeComponent>, RefRO<TransformComponent>, RefRO<CircleColliderComponent>>()
                    .WithNone<DestroyRequestComponent>()
                    .WithEntityAccess())
            {
                entityMap[aabb.ValueRO.EntryId] = (entity, transform.ValueRO.Position, collider.ValueRO.Radius);
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, collider)
                in SystemAPI
                    .Query<RefRO<TransformComponent>, RefRO<CircleColliderComponent>>()
                    .WithAll<AABBQueryRequestComponent>()
                    .WithNone<DestroyRequestComponent>())
            {
                var position = transform.ValueRO.Position;
                var radius = collider.ValueRO.Radius;

                var result = new NativeList<uint>(Allocator.Temp);

                var aabb = new Utils.AABB
                {
                    Min = position - radius,
                    Max = position + radius,
                };

                aabbTree.Tree.Query(aabb, ref result);

                foreach (var entryId in result)
                {
                    var (target, targetPos, targetRadius) = entityMap[entryId];

                    if (math.distancesq(position, targetPos) <= math.pow(radius + targetRadius, 2))
                    {
                        ecb.AddComponent<DestroyRequestComponent>(target);
                        break;
                    }
                }

                result.Dispose();
            }

            entityMap.Dispose();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}