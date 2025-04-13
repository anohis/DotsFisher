using Unity.Entities;

namespace DotsFisher
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class GameSystemGroup : ComponentSystemGroup
    {
    }
}