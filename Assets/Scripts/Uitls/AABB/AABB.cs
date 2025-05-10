namespace DotsFisher.Utils
{
    using Unity.Mathematics;

    public struct AABB
    {
        public float2 Min;
        public float2 Max;

        public float Area
        {
            get
            {
                var size = Max - Min;
                return size.x * size.y;
            }
        }

        public static AABB operator +(AABB a, AABB b)
            => new AABB
            {
                Min = math.min(a.Min, b.Min),
                Max = math.max(a.Max, b.Max),
            };

        public static bool IsOverlap(AABB a, AABB b)
            => a.Min.x <= b.Max.x
            && a.Max.x >= b.Min.x
            && a.Min.y <= b.Max.y
            && a.Max.y >= b.Min.y;

        public static bool IsContains(AABB a, AABB b)
        {
            return math.all(a.Min <= b.Min) && math.all(a.Max >= b.Max);
        }

        public static AABB Expand(AABB a, float size)
        {
            return new AABB
            {
                Min = a.Min - size,
                Max = a.Max + size,
            };
        }

        public override string ToString()
        {
            return $"Min: {Min}, Max: {Max}";
        }
    }
}