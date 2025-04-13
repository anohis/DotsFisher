namespace DotsFisher.Conponent
{
    using Unity.Entities;
    using Unity.Mathematics;

    public partial struct MovementComponent : IComponentData
    {
        public float Speed;
        public float2 Direction;
    }
}