namespace DotsFisher
{
    using Cysharp.Threading.Tasks;
    using DotsFisher.Conponent;
    using DotsFisher.Utils;
    using System.Threading;
    using Unity.Entities;
    using Unity.Mathematics;

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

            var rotation = math.radians(180f);
            var direction = CoordinateUtils.RadiansToDirection(rotation);

            var entity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(entity, new MovementComponent
            {
                Speed = 1,
                Direction = direction,
            });
            world.EntityManager.AddComponentData(entity, new TransformComponent
            {
                Position = float2.zero,
                Rotation = rotation,
            });

            while (!token.IsCancellationRequested)
            {
                await UniTask.NextFrame();
            }

            world.Dispose();
        }
    }
}