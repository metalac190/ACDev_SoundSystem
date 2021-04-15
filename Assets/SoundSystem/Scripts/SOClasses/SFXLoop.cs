using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    [CreateAssetMenu(menuName = "SoundSystem/SFX Looped", fileName = "SFX_LP_")]
    public class SFXLoop : SFXEvent
    {
        public void Play(AudioSource audioSource)
        {
            SetVariationValues();

            if (Clip == null)
            {
                Debug.LogWarning("SFXLoop.Play: no clips specified");
                return;
            }

            audioSource.clip = Clip;
            audioSource.outputAudioMixerGroup = Mixer;
            audioSource.priority = Priority;
            audioSource.volume = Volume;
            audioSource.pitch = Pitch;
            audioSource.panStereo = StereoPan;
            audioSource.spatialBlend = SpatialBlend;

            audioSource.Play();
        }

        public void Stop(AudioSource audioSource)
        {
            audioSource.Stop();
        }
    }
}

