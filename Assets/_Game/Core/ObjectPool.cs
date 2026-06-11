using System;
using System.Collections.Generic;

namespace Ruinborne.Core
{
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool;
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;

        public int CountInactive => _pool.Count;

        public ObjectPool(Func<T> factory, Action<T> onGet = null, Action<T> onReturn = null, int initialCapacity = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet;
            _onReturn = onReturn;
            _pool = new Stack<T>(initialCapacity);

            for (int i = 0; i < initialCapacity; i++)
                _pool.Push(_factory());
        }

        public T Get()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : _factory();
            _onGet?.Invoke(item);
            return item;
        }

        public void Return(T item)
        {
            if (item == null) return;
            _onReturn?.Invoke(item);
            _pool.Push(item);
        }

        public void Clear() => _pool.Clear();
    }
}
