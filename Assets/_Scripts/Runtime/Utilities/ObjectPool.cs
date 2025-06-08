using System;
using System.Collections.Generic;

namespace RPG.Inventory
{
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        public int PoolSize => _pool.Count;

        public ObjectPool(Func<T> createFunc = null, Action<T> onGet = null, Action<T> onRelease = null, int poolSize = 2)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException("Return type function is not provided");

            _onGet = onGet;
            _onRelease = onRelease;
            _pool = new Stack<T>(poolSize);

            for (int i = 0; i < poolSize; i++)
                _pool.Push(_createFunc());
        }

        public T Get()
        {
            T item;
            item = _pool.Count > 0 ? _pool.Pop() : _createFunc();

            _onGet?.Invoke(item);

            return item;
        }

        public void Release(T item)
        {
            if (item == null) throw new ArgumentNullException("Can not release, the item provided is null");

            _onRelease?.Invoke(item);
            _pool.Push(item);
        }
    }
}