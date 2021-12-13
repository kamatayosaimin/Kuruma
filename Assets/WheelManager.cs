using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    private WheelController[] wheels;

    public float RadiusAverage
    {
        get
        {
            float sum = 0f;

            foreach (var w in wheels)
                sum += w.Radius;

            return sum / wheels.Length;
        }
    }

    public float RpmAvarage
    {
        get
        {
            float sum = 0f;

            foreach (var w in wheels)
                sum += w.Rpm;

            return sum / wheels.Length;
        }
    }

    public WheelController[] Wheels
    {
        get
        {
            return wheels;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init()
    {
        wheels = GetComponentsInChildren<WheelController>();

        foreach (var w in wheels)
            w.Init();
    }

    public void SetMotor(float motor)
    {
        foreach (var w in wheels)
            w.SetMotor(motor);
    }

    public void SetSteering(float steering)
    {
        foreach (var w in wheels)
            w.SetSteering(steering);
    }

    public void SetBrake(float brake, float input, float controlRate)
    {
        foreach (var w in wheels)
            w.SetBrake(brake, input, controlRate);
    }

    public void SetWheels(OYKT.Tire tire)
    {
        foreach (var w in wheels)
            w.SetWheel(tire);
    }
}
