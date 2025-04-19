namespace DotsFisher.EcsSystem
{
    using Unity.Entities;

    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial class AABBTreeSystemGroup : ComponentSystemGroup
    {
    }
}