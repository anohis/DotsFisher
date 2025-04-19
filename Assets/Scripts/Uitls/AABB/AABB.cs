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

        public override string ToString()
        {
            return $"Min: {Min}, Max: {Max}";
        }
    }
}