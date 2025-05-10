namespace DotsFisher.EcsSystem
{
    using DotsFisher.EcsComponent;
    using DotsFisher.Mono;
    using DotsFisher.Utils;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Entities;

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class DebugDrawAABBTreeSystem : SystemBase
    {
        private AABBDebugDrawer _drawer;
        private List<AABBDebugDrawer.AABB> _cache = new List<AABBDebugDrawer.AABB>();

        public void Initialize(AABBDebugDrawer drawer)
        {
            _drawer = drawer;
        }

        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<AABBTreeComponent>();
        }

        protected override void OnUpdate()
        {
            var aabbTree = SystemAPI.GetSingleton<AABBTreeComponent>();
            var list = new NativeList<AABB>(Allocator.Temp);

            aabbTree.Tree.Query(ref list);

            _cache.Clear();
            for (int i = 0; i < list.Length; i++)
            {
                _cache.Add(new AABBDebugDrawer.AABB
                {
                    Min = list[i].Min,
                    Max = list[i].Max,
                });
            }
            _drawer.Draw(_cache);

            list.Dispose();
        }
    }
}