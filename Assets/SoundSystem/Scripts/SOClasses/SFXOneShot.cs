using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    [CreateAssetMenu(menuName = "SoundSystem/SFX OneShot", fileName = "SFX_OS_")]
    public class SFXOneShot : SFXEvent
    {
        public void PlayOneShot(Vector3 position)
        {
            SetVariationValues();

            if (Clip == null)
            {
                Debug.LogWarning("SFXOneShot.PlayOneShot: no clips specified");
                return;
            }

            SoundManager.Instance.PlayOneShot(this, position);
        }

        public void Preview(AudioSource source)
        {
            SetVariationValues();

            if (Clip == null) return;

            source.clip = Clip;
            source.outputAudioMixerGroup = Mixer;
            source.priority = Priority;
            source.volume = Volume;
            source.pitch = Pitch;
            source.panStereo = StereoPan;
            source.spatialBlend = SpatialBlend;

            source.Play();
        }
    }
}

