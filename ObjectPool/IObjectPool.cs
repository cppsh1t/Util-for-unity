using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;


namespace UnityUtil.Pool
{

    public interface IObjectPoolBase
    {

        /// <summary>
        /// 目前池内数量
        /// </summary>
        int Count { get; }

        int Capacity {get;}

        object ReleaseNoGeneric();
        object[] ReleaseNoGeneric(int count);

        void RecycleNoGeneric(object item);

        void RecycleNoGeneric(IEnumerable<object> items);

        void Preload();

        /// <summary>
        /// 释放对象池
        /// </summary>
        void Clear();

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="count">释放的对象的数量</param>
        void DiposeItems(int count);
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
        T[] Release(int count);

        /// <summary>
        /// 回收对象回对象池，警告:回收不属于对象池的对象可能会造成问题！
        /// </summary>
        /// <param name="item">回收的对象</param>
        void Recycle(T item);

        /// <summary>
        /// 回收对象回对象池，警告:回收不属于对象池的对象可能会造成问题！
        /// </summary>
        /// <param name="items">回收的对象们</param>
        void Recycle(IEnumerable<T> items);

    }

}