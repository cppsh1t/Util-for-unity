#nullable enable

using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityUtil.Pool
{
    public interface IPoolCacheBase { }

    public interface IPoolCache<T> : IPoolCacheBase
    {
        T Release();
        ICollection<T> Release(int count);

        void Recycle(T item);

        void Recycle(IEnumerable<T> items);

        void Clear();
    }

    public class PoolCache<T> : IPoolCache<T> where T : class
    {
        private readonly Func<int, ICollection<T>> objectGetter;

        private readonly Stack<WeakReference<T>> pool = new();

        public PoolCache(Func<int, ICollection<T>> func) => objectGetter = func;


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
            T? target = null;
            if (pool.Count > 0)
            {
                var arr = pool.ToArray();
                for (int i = 1; i < arr.Length; i++)
                {
                    arr[i].TryGetTarget(out target);
                    if (target != null) break;
                }
            }
            target ??= objectGetter(1).First();
            return target;
        }

        public ICollection<T> Release(int count)
        {
            var objects = new List<T>(count);
            int currentIndex = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                pool.Pop().TryGetTarget(out var item);
                if (item != null)
                {
                    objects.Add(item);
                    currentIndex++;
                }

                if (currentIndex == count) break;
            }

            if (currentIndex != count)
            {
                var needItems = objectGetter(count - currentIndex);
                objects.AddRange(needItems);
            }

            return objects;
        }

        public void Clear()
        {
            pool.Clear();
        }

    }
}