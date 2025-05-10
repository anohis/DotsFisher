namespace DotsFisher.Mono
{
    using DotsFisher.Utils;
    using System.Collections.Generic;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

    public class BulletDebugDrawer : MonoBehaviour
    {
        public struct Bullet
        {
            public Vector3 Position;
            public float Radius;
        }

        [BurstCompile]
        private struct DrawJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Bullet> Bullets;

            public void Execute(int index)
            {
                DebugUtils.DrawWireCircle(
                    Bullets[index].Position,
                    Bullets[index].Radius,
                    Color.red,
                    segments: 8);
            }
        }

        private List<Bullet> _bullets = new List<Bullet>();

        public void Draw(IEnumerable<Bullet> bullets)
        {
            _bullets.Clear();
            _bullets.AddRange(bullets);
        }

        private void OnDrawGizmos()
        {
            using var array = _bullets.ToNativeArray(Allocator.TempJob);
            var job = new DrawJob
            {
                Bullets = array,
            };
            var handler = job.Schedule(array.Length, 32);
            handler.Complete();
        }
    }
}
