namespace DotsFisher
{
    using Cysharp.Threading.Tasks;
    using DotsFisher.Conponent;
    using System.Threading;
    using Unity.Entities;

    public class Main
    {
        public async UniTask Run(CancellationToken token)
        {
            var world = DefaultWorldInitialization.Initialize("DotsFisher");

            var fixedSimulationGroup = world.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();
            if (fixedSimulationGroup != null)
            {
                fixedSimulationGroup.Timestep = 1.0f / 60;
            }

            var entity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponent<BulletSpawnRequestComponent>(entity);

            while (!token.IsCancellationRequested)
            {
                await UniTask.NextFrame();
            }

            world.Dispose();
        }
    }
}