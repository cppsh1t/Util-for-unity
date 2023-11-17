using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

namespace UnityUtil.AudioDispatcher
{
    public class AudioPlayer : MonoBehaviour
    {
        internal AudioPool pool;
        private AudioToken token;
        public AudioSource AudioSource { get; internal set; }
        protected bool start = false;
        protected bool stop = false;
        protected virtual bool NeedRecycle => (start == true && !AudioSource.isPlaying) || stop || TokenCanceled;
        protected Coroutine endPlayCoroutine;
        protected bool TokenCanceled => token != null && token.Canceled;

        void OnEnable()
        {
            start = false;
            stop = false;
            endPlayCoroutine = null;
        }

        public void Play(AudioToken token = null)
        {
            this.token ??= token;
            if (!start)
            {
                start = true;
                AudioSource.Play();
                endPlayCoroutine = StartCoroutine(nameof(WaitToEnd));
            }
        }

        public void Stop()
        {
            stop = true;
            endPlayCoroutine ??= StartCoroutine(nameof(WaitToEnd));
        }


        protected IEnumerator WaitToEnd()
        {
            yield return null;
            while (!NeedRecycle)
            {
                yield return null;
            }
            token = null;
            Recycle();
        }

        public void Recycle()
        {
            pool.Recycle(this);
        }
    }

}