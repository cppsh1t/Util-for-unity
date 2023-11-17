using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityUtil.Pool
{
    public abstract class CacheableObjectPool<T> : IObjectPool<T> where T : class
    {
        private PoolCache<T> poolCache;
        protected Stack<T> pool = new();
        public int Count => pool.Count;
        public int Capacity { get; protected set; }

        protected abstract T CreateItem();
        protected abstract void OnEnableItem(T item);
        protected abstract void OnDisableItem(T item);
        protected abstract void OnDiposeItem(T item);

        protected CacheableObjectPool(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("capacity must bigger than 0");
            Capacity = capacity;
            poolCache = new(CreateItem);
        }

        public void Preload()
        {
            int need = Capacity - Count;
            if (need <= 0) return;
            for (int i = 0; i < need; i++)
            {
                T item = CreateItem();
                pool.Push(item);
            }
        }

        public T Release()
        {
            if (!pool.TryPop(out T item))
            {
                item = poolCache.Release();
            }
            OnEnableItem(item);
            return item;
        }

        public T[] Release(int count)
        {
            if (count <= 0) throw new ArgumentException("count must bigger than 0");
            T[] items = new T[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = Release();
            }
            return items;
        }

        public void Recycle(T item)
        {
            OnDisableItem(item);
            if (Capacity == Count)
            {
                poolCache.Recycle(item);
                return;
            }
            pool.Push(item);
        }

        public void Recycle(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Recycle(item);
            }
        }

        public void RecycleNoGeneric(object item)
        {
            if (item is T convertItem)
            {
                Recycle(convertItem);
            }
        }

        public void RecycleNoGeneric(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                RecycleNoGeneric(item);
            }
        }

        public object ReleaseNoGeneric()
        {
            return Release();
        }

        public object[] ReleaseNoGeneric(int count)
        {
            if (count <= 0) throw new ArgumentException("count must bigger than 0");
            return Release(count).Cast<object>().ToArray();
        }

        public void Clear()
        {
            foreach (var item in pool)
            {
                OnDiposeItem(item);
            }
            pool.Clear();
            poolCache.Clear();
        }


        //TODO: 应该先释放Cache里的
        public void DiposeItems(int count)
        {
            if (count <= 0) throw new ArgumentException("count must bigger than 0");
            count = Math.Min(count, Count);
            for (int i = 0; i < count; i++)
            {
                T item = pool.Pop();
                OnDiposeItem(item);
            }
        }
    }
}