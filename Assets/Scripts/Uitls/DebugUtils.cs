namespace DotsFisher.Utils
{
    using UnityEngine;

    public static class DebugUtils
    {
        public static void DrawWireCircle(
            Vector3 center,
            float radius,
            Color color,
            int segments = 32,
            int normalAxis = 1,
            float duration = 0)
        {
            segments = Mathf.Max(8, segments);

            var (axisA, axisB) = normalAxis switch
            {
                0 => (Vector3.forward, Vector3.up),
                2 => (Vector3.right, Vector3.up),
                _ => (Vector3.right, Vector3.forward),
            };

            var angleIncrement = 2f * Mathf.PI / segments;
            var prevPoint = center + axisA * radius;

            for (int i = 1; i <= segments; i++)
            {
                var angle = i * angleIncrement;
                var nextPoint = center
                    + (Mathf.Cos(angle) * axisA
                        + Mathf.Sin(angle) * axisB) * radius;

                Debug.DrawLine(prevPoint, nextPoint, color, duration);
                prevPoint = nextPoint;
            }
        }
    }
}