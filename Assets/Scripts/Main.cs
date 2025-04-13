namespace DotsFisher
{
    using Cysharp.Threading.Tasks;
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

            while (!token.IsCancellationRequested)
            {
                await UniTask.NextFrame();
            }

            world.Dispose();
        }
    }
}