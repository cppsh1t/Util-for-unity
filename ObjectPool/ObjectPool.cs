#pragma warning disable CS8618
#nullable enable

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Unity.VisualScripting;

namespace UnityUtil.Pool
{
    public interface IObjectPoolBase
    {
        /// <summary>
        /// 空闲的对象的数量
        /// </summary>
        int IdledCount { get; }

        /// <summary>
        /// 使用中的的对象的数量
        /// </summary>
        int EngagedCount { get; }

        /// <summary>
        /// 总的对象的数量
        /// </summary>
        int Count { get; }

        object ReleaseNoGeneric();
        ICollection<object> ReleaseNoGeneric(int count);

        void RecycleNoGeneric(object item);

        void RecycleNoGeneric(ICollection<object> items);

        /// <summary>
        /// 清空对象池
        /// </summary>
        void Clear();

        /// <summary>
        /// 重新分配对象池大小并加载
        /// </summary>
        /// <param name="count">新的大小</param>
        void Resize(int count, bool needPreload = true);

        /// <summary>
        /// 进行预加载
        /// </summary>
        void Preload();

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="count">释放的对象的数量</param>
        void Dipose(int count);


    }



    public interface IObjectPool<T> : IObjectPoolBase
    {

        /// <summary>
        /// 分配对象资源给外界
        /// </summary>
        /// <returns>对象池中的一个对象</returns>
        T Release();

        /// <summary>
        /// 分配对象资源给外界
        /// </summary>
        /// <param name="count">分配数量</param>
        /// <returns>对象池中的多个对象</returns>
        ICollection<T> Release(int count);

        /// <summary>
        /// 回收对象回对象池，警告:回收不属于对象池的对象可能会造成问题！
        /// </summary>
        /// <param name="item">回收的对象</param>
        void Recycle(T item);

        /// <summary>
        /// 回收对象回对象池，警告:回收不属于对象池的对象可能会造成问题！
        /// </summary>
        /// <param name="items">回收的对象们</param>
        void Recycle(ICollection<T> items);

    }


    public abstract class ObjectPool<T> : IObjectPool<T>
    {
        public int Capacity { get; protected set; }
        protected Stack<T> pool;

        public int IdledCount => pool.Count;

        public int EngagedCount { get; protected set; }

        public int CanProvideCount => Capacity - EngagedCount;

        public int Count => IdledCount + EngagedCount;

        protected int increment = 4;

        protected abstract void OnEnableObject(T item);
        protected abstract void OnDisableObject(T item);
        protected abstract void OnDiposeObject(T item);

        protected void ProcessObjects(Action<T> action, ICollection<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                action(items.ElementAt(i));
            }
        }


        /// <summary>
        /// 初始化对象池: 设置大小和相关分配设置
        /// </summary>
        /// <param name="initialCount">初始大小</param>
        /// <param name="option">分配设置</param>
        protected virtual void Init(int initialCount)
        {
            Capacity = initialCount;
            pool = new Stack<T>(initialCount);
        }

        public virtual void Preload()
        {
            int needCount = Capacity - Count;
            if (needCount != 0)
            {
                var objects = GetObjects(needCount);
                foreach (var obj in objects)
                {
                    pool.Push(obj);
                }
            }
        }

        public void Recycle(T item)
        {
            Recycle(new T[1] { item });
        }

        public virtual void Recycle(ICollection<T> items)
        {
            ProcessObjects(OnDisableObject, items);
            foreach (var item in items)
            {
                pool.Push(item);
            }
            EngagedCount -= items.Count;
        }

        protected abstract ICollection<T> GetObjects(int count);

        public T Release()
        {
            return Release(1).First();
        }


        public ICollection<T> Release(int applyCount)
        {
            var (instances, extraCount) = ReleaseInCanProvide(applyCount);
            if (extraCount != 0)
            {
                var newInstances = ReleaseInExtra(extraCount);
                instances.AddRange(newInstances);
            }

            ProcessObjects(OnEnableObject, instances);
            return instances;
        }

        protected virtual (ICollection<T> objects, int extraCount) ReleaseInCanProvide(int applyCount)
        {
            int targetCount = applyCount > CanProvideCount ? CanProvideCount : applyCount;
            int extraCount = applyCount > CanProvideCount ? applyCount - CanProvideCount : 0;
            ICollection<T> instances;

            if (targetCount <= IdledCount)
            {
                instances = pool.Pop(targetCount);
            }
            else
            {
                instances = new List<T>(targetCount);
                var hadItems = pool.Pop(IdledCount);
                instances.AddRange(hadItems);

                int stillCount = targetCount - IdledCount;
                var newItems = GetObjects(stillCount);
                instances.AddRange(newItems);
            }

            EngagedCount += targetCount;
            return (instances, extraCount);
        }


        protected virtual ICollection<T> ReleaseInExtra(int extraCount)
        {
            EngagedCount += extraCount;
            Resize(Capacity + extraCount + increment, false);
            return GetObjects(extraCount);
        }

        public virtual void Resize(int count, bool needPreload = true)
        {
            if (count == Capacity) return;
            bool bigger = count > Capacity;
            if (bigger)
            {
                int addCount = count - Capacity;
                var list = new List<T>(count);
                list.AddRange(pool);
                if (needPreload)
                {
                    var items = GetObjects(addCount);
                    list.AddRange(items);
                }
                Capacity = count;
                pool = new Stack<T>(list);
            }

            else
            {
                int newCapacity = Capacity - count;
                int getCount = IdledCount > newCapacity ? newCapacity : IdledCount;
                var items = pool.Pop(getCount);
                ProcessObjects(OnDiposeObject, items);
                pool = new Stack<T>(newCapacity);
                pool.PushAll(items);
            }
        }

        public virtual void Clear()
        {
            ProcessObjects(OnDiposeObject, pool.ToArray());
            pool.Clear();
        }

        public void Dipose(int count)
        {
            count = Capacity > count ? count : Capacity;
            var items = pool.Pop(count);
            ProcessObjects(OnDiposeObject, items);
        }

        object IObjectPoolBase.ReleaseNoGeneric()
        {
            return Release()!;
        }

        ICollection<object> IObjectPoolBase.ReleaseNoGeneric(int count)
        {
            return Release(count).Cast<object>().ToArray();
        }

        public void RecycleNoGeneric(object item)
        {
            Recycle((T)item);
        }

        public void RecycleNoGeneric(ICollection<object> items)
        {
            Recycle(items.Cast<T>().ToArray());
        }
    }

    public class GeneralObjectPool<T> : ObjectPool<T>
    {
        private readonly Action<T> onDisableCallBack;
        private readonly Action<T> onEnableCallBack;
        private readonly Action<T> onDiposeCallBack;

        private readonly Func<T> objectGetter;

        public GeneralObjectPool(int initialCount, Func<T> objectGetter, Action<T>? onEnableCallBack = null,
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


    static class ObjectPoolExtension
    {
        public static ICollection<T> Pop<T>(this Stack<T> self, int count)
        {
            var list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(self.Pop());
            }
            return list;
        }

        public static void PushAll<T>(this Stack<T> self, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                self.Push(item);
            }
        }
    }
}
