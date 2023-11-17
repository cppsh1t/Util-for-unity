using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

namespace UnityUtil.Pool
{

    public abstract class GameObjectPool : ObjectPool<GameObject>
    {
        protected readonly GameObject template;
        public GameObjectPool(GameObject template, int capacity) : base(capacity)
        {
            this.template = template;
        }

        protected override GameObject CreateItem()
        {
            GameObject instance = UnityEngine.Object.Instantiate(template);
            instance.SetActive(false);
            return instance;
        }

        protected override void OnEnableItem(GameObject item)
        {
            item.SetActive(true);
        }

        protected override void OnDisableItem(GameObject item)
        {
            item.SetActive(false);
        }

        protected override void OnDiposeItem(GameObject item)
        {
            UnityEngine.Object.Destroy(item);
        }

    }

    public class GeneralGameObjectPool : GameObjectPool
    {
        private readonly Action<GameObject> onInitCallBack;
        private readonly Action<GameObject> onEnableCallBack;
        private readonly Action<GameObject> onDisableCallBack;
        private readonly Action<GameObject> onDiposeCallBack;

        public GeneralGameObjectPool(GameObject template, int capacity, Action<GameObject> onInitCallBack = null,
            Action<GameObject> onEnableCallBack = null, Action<GameObject> onDisableCallBack = null,
            Action<GameObject> onDiposeCallBack = null) : base(template, capacity)
        {
            onInitCallBack ??= _ => { };
            onEnableCallBack ??= _ => { };
            onDisableCallBack ??= _ => { };
            onDiposeCallBack ??= _ => { };
            this.onInitCallBack += onInitCallBack;
            this.onEnableCallBack += onEnableCallBack;
            this.onDisableCallBack += onDisableCallBack;
            this.onDiposeCallBack += onDiposeCallBack;
        }


        protected override GameObject CreateItem()
        {
            GameObject instance = base.CreateItem();
            onInitCallBack.Invoke(instance);
            return instance;
        }

        protected override void OnEnableItem(GameObject item)
        {
            base.OnEnableItem(item);
            onEnableCallBack(item);
        }

        protected override void OnDisableItem(GameObject item)
        {
            onDisableCallBack(item);
            base.OnDisableItem(item);
        }

        protected override void OnDiposeItem(GameObject item)
        {
            onDiposeCallBack(item);
            base.OnDiposeItem(item);
        }
    }

}