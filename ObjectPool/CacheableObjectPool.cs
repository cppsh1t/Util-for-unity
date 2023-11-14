#pragma warning disable CS8618
#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityUtil.Pool
{
    public abstract class CacheableObjectPool<T> : ObjectPool<T> where T : class
    {
        private PoolCache<T> poolCache;

        protected override void Init(int initialCount)
        {
            base.Init(initialCount);
            poolCache = new PoolCache<T>(GetObjects);
        }

        protected override ICollection<T> ReleaseInExtra(int extraCount)
        {
            return poolCache.Release(extraCount);
        }

        public override void Recycle(ICollection<T> items)
        {
            ProcessObjects(OnDisableObject, items);
            int itemsCount = items.Count;
            if (itemsCount <= EngagedCount)
            {
                foreach (var item in items)
                {
                    pool.Push(item);
                }
                EngagedCount -= items.Count;
            }
            else
            {
                var enaggedItems = items.Take(EngagedCount);
                var obsItems = items.Skip(EngagedCount);
                foreach (var item in enaggedItems)
                {
                    pool.Push(item);
                }
                EngagedCount = 0;
                poolCache.Recycle(obsItems.ToArray());
            }

        }

        public override void Clear()
        {
            base.Clear();
            poolCache.Clear();
        }
    }

    public class GeneralCacheableObjectPool<T> : CacheableObjectPool<T> where T : class
    {
        private readonly Action<T> onDisableCallBack;
        private readonly Action<T> onEnableCallBack;
        private readonly Action<T> onDiposeCallBack;

        private readonly Func<T> objectGetter;

        public GeneralCacheableObjectPool(int initialCount, Func<T> objectGetter, Action<T>? onEnableCallBack = null,
        Action<T>? onDisableCallBack = null, Action<T>? onDiposeCallBack = null)
        {
            Init(initialCount);
            this.objectGetter = objectGetter;
            onEnableCallBack ??= _ => { };
            onDisableCallBack ??= _ => { };
            onDiposeCallBack ??= _ => { };
            this.onEnableCallBack += onEnableCallBack;
            this.onDisableCallBack += onDisableCallBack;
            this.onDiposeCallBack += onDiposeCallBack;
        }

        protected override ICollection<T> GetObjects(int count)
        {
            T[] arr = new T[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = objectGetter();
            }
            return arr;
        }

        protected override void OnDisableObject(T item)
        {
            onDisableCallBack(item);
        }

        protected override void OnEnableObject(T item)
        {
            onEnableCallBack(item);
        }

        protected override void OnDiposeObject(T item)
        {
            onDiposeCallBack(item);
        }
    }
}