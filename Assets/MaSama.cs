using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OYKT
{

    [System.Serializable]
    class EngineSound
    {
        [SerializeField]
        private float pitchMultipler, highPitchMultipler, volumeMinDistance, volumeMaxDistance;
        [SerializeField]
        private AudioSource accelLow, accelHigh, decelLow, decelHigh;

        public void SetSound(float engineRpm, float accel)
        {
            float lowPitch = engineRpm * pitchMultipler,
                highPitch = lowPitch * highPitchMultipler,
                highVolume = Mathf.InverseLerp(volumeMinDistance, volumeMaxDistance, engineRpm),
                lowVolume = 1f - highVolume,
                decelVolume = 1f - Mathf.Abs(accel);

            SetSound(accelLow, decelLow, lowPitch, lowVolume, decelVolume);
            SetSound(accelHigh, decelHigh, highPitch, highVolume, decelVolume);
        }

        void SetSound(AudioSource accelSound, AudioSource decelSound, float pitch, float volume, float decelVolume)
        {
            accelSound.pitch = decelSound.pitch = pitch;

            accelSound.volume = volume;

            decelSound.volume = volume * decelVolume;
        }
    }
}
