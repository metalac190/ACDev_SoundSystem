using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    public abstract class SFXEvent : ScriptableObject
    {
        [Header("General Settings")]
        [SerializeField] AudioClip[] _possibleClips = new AudioClip[0];
        [SerializeField] AudioMixerGroup _mixer = null;
        [Space]

        [Range(0, 128)][SerializeField] 
        int _priority = 128;

        [SerializeField][MinMaxRange(0, 1)]
        RangedFloat _volume = new RangedFloat(.8f, .8f);

        [SerializeField][MinMaxRange(0, 2)]
        RangedFloat _pitch = new RangedFloat(.95f, 1.05f);

        [SerializeField][Range(-1, 1)]
        private float _stereoPan = 0;

        [SerializeField] [Range(0, 1)]
        float _spatialBlend = 0;

        [Header("3D Settings")]
        //TODO hide these fields if 2D is true
        [SerializeField] float _attenuationMin = 1;
        [SerializeField] float _attenuationMax = 500;

        int _clipIndex = 0;

        public AudioClip Clip => _possibleClips[_clipIndex];
        public AudioMixerGroup Mixer => _mixer;

        public int Priority => _priority;
        public float Volume { get; private set; }
        public float Pitch { get; private set; }
        public float StereoPan => _stereoPan;

        public float SpatialBlend => _spatialBlend;

        public float AttenuationMin => _attenuationMin;
        public float AttenuationMax => _attenuationMax;

        protected void SetVariationValues()
        {
            _clipIndex = Random.Range(0, _possibleClips.Length);
            Volume = Random.Range(_volume.MinValue, _volume.MaxValue);
            Pitch = Random.Range(_pitch.MinValue, _pitch.MaxValue);
        }
    }
}

