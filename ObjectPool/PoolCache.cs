using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityUtil.Pool
{
    public interface IPoolCacheBase { }

    public interface IPoolCache<T> : IPoolCacheBase
    {
        T Release();
        T[] Release(int count);

        void Recycle(T item);

        void Recycle(IEnumerable<T> items);

        void Clear();
    }

    public class PoolCache<T> : IPoolCache<T> where T : class
    {
        private readonly Func<T> objectGetter;

        private readonly Stack<WeakReference<T>> pool = new();

        public PoolCache(Func<T> func) => objectGetter = func;


        public void Recycle(T item)
        {
            pool.Push(new WeakReference<T>(item));
        }

        public void Recycle(IEnumerable<T> items)
        {
            var weakItems = items.Select(item => new WeakReference<T>(item));
            foreach(var item in weakItems)
            {
                pool.Push(item);
            }
        }

        public T Release()
        {
            T item = default;
            pool.FirstOrDefault(arg => arg.TryGetTarget(out item));
            item ??= objectGetter.Invoke();
            return item;
        }

        public T[] Release(int count)
        {   
            if (count <= 0) throw new ArgumentException("count must bigger than 0");
            T[] items = new T[count];
            for(int i = 0; i < count; i++)
            {
                items[i] = Release();
            }
            return items;
        }

        public void Clear()
        {
            pool.Clear();
        }

    }
}