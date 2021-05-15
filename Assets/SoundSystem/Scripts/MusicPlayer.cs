using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class MusicPlayer : MonoBehaviour
    {
        // each source will correspond to a layer, and we'll control them together
        List<AudioSource> _layerSources = new List<AudioSource>();
        // this gives us control over AudioSource volumes for better fading
        List<float> _sourceStartVolumes = new List<float>();

        Coroutine _fadeVolumeRoutine = null;
        Coroutine _stopRoutine = null;

        bool _isStopping = false;
        public bool IsStopping => _isStopping;

        MusicManager _musicManager;
        MusicEvent _musicEvent;

        private void Awake()
        {
            _musicManager = MusicManager.Instance;
            CreateLayerSources();
        }

        void CreateLayerSources()
        {
            for (int i = 0; i < MusicManager.MaxLayers; i++)
            {
                Debug.Log("Layer Index: " + i);
                _layerSources.Add(gameObject.AddComponent<AudioSource>());
                //_layerSources[i] = gameObject.AddComponent<AudioSource>();
                _layerSources[i].playOnAwake = false;
                _layerSources[i].loop = true;
            }
        }

        public void Play(MusicEvent musicEvent, float fadeTime)
        {
            if (musicEvent == null) return;

            _musicEvent = musicEvent;
            for (int i = 0; i < _layerSources.Count && i < musicEvent.MusicLayers.Length; i++)
            {
                // if the music layers are properly filled out
                if (musicEvent.MusicLayers[i] != null)
                {
                    _layerSources[i].volume = 0;
                    _layerSources[i].clip = musicEvent.MusicLayers[i];
                    _layerSources[i].outputAudioMixerGroup = musicEvent.Mixer;

                    _layerSources[i].Play();
                }  
            }
            
            FadeVolume(_musicManager.Volume, fadeTime);
        }

        public void Stop(float fadeTime)
        {
            if (_stopRoutine != null)
                StopCoroutine(_stopRoutine);
            _stopRoutine = StartCoroutine(StopRoutine(fadeTime));
        }

        public void FadeVolume(float targetVolume, float fadeTime)
        {
            if (_musicEvent == null) return;

            targetVolume = Mathf.Clamp(targetVolume, 0, 1);
            if (fadeTime < 0) fadeTime = 0;

            if (_fadeVolumeRoutine != null)
                StopCoroutine(_fadeVolumeRoutine);

            // if single layer, lerp active layer
            if (_musicEvent.LayerType == LayerType.Single)
            {
                _fadeVolumeRoutine = StartCoroutine
                    (LerpSourcesSingleRoutine(targetVolume, fadeTime));
            }
            // if additive, lerp additively
            else if (_musicEvent.LayerType == LayerType.Additive)
            {
                _fadeVolumeRoutine = StartCoroutine
                    (LerpSourceAdditiveRoutine(targetVolume, fadeTime));
            }

        }

        private IEnumerator StopRoutine(float fadeTime)
        {
            _isStopping = true;
            // cancel current running volume fades
            if (_fadeVolumeRoutine != null)
                StopCoroutine(_fadeVolumeRoutine);

            // start the fadeout
            // when done fading out, disable all sources
            // if additive, lerp additively
            // otherwise lerp up only the active layer
            if (_musicEvent.LayerType == LayerType.Single)
            {
                _fadeVolumeRoutine = StartCoroutine
                    (LerpSourcesSingleRoutine(0, fadeTime));
            }

            else if (_musicEvent.LayerType == LayerType.Additive)
            {
                _fadeVolumeRoutine = StartCoroutine
                    (LerpSourceAdditiveRoutine(0, fadeTime));
            }
            else
            {
                Debug.LogWarning("Layer type not implemented yet!");
            }

            // wait for volume fade to finish
            yield return _fadeVolumeRoutine;

            foreach(AudioSource source in _layerSources)
            {
                source.Stop();
            }

            _isStopping = false;
        }

        private void SaveSourceStartVolumes()
        {
            // store source start volumes independently for individual ASource lerping
            _sourceStartVolumes.Clear();
            for (int i = 0; i < _layerSources.Count; i++)
            {
                _sourceStartVolumes.Add(_layerSources[i].volume);
            }
        }

        // go through sources and fade in all layers up to active layer, fade down the rest
        private IEnumerator LerpSourceAdditiveRoutine(float targetVolume, float fadeTime)
        {
            SaveSourceStartVolumes();

            float newVolume = 0;
            float startVolume = 0;

            for (float elapsedTime = 0; elapsedTime <= fadeTime; elapsedTime += Time.deltaTime)
            {
                // go through all layers
                for (int i = 0; i < _layerSources.Count; i++)
                {
                    // fade layers up until active layer
                    if (i <= _musicManager.ActiveLayerIndex)
                    {
                        // fade the volume
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                        Debug.Log("AudioSource: " + i + " | volume: " + newVolume);
                    }
                    // otherwise fade it to 0 from its current position
                    else
                    {
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                        Debug.Log("AudioSource: " + i + " | volume: " + newVolume);
                    }
                }

                yield return null;
            }
            // set final target just to make sure we hit the exact value
            //TODO - should this be _layers.Count? instead of ActiveLayerIndex
            for (int i = 0; i <= _musicManager.ActiveLayerIndex; i++)
            {
                if (i <= _musicManager.ActiveLayerIndex)
                {
                    _layerSources[i].volume = targetVolume;
                    Debug.Log("AudioSource: " + i + " | volume: " + _layerSources[i].volume);
                }
                // otherwise set it to 0
                else
                {
                    _layerSources[i].volume = 0;
                    Debug.Log("AudioSource: " + i + " | volume: " + _layerSources[i].volume);
                }
            }
        }

        // go through all the sources and fade on the active layer, fade down the rest
        private IEnumerator LerpSourcesSingleRoutine(float targetVolume, float fadeTime)
        {
            SaveSourceStartVolumes();

            float newVolume = 0;
            float startVolume = 0;

            for (float elapsedTime = 0; elapsedTime <= fadeTime; elapsedTime += Time.deltaTime)
            {
                for (int i = 0; i < _layerSources.Count; i++)
                {
                    if (i == _musicManager.ActiveLayerIndex)
                    {
                        // fade up
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                        Debug.Log("AudioSource: " + i + " | volume: " + newVolume);
                    }
                    else
                    {
                        // fade down
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                        Debug.Log("AudioSource: " + i + " | volume: " + newVolume);
                    }
                }

                yield return null;
            }

            // set final target just to make sure we hit the exact value
            for (int i = 0; i <= _musicManager.ActiveLayerIndex; i++)
            {
                if (i == _musicManager.ActiveLayerIndex)
                {
                    _layerSources[i].volume = targetVolume;
                    Debug.Log("AudioSource: " + i + " | volume: " + _layerSources[i].volume);
                }
                else
                {
                    _layerSources[i].volume = 0;
                    Debug.Log("AudioSource: " + i + " | volume: " + _layerSources[i].volume);
                }
            }
        }
    }
}

