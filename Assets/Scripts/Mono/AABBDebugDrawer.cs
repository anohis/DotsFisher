namespace DotsFisher.Mono
{
    using DotsFisher.Utils;
    using System.Collections.Generic;
    using System.Linq;
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

        [SerializeField] private bool _useJob = true;
        [SerializeField] private int _batchSize = 128;
        [SerializeField] private int _maxDisplayCount = 1000;

        private List<AABB> _aabbs = new List<AABB>();

        public void Draw(IEnumerable<AABB> aabb)
        {
            _aabbs.Clear();
            _aabbs.AddRange(aabb.Take(_maxDisplayCount));
        }

        private void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }

            if (_useJob)
            {
                using var array = _aabbs.ToNativeArray(Allocator.TempJob);
                var job = new DrawJob
                {
                    AABBs = array,
                };
                var handler = job.Schedule(array.Length, _batchSize);
                handler.Complete();
            }
            else
            {
                foreach (var aabb in _aabbs)
                {
                    DebugUtils.DrawWireRect(
                        aabb.Min,
                        aabb.Max,
                        Color.green);
                }
            }
        }
    }
}
