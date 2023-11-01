using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.CoroutineToAsync
{

    public class Token 
    {
        public bool Canceled { get; private set; } = false;

        public void Cancel()
        {
            Canceled = true;
        }
    }


    public static class AsyncCaster
    {
        private static MonoBehaviour behaviour;

        static AsyncCaster()
        {
            GameObject behaviourObj = new("CoroutineCasterBehaviour");
            behaviourObj.AddComponent<AsyncCasterBehaviour>();
            behaviour = AsyncCasterBehaviour.instance;
        }

        public static void DelayInvoke(Action callback, YieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                yield return yieldInstruction;
                callback.Invoke();
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void DelayInvoke(Action callback, CustomYieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                yield return yieldInstruction;
                callback.Invoke();
            }
            behaviour.StartCoroutine(CallBackCaster());
        }


        public static void DelayInvoke(Action callback, Token token, YieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                yield return yieldInstruction;
                if (!token.Canceled)
                {
                    callback.Invoke();
                }
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void DelayInvoke(Action callback, Token token, CustomYieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                yield return yieldInstruction;
                if (!token.Canceled)
                {
                    callback.Invoke();
                }
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void IntervalInvoke(Action callback, YieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                while (true)
                {
                    yield return yieldInstruction;
                    callback.Invoke();
                }
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void IntervalInvoke(Action callback, CustomYieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                while (true)
                {
                    yield return yieldInstruction;
                    callback.Invoke();
                }
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void IntervalInvoke(Action callback, Token token, YieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                while (!token.Canceled)
                {
                    yield return yieldInstruction;
                    callback.Invoke();
                }
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void IntervalInvoke(Action callback, Token token, CustomYieldInstruction yieldInstruction = null)
        {
            IEnumerator CallBackCaster()
            {
                while (!token.Canceled)
                {
                    yield return yieldInstruction;
                    callback.Invoke();
                }
            }
            behaviour.StartCoroutine(CallBackCaster());
        }

        public static void StopAll()
        {
            behaviour.StopAllCoroutines();
        }
    }

}

