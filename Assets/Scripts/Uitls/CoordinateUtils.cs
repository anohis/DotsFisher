namespace DotsFisher.Utils
{
    using Unity.Mathematics;
    using UnityEngine;

    public static class CoordinateUtils
    {
        public static Vector3 ToPosition3D(this float2 position)
            => new Vector3(
                position.x,
                0,
                position.y);

        public static float2 RadiansToDirection(float radians)
            => new float2(math.cos(radians), math.sin(radians));
    }
}