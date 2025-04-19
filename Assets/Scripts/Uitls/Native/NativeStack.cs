namespace DotsFisher.Utils.Native
{
    using System;
    using Unity.Burst;
    using Unity.Collections;

    [BurstCompile]
    public struct NativeStack<T> : IDisposable where T : unmanaged
    {
        private NativeList<T> _list;

        public int Length => _list.Length;

        public NativeStack(Allocator allocator)
        {
            _list = new NativeList<T>(allocator);
        }

        public void Push(T element)
        {
            _list.Add(element);
        }

        public T Pop()
        {
            var last = Length - 1;
            var element = _list[last];
            _list.RemoveAt(last);
            return element;
        }

        public void Dispose()
        {
            _list.Dispose();
        }
    }
}



