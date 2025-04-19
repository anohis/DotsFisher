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

        public static void DrawWireRect(
            Vector2 min,
            Vector2 max,
            Color color,
            int normalAxis = 1,
            float duration = 0)
        {
            var bottomLeft = min;
            var topLeft = new Vector2(min.x, max.y);
            var topRight = max;
            var bottomRight = new Vector2(max.x, min.y);

            Debug.DrawLine(ToVector3(bottomLeft), ToVector3(topLeft), color, duration);
            Debug.DrawLine(ToVector3(topLeft), ToVector3(topRight), color, duration);
            Debug.DrawLine(ToVector3(topRight), ToVector3(bottomRight), color, duration);
            Debug.DrawLine(ToVector3(bottomRight), ToVector3(bottomLeft), color, duration);

            Vector3 ToVector3(Vector2 position)
            {
                return normalAxis switch
                {
                    0 => new Vector3(0, position.x, position.y),
                    2 => new Vector3(position.x, position.y, 0),
                    _ => new Vector3(position.x, 0, position.y),
                };
            }
        }
    }
}