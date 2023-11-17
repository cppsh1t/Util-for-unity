#nullable enable

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;



namespace UnityUtil.Pool
{
    public abstract class ObjectPool<T> : IObjectPool<T> where T : class
    {
        protected Stack<T> pool = new();
        public int Count => pool.Count;
        public int Capacity { get; protected set; }

        protected abstract T CreateItem();
        protected abstract void OnEnableItem(T item);
        protected abstract void OnDisableItem(T item);
        protected abstract void OnDiposeItem(T item);

        public ObjectPool(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("capacity must bigger than 0");
            Capacity = capacity;
        }

        public void Preload()
        {
            int need = Capacity - Count;
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
                item = CreateItem();
                Capacity++;
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
        }

        public void DiposeItems(int count)
        {
            if (count <= 0) throw new ArgumentException("count must bigger than 0");
            count = Math.Min(count, Count);
            for(int i = 0; i < count; i++)
            {
                T item = pool.Pop();
                OnDiposeItem(item);
            }
        }

    }

    public class GeneralObjectPool<T> : ObjectPool<T> where T : class
    {
        private readonly Action<T> onDisableCallBack;
        private readonly Action<T> onEnableCallBack;
        private readonly Action<T> onDiposeCallBack;

        private readonly Func<T> objectGetter;

        public GeneralObjectPool(int initialCount, Func<T> objectGetter, Action<T>? onEnableCallBack = null,
            Action<T>? onDisableCallBack = null, Action<T>? onDiposeCallBack = null) : base(initialCount)
        {
            this.objectGetter = objectGetter;
            onEnableCallBack ??= _ => { };
            onDisableCallBack ??= _ => { };
            onDiposeCallBack ??= _ => { };
            this.onEnableCallBack += onEnableCallBack;
            this.onDisableCallBack += onDisableCallBack;
            this.onDiposeCallBack += onDiposeCallBack;
        }

        protected override T CreateItem()
        {
            return objectGetter.Invoke();
        }

        protected override void OnEnableItem(T item)
        {
            onEnableCallBack(item);
        }

        protected override void OnDisableItem(T item)
        {
            onDisableCallBack(item);
        }

        protected override void OnDiposeItem(T item)
        {
            onDiposeCallBack(item);
        }
    }
}