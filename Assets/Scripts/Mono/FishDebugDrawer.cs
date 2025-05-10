namespace DotsFisher.Mono
{
    using DotsFisher.Utils;
    using System.Collections.Generic;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

    public class FishDebugDrawer : MonoBehaviour
    {
        public struct Fish
        {
            public Vector3 Position;
            public float Radius;
        }

        [BurstCompile]
        private struct DrawJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Fish> Fishes;

            public void Execute(int index)
            {
                DebugUtils.DrawWireCircle(
                    Fishes[index].Position,
                    Fishes[index].Radius,
                    Color.yellow,
                    segments: 8);
            }
        }

        private List<Fish> _fishes = new List<Fish>();

        public void Draw(IEnumerable<Fish> fishes)
        {
            _fishes.Clear();
            _fishes.AddRange(fishes);
        }

        private void OnDrawGizmos()
        {
            //using var array = _fishes.ToNativeArray(Allocator.TempJob);
            //var job = new DrawJob
            //{
            //    Fishes = array,
            //};
            //var handler = job.Schedule(array.Length, 32);
            //handler.Complete();

            foreach (var fish in _fishes)
            {
                DebugUtils.DrawWireCircle(
                   fish.Position,
                   fish.Radius,
                   Color.yellow,
                   segments: 8);
            }
        }
    }
}
