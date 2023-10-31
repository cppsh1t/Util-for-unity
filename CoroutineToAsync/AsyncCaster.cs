using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.CoroutineToAsync
{
    public interface IToken
    {
        public abstract bool Canceled { get; }
        void Cancel();
    }

    public class Token : IToken
    {
        public bool Canceled { get; private set; } = false;

        public void Cancel()
        {
            Canceled = true;
        }
    }

    public class SetToken : IToken
    {
        public bool Canceled { get; private set; } = false;

        private readonly HashSet<IToken> tokens = new();

        public void AddToken(IToken token)
        {
            tokens.Add(token);
        }

        public void RemoveToken(IToken token)
        {
            tokens.Remove(token);
        }

        public void Cancel()
        {
            Canceled = false;
            foreach (var token in tokens)
            {
                token.Cancel();
            }
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


        public static void DelayInvoke(Action callback, IToken token, YieldInstruction yieldInstruction = null)
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

        public static void DelayInvoke(Action callback, IToken token, CustomYieldInstruction yieldInstruction = null)
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

        public static void IntervalInvoke(Action callback, IToken token, YieldInstruction yieldInstruction = null)
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

