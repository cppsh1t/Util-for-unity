using System;
using System.Collections.Generic;

namespace Dwell
{
    public interface IBean
    {
        void Init();
    }

    public interface IContainer
    {
        bool Register<T>(T singleton);
        bool Register(Type type, object singleton);
        T Resolve<T>();
        object Resolve(Type type);
    }

    public interface IEventBus
    {
        void Subscribe<T>(Delegate @delegate);
        void Subscribe<T>(Action<T> action);
        void UnSubscribe<T>(Delegate @delegate);
        void Publish<T>(T item);
        void Publish<T>();
    }



    public class AbstractContainer : IContainer, IEventBus
    {
        private readonly Dictionary<Type, object> singletonMap = new();
        private readonly Dictionary<Type, List<Delegate>> callbackMap = new();

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            return singletonMap.GetValueOrDefault(type);
        }

        public bool Register<T>(T singleton)
        {
            return Register(typeof(T), singleton);
        }

        public bool Register(Type type, object singleton)
        {
            return singletonMap.TryAdd(type, singleton);
        }

        public void Subscribe<T>(Action<T> action)
        {
            Subscribe<T>(action as Delegate);
        }

        public void Subscribe<T>(Delegate @delegate)
        {
            Type type = typeof(T);
            if (callbackMap.ContainsKey(type))
            {
                var list = callbackMap[type];
                list.Add(@delegate);
            }
            else
            {
                var list = new List<Delegate>() { @delegate };
                callbackMap.Add(type, list);
            }
        }

        public void UnSubscribe<T>(Delegate @delegate)
        {
            Type type = typeof(T);
            if (callbackMap.ContainsKey(type))
            {
                var list = callbackMap[type];
                list.Remove(@delegate);
                if (list.Count == 0) callbackMap.Remove(type);
            }
        }

        public void Publish<T>(T item)
        {
            Type type = typeof(T);
            if (callbackMap.ContainsKey(type))
            {
                var list = callbackMap[type];
                list.ForEach(cb => cb.DynamicInvoke(item));
            }
        }

        public void Publish<T>()
        {
            Type type = typeof(T);
            if (callbackMap.ContainsKey(type))
            {
                var list = callbackMap[type];
                list.ForEach(cb =>
                {
                    Action action = cb as Action;
                    action?.Invoke();
                });
            }
        }

        protected virtual void Init() { }

        protected virtual void InitBeans()
        {
            foreach (var bean in singletonMap.Values)
            {
                (bean as IBean)?.Init();
            }
        }

    }


    public partial class GameRoot : AbstractContainer
    {
        public static GameRoot Instance => GetInstance();
        private static GameRoot _instacne;
        private static GameRoot GetInstance()
        {
            if (_instacne == null)
            {
                GameRoot gameRoot = new();
                _instacne = gameRoot;
                _instacne.Init();
                _instacne.InitBeans();
            }
            return _instacne;
        }

    }
}

