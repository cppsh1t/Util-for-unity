using System.Collections.Generic;
using System;
using UnityEngine;

namespace UnityUtil.AudioDispatcher
{
    public class AudioOption
    {
        public static AudioOption DefaultOption { get; } = new AudioOption("");
        public string Path { get; set; }
        public string AudioMixerName { get; set; } = "Master";
        public bool Mute { get; set; } = false;
        public bool BypassEffects { get; set; } = false;
        public bool BypassListenerEffects { get; set; } = false;
        public bool BypassReverbZones { get; set; } = false;
        public bool PlayOnAwake { get; set; } = false;
        public bool Loop { get; set; } = false;
        public int Priority { get; set; } = 128;
        public float Volume { get; set; } = 1;
        public float Pitch { get; set; } = 1;
        public float PanStereo { get; set; } = 0;
        public float SpatialBlend { get; set; } = 0;
        public float ReverbZoneMix { get; set; } = 1;
        public float DopplerLevel { get; set; } = 1;
        public float Spread { get; set; } = 0;
        public AudioRolloffMode RolloffMode { get; set; } = AudioRolloffMode.Logarithmic;
        public float MinDistance { get; set; } = 1;
        public float MaxDistance { get; set; } = 500;
        public GamepadSpeakerOutputType GamepadSpeakerOutputType { get; set; } = GamepadSpeakerOutputType.Speaker;
        public bool IgnoreListenerPause { get; set; } = false;
        public bool IgnoreListenerVolume { get; set; } = false;
        public bool Spatialize { get; set; } = false;
        public bool SpatializePostEffects { get; set; } = false;
        public float StartTime { get; set; } = 0;
        public int TimeSamples { get; set; } = 0;
        public AudioVelocityUpdateMode VelocityUpdateMode { get; set; } = AudioVelocityUpdateMode.Auto;
        public AudioOption(string path) 
        {
            Path = path;
        }
    }
}