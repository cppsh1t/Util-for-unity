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
        protected float clipLength;
        protected bool start = false;
        protected bool stop = false;
        protected bool PlayCompleted => GetAudioCompleted();
        protected bool NeedRecycle => PlayCompleted || stop || TokenCanceled;
        protected Coroutine endPlayCoroutine;
        protected bool TokenCanceled => token != null && token.Canceled;

        private bool trace = false;
        private Transform traceTarget;
        private GameObject traceObj;

        private bool GetAudioCompleted()
        {
            if (AudioSource.loop) return false;

            return AudioSource.time >= clipLength;
        }

        private void SetTrace(Transform target)
        {
            trace = true;
            traceTarget = target;
            traceObj = traceTarget.gameObject;
        }

        void OnEnable()
        {
            start = false;
            stop = false;
            endPlayCoroutine = null;
        }

        void OnDisable()
        {
            trace = false;
            traceTarget = null;
            traceObj = null;
        }

        void Update()
        {
            if (trace && traceObj)
            {
                transform.position = traceTarget.position;
            }
        }

        public void Play(AudioToken token = null)
        {
            clipLength = AudioSource.clip.length;
            this.token ??= token;
            if (!start)
            {
                start = true;
                AudioSource.Play();
                endPlayCoroutine = StartCoroutine(nameof(WaitToEnd));
            }
        }

        public void PlayOnTransform(Transform transform, AudioToken token = null)
        {
            SetTrace(transform);
            Play(token);
        }

        public void PlayOnPosition(Vector3 position, AudioToken token = null)
        {
            transform.position = position;
            Play(token);
        }

        public void PlayDelayed(float delay, AudioToken token = null)
        {
            clipLength = AudioSource.clip.length;
            this.token ??= token;
            if (!start)
            {
                start = true;
                AudioSource.PlayDelayed(delay);
                endPlayCoroutine = StartCoroutine(nameof(WaitToEnd));
            }
        }

        public void Stop()
        {
            stop = true;
            endPlayCoroutine ??= StartCoroutine(nameof(WaitToEnd));
        }

        public void Pause()
        {
            AudioSource.Pause();
        }

        public void UnPause()
        {
            AudioSource.UnPause();
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