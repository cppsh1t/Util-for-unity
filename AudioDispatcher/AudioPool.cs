using System.Collections.Generic;
using System;
using UnityEngine;
using UnityUtil.Pool;

namespace UnityUtil.AudioDispatcher
{
    public class AudioPool : ObjectPool<AudioPlayer>
    {
        public AudioPool(int capacity) : base(capacity)
        {
        }

        protected override AudioPlayer CreateItem()
        {
            GameObject gameObject = new("AudioSourcePlayer");
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            gameObject.SetActive(false);
            AudioPlayer audioSourcePlayer = gameObject.AddComponent<AudioPlayer>();
            audioSourcePlayer.AudioSource = audioSource;
            audioSourcePlayer.pool = this;
            
            return audioSourcePlayer;
        }

        protected override void OnDiposeItem(AudioPlayer item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }

        protected override void OnDisableItem(AudioPlayer item)
        {
            //Process GameObject
            item.gameObject.SetActive(false);
            item.transform.position = Vector3.zero;
            item.transform.SetParent(null);

            //Process AudioSource
            AudioSource audioSource = item.AudioSource;
            audioSource.clip = null;
            audioSource.outputAudioMixerGroup = null;
            audioSource.mute = false;
            audioSource.bypassEffects = false;
            audioSource.bypassListenerEffects = false;
            audioSource.bypassReverbZones = false;
            audioSource.playOnAwake = true;
            audioSource.loop = false;
            audioSource.priority = 128;
            audioSource.volume = 1;
            audioSource.pitch = 1;
            audioSource.panStereo = 0;
            audioSource.spatialBlend = 0;
            audioSource.reverbZoneMix = 1;
            audioSource.dopplerLevel = 1;
            audioSource.spread = 0;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 1;
            audioSource.maxDistance = 500;

            //Process other Prop
            audioSource.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Speaker;
            audioSource.ignoreListenerPause = false;
            audioSource.ignoreListenerVolume = false;
            audioSource.spatialize = false;
            audioSource.spatializePostEffects = false;
            audioSource.time = 0;
            audioSource.timeSamples = 0;
            audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Auto;
        }

        protected override void OnEnableItem(AudioPlayer item)
        {
            item.gameObject.SetActive(true);
        }
    }

}