using System.Collections.Generic;
using System;
using UnityEngine;

namespace UnityUtil.AudioDispatcher
{
    public class AudioToken
    {
        public bool Canceled { get; private set; } = false;

        public void Cancel()
        {
            Canceled = true;
        }

        public void Restart()
        {
            Canceled = false;
        }
    }

    [Obsolete]
    public class ObsoleteAudioPlayer
    {
        protected readonly AudioDispatcher dispatcher;

        internal ObsoleteAudioPlayer(AudioDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void Play(string path)
        {

        }

        public void Play(string path, AudioToken token)
        {

        }

        public void Play(string path, float delay)
        {

        }

        public void Play(string path, float delay, AudioToken token)
        {

        }

        public void PlayOnTransform(string path)
        {

        }

        public void PlayOnPosition(string path) 
        {
            
        }

    }
}