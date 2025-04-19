namespace DotsFisher.Component
{
    using Unity.Entities;
    using Unity.Mathematics;

    public partial struct TransformComponent : IComponentData
    {
        public float2 Position;
        public float Rotation;
    }
}