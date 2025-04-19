namespace DotsFisher.Utils
{
    public static class MathUtils
    {
        public static int NextPowerOfTwo(int n)
        {
            if (n <= 0)
            {
                return 1;
            }
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            return n + 1;
        }
    }
}