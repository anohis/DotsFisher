namespace DotsFisher
{
    using Cysharp.Threading.Tasks;
    using DotsFisher.EcsSystem;
    using DotsFisher.Mono;
    using System.Threading;
    using Unity.Entities;

    public class Main
    {
        public async UniTask Run(
            FishDebugDrawer fishDebugDrawer,
            BulletDebugDrawer bulletDebugDrawer,
            AABBDebugDrawer aabbDebugDrawer,
            CancellationToken token)
        {
            var world = DefaultWorldInitialization.Initialize("DotsFisher");

            var fixedSimulationGroup = world.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();
            if (fixedSimulationGroup != null)
            {
                fixedSimulationGroup.Timestep = 1.0f / 60;
            }

            world.GetExistingSystemManaged<DebugDrawFishSystem>().Initialize(fishDebugDrawer);
            world.GetExistingSystemManaged<DebugDrawBulletSystem>().Initialize(bulletDebugDrawer);
            world.GetExistingSystemManaged<DebugDrawAABBTreeSystem>().Initialize(aabbDebugDrawer);

            while (!token.IsCancellationRequested)
            {
                await UniTask.NextFrame();
            }

            world.Dispose();
        }
    }
}