#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.Pool
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        private readonly Action<GameObject> onDisableCallBack;
        private readonly Action<GameObject> onEnableCallBack;
        private readonly Action<GameObject> onDiposeCallBack;
        private readonly Action<GameObject> onInitCallBack;
        private readonly GameObject template;

        public GameObjectPool(int initialCount, GameObject template, Action<GameObject>? onEnableCallBack = null,
        Action<GameObject>? onDisableCallBack = null, Action<GameObject>? onDiposeCallBack = null, Action<GameObject>? onInitCallBack = null)
        {
            Init(initialCount);
            this.template = template;
            onEnableCallBack ??= _ => { };
            onDisableCallBack ??= _ => { };
            onDiposeCallBack ??= _ => { };
            onInitCallBack ??= _ => { };
            this.onEnableCallBack += onEnableCallBack;
            this.onDisableCallBack += onDisableCallBack;
            this.onDiposeCallBack += onDiposeCallBack;
            this.onInitCallBack += onInitCallBack;

            this.onDisableCallBack += obj =>
            {
                obj.SetActive(false);
            };
            this.onEnableCallBack += obj =>
            {
                obj.SetActive(true);
            };
            this.onDiposeCallBack += obj =>
            {
                UnityEngine.Object.Destroy(obj);
            };
        }

        protected override ICollection<GameObject> GetObjects(int count)
        {
            GameObject[] arr = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                GameObject item = UnityEngine.Object.Instantiate(template);
                arr[i] = item;
                onInitCallBack(item);
            }
            return arr;
        }

        protected override void OnDisableObject(GameObject item)
        {
            onDisableCallBack(item);
        }

        protected override void OnEnableObject(GameObject item)
        {
            onEnableCallBack(item);
        }

        protected override void OnDiposeObject(GameObject item)
        {
            onDiposeCallBack(item);
        }
    }

}

