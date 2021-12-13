using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    private AudioSource squeal;
    [SerializeField]
    private ParticleSystem primarySmoke, trailSmoke;
    private WheelCollider wheel;

    public float Radius
    {
        get
        {
            return wheel.radius;
        }
    }

    public float Rpm
    {
        get
        {
            return Mathf.Abs(wheel.rpm);
        }
    }

    // Use this for initialization
    void Start()
    {
        squeal = GetComponent<AudioSource>();
        primarySmoke = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init()
    {
        wheel = GetComponent<WheelCollider>();
    }

    public void SetMotor(float motor)
    {
        wheel.motorTorque = motor;
    }

    public void SetSteering(float steering)
    {
        wheel.steerAngle = steering;
    }

    public void SetBrake(float brake, float input, float controlRate)
    {
        wheel.brakeTorque = GetBrake(brake, input, controlRate);
    }

    public void SetWheel(OYKT.Tire tire)
    {
        SetMesh();
        SetSqueal(tire);
    }

    void SetMesh()
    {
        Quaternion r;
        Vector3 p;
        Transform wt = transform.GetChild(0);

        wheel.GetWorldPose(out p, out r);

        wt.rotation = r;
        wt.position = p;
    }

    void SetSqueal(OYKT.Tire tire)
    {
        float volume = squeal.volume;

        SetSquealSound(ref volume, tire.SidewaysSlipRate, tire.SquealPitchMultipler, tire.SquealDamp);
        SetSquealSmoke(volume, tire.SmokeEmissionRate, tire.TrailSmokeTarget);
    }

    void SetSquealSound(ref float volume, float sidewaysRate, float pitchMultipler, float damp)
    {
        float level = GetSlipLevel(sidewaysRate),
            limit = 1f;

        squeal.pitch = GetSquealPitch(pitchMultipler, level, limit);
        squeal.volume = volume = GetSquealVolume(volume, damp, level, limit);
    }

    void SetSquealSmoke(float squealVolume, float emissionRate, float trailTarget)
    {
        SetSquealPrimarySmoke(squealVolume, emissionRate);
        SetSquealTrailSmoke(squealVolume, trailTarget);
    }

    void SetSquealPrimarySmoke(float squealVolume, float emissionRate)
    {
        ParticleSystem.EmissionModule emission = primarySmoke.emission;
        ParticleSystem.MinMaxCurve rate = emission.rateOverTime;

        rate.constantMax = emissionRate * squealVolume;

        emission.rateOverTime = rate;
    }

    void SetSquealTrailSmoke(float squealVolume, float trailTarget)
    {
        bool enabled = squealVolume >= trailTarget;

        if (trailSmoke.isPlaying)
        {
            SquealTrailSmokeOff(enabled);

            return;
        }

        SquealTrailSmokeOn(enabled);
    }

    void SquealTrailSmokeOff(bool enabled)
    {
        if (!enabled)
            trailSmoke.Stop();
    }

    void SquealTrailSmokeOn(bool enabled)
    {
        if (enabled)
            trailSmoke.Play();
    }

    float GetBrake(float brake, float input, float controlRate)
    {
        float _brake = brake * input,
            torqueControl = GetTorqueControlledBrake(brake, controlRate);

        return Mathf.Max(_brake, torqueControl);
    }

    float GetTorqueControlledBrake(float brake, float controlRate)
    {
        WheelHit hit = GetHit();

        float slip = Mathf.Abs(hit.forwardSlip);

        return slip >= wheel.forwardFriction.extremumSlip ? brake * controlRate : 0f;
    }

    float GetSlipLevel(float sidewaysRate)
    {
        Vector2 slipDir = GetSlipDirection(sidewaysRate);

        return slipDir.magnitude;
    }

    float SidewaysSlipLimit(float slipRate)
    {
        WheelFrictionCurve grip = wheel.sidewaysFriction;

        return Mathf.Lerp(grip.extremumSlip, grip.asymptoteSlip, slipRate);
    }

    float GetSquealPitch(float pitchMultipler, float level, float limit)
    {
        return limit + (level - limit) * pitchMultipler;
    }

    float GetSquealVolume(float volume, float damp, float level, float limit)
    {
        bool isSlip = level >= limit;

        return isSlip ? 1f : GetSquealDampedVolume(volume, damp);
    }

    float GetSquealDampedVolume(float volume, float damp)
    {
        damp *= Time.deltaTime;

        return Mathf.Lerp(volume, 0f, damp);
    }

    Vector2 GetSlipDirection(float sidewaysRate)
    {
        WheelHit hit = GetHit();

        return GetSlipDirection(hit, sidewaysRate);
    }

    Vector2 GetSlipDirection(WheelHit hit, float sidewaysRate)
    {
        float forward = hit.forwardSlip / wheel.forwardFriction.extremumSlip,
            sideways = hit.sidewaysSlip / SidewaysSlipLimit(sidewaysRate);

        return new Vector2(forward, sideways);
    }

    public WheelHit GetHit()
    {
        WheelHit hit;

        wheel.GetGroundHit(out hit);

        return hit;
    }
}
