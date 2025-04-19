namespace DotsFisher.EcsComponent
{
    using DotsFisher.Utils;
    using Unity.Entities;

    public partial struct AABBTreeComponent : IComponentData
    {
        public AABBTree Tree;
    }
}