using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muffler : MonoBehaviour
{
    private float prevAccel;
    [Range(0f, 8f)]
    public float lightIntensity;
    private Light backFireLight;
    private ParticleSystem backFireParticle;

    // Use this for initialization
    void Start()
    {
        backFireLight = GetComponentInChildren<Light>();
        backFireParticle = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    bool IsTrigger(float accel)
    {
        return !backFireParticle.isPlaying && IsAccelOff(accel);
    }

    bool IsAccelOff(float accel)
    {
        return accel < prevAccel;
    }

    public IEnumerator ApplyBackFire(float accel)
    {
        accel = Mathf.Abs(accel);

        bool isTrigger = IsTrigger(accel);

        prevAccel = accel;

        if (!isTrigger)
            yield break;

        backFireParticle.Play();

        while (backFireParticle.isPlaying)
        {
            backFireLight.intensity = backFireParticle.particleCount * lightIntensity;

            yield return null;
        }

        backFireLight.intensity = 0f;
    }
}
