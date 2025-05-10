namespace DotsFisher.Mono
{
    using DotsFisher.Utils;
    using System.Collections.Generic;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

    public class AABBDebugDrawer : MonoBehaviour
    {
        public struct AABB
        {
            public Vector2 Min;
            public Vector2 Max;
        }

        [BurstCompile]
        private struct DrawJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<AABB> AABBs;

            public void Execute(int index)
            {
                DebugUtils.DrawWireRect(
                    AABBs[index].Min,
                    AABBs[index].Max,
                    Color.green);
            }
        }

        private List<AABB> _aabbs = new List<AABB>();

        public void Draw(IEnumerable<AABB> aabb)
        {
            _aabbs.Clear();
            _aabbs.AddRange(aabb);
        }

        private void OnDrawGizmos()
        {
            using var array = _aabbs.ToNativeArray(Allocator.TempJob);
            var job = new DrawJob
            {
                AABBs = array,
            };
            var handler = job.Schedule(array.Length, 32);
            handler.Complete();
        }
    }
}
