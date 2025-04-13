namespace DotsFisher.Utils
{
    using Unity.Mathematics;
    using UnityEngine;

    public static class CoordinateUtils
    {

        public static float2 RadiansToDirection(float radians)
            => new float2(math.cos(radians), math.sin(radians));
    }
}