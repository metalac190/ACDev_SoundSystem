using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    /// <summary>
    /// This class is a Singleton that helps maintain consistency across MusicPlayers in the scene.
    /// The main approach is to control 2 'MusicPlayers' that can activate/deactivate and blend with
    /// each other. New 'track layers' can be faded up to create additive music tracks that emulate
    /// stems. This can be useful for building new musical instrument layers based on game events
    /// to increase or decrease intensity.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        #region Singleton
        private static bool _shuttingDown = false;
        private static object _lock = new object();

        private static MusicManager _instance;
        public static MusicManager Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    return null;
                }
                lock (_lock)
                {
                    if(_instance == null)
                    {
                        _instance = FindObjectOfType<MusicManager>();
                        // create it if it's not in the scene
                        if (_instance == null)
                        {
                            GameObject singletonGO = new GameObject();
                            _instance = singletonGO.AddComponent<MusicManager>();
                            singletonGO.name = "MusicManager (singleton)";

                            DontDestroyOnLoad(singletonGO);
                        }

                        
                    }
                    return _instance;
                }
            }
        }

        void Awake()
        {
            // specifically ensure we destroy excess MusicManagers,
            // if we already have one
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            SetupMusicPlayers();
        }
        #endregion

        public const int MaxLayers = 3;

        // use 2 music sources so that we can do cross blending
        MusicPlayer _musicPlayer1 = null;
        MusicPlayer _musicPlayer2 = null;
        MusicEvent _activeSong;

        private bool _music1SourcePlaying = false;
        private int _activeLayerIndex;     // used to maintain a 'level' for the musicplayer intensity
        private float _volume = .8f;

        public int ActiveLayerIndex => _activeLayerIndex;

        public float Volume
        {
            get => _volume;
            private set
            {
                value = Mathf.Clamp(value, 0, 1);
                _volume = value;
            }
        }
        public MusicPlayer ActivePlayer => (_music1SourcePlaying) ? _musicPlayer1 : _musicPlayer2;
        public MusicPlayer InactivePlayer => (_music1SourcePlaying) ? _musicPlayer2 : _musicPlayer1;

        #region PUBLIC METHODS
        public void SetVolume(float newVolume, float fadeTime)
        {
            Volume = newVolume;
            // fade MusicPlayer volumes to match with new change
            ActivePlayer.FadeVolume(Volume, fadeTime);
        }

        public void SetLayerLevel(int newLayerIndex, float fadeTime)
        {
            newLayerIndex = Mathf.Clamp(newLayerIndex, 0, MaxLayers-1);
            // if the layer changed, don't do anything different
            if (newLayerIndex == _activeLayerIndex)
            {
                Debug.Log("Layer didn't change");
                return;
            }

            _activeLayerIndex = newLayerIndex;
            SetVolume(Volume, fadeTime);
        }

        public void IncreaseLayerLevel(float fadeTime)
        {
            int newLayerIndex = _activeLayerIndex + 1;
            newLayerIndex = Mathf.Clamp(newLayerIndex, 0, MaxLayers - 1);

            // if the layer changed, don't do anything different
            if (newLayerIndex == _activeLayerIndex)
                return;

            _activeLayerIndex = newLayerIndex;
            ActivePlayer.FadeVolume(Volume, fadeTime);
        }

        public void DecreaseLayerLevel(float fadeTime)
        {
            int newLayerIndex = _activeLayerIndex - 1;
            newLayerIndex = Mathf.Clamp(newLayerIndex, 0, MaxLayers - 1);

            // if the layer changed, don't do anything different
            if (newLayerIndex == _activeLayerIndex)
            {
                return;
            }
                
            _activeLayerIndex = newLayerIndex;
            ActivePlayer.FadeVolume(Volume, fadeTime);
        }

        public void PlayMusic(MusicEvent musicEvent, float fadeTime)
        {
            // if it's the same song, no need to restart
            if (_activeSong == musicEvent)
                return;
            // if there's already a song, stop it
            if (_activeSong != null)
                ActivePlayer.Stop(fadeTime);

            // otherwise, play a new song
            _activeSong = musicEvent;
            _music1SourcePlaying = !_music1SourcePlaying;

            ActivePlayer.Play(musicEvent, fadeTime);
        }

        public void StopMusic(float fadeTime)
        {
            // if there's no song, there's nothing to stop
            if (_activeSong == null)
                return;

            _activeSong = null;
            ActivePlayer.Stop(fadeTime);
        }
        #endregion

        void SetupMusicPlayers()
        {
            _musicPlayer1 = gameObject.AddComponent<MusicPlayer>();
            _musicPlayer2 = gameObject.AddComponent<MusicPlayer>();

            _musicPlayer1.FadeVolume(0, 0);
            _musicPlayer2.FadeVolume(0, 0);
        }
    }
}


