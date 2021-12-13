using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class OYKT : MonoBehaviour
{

    static class GOISDebug
    {

        public static void GOIS(Transform cam, Rigidbody rb, WheelManager frontWheels, WheelManager rearWheels)
        {
            GOIS(cam, frontWheels, 0.5f);
            GOIS(cam, rearWheels, 0.45f);

            Color[] c = new[] { Color.red, Color.green, Color.blue, Color.yellow };
            Vector3 av = rb.angularVelocity;

            for (int i = 0; i < c.Length; i++)
                GOIS(cam, 0f, 0.4f - 0.01f * i, i < 3 ? av[i] : av.magnitude * 0.5f, c[i]);
        }

        static void GOIS(Transform cam, WheelManager manager, float y)
        {
            WheelController[] wheels = manager.Wheels;

            for (int i = 0; i < wheels.Length; i++)
            {
                float dir = i * 2f - 1f;
                WheelController wheel = wheels[i];

                WheelHit hit = wheel.GetHit();

                float rpm = wheel.Rpm, fs = hit.forwardSlip, ss = hit.sidewaysSlip;

                GOIS(cam, dir, y, rpm, 0.0001f, GOIS(rpm));
                GOIS(cam, dir, y - 0.01f, fs, 1f, GOIS(fs));
                GOIS(cam, dir, y - 0.02f, ss, 1f, GOIS(ss));
            }
        }

        static void GOIS(Transform cam, float dir, float sy, float value, float scale, Color c)
        {
            GOIS(cam, 0.01f * dir, sy, Mathf.Abs(value) * scale * dir, c);
        }

        static void GOIS(Transform cam, float sx, float sy, float dx, Color c)
        {
            Debug.DrawRay(cam.TransformPoint(new Vector3(sx, sy, 1f)), cam.TransformDirection(Vector3.right * dx), c);
        }

        static Color GOIS(float value)
        {
            return value < 0f ? Color.red : Color.green;
        }
    }

    private int shiftCount = 1;
    private float engineRpm, clutch = 1f, prevAccel, driveBalance, wheelRate;
    [SerializeField]
    private float initialSpeed;
    [SerializeField]
    private AutomaticTransmission at;
    [SerializeField]
    private KURUMAInput kurumaInput;
    [SerializeField]
    private Mission mission;
    [SerializeField]
    private Powertrein powertrein;
    [SerializeField]
    private Tire tire;
    [SerializeField]
    private TorqueControl torqueControl;
    private Rigidbody rb;
    [SerializeField]
    private Transform centerOfMass;
    private Charger charger;
    private FourWheelDriveController fourWDController;
    private FourWheelSteering fourWS;
    private Muffler muffler;
    [SerializeField]
    private WheelManager frontWheels, rearWheels;
    [SerializeField]
    private DownForce downForce;
    [SerializeField]
    private EngineSound engineSound;
    [SerializeField]
    private EngineSpec engineSpec;

    public int ShiftCount
    {
        get
        {
            return shiftCount;
        }
    }

    int IdleRpm
    {
        get
        {
            return engineSpec.IdleRpm;
        }
    }

    int RevLimit
    {
        get
        {
            return engineSpec.RevLimit;
        }
    }

    public float EngineRpm
    {
        get
        {
            return engineRpm;
        }
    }

    float FinalGear
    {
        get
        {
            return mission.FinalGear;
        }
    }

    public float Steering
    {
        get
        {
            return powertrein.Steering;
        }
    }

    float[] GearRatio
    {
        get
        {
            return mission.GearRatio;
        }
    }

    bool IsAT
    {
        get
        {
            return at.IsAT;
        }
    }

    public EngineSpec Engine
    {
        get
        {
            return engineSpec;
        }
    }

    public KURUMAInputGetter Input
    {
        get
        {
            return kurumaInput;
        }
    }

    // Use this for initialization
    void Start()
    {
        downForce.Init();

        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = centerOfMass.localPosition;
        rb.velocity = GetInitVelocity();

        frontWheels.Init();

        rearWheels.Init();

        wheelRate = GetWheelRate(frontWheels, rearWheels);

        charger = GetComponent<Charger>();

        if (charger)
            charger.Init(IdleRpm, RevLimit);

        if (powertrein.DriveType == DriveType.FourWheelDrive)
            fourWDController = GetComponent<FourWheelDriveController>();

        fourWS = GetComponent<FourWheelSteering>();

        driveBalance = GetDriveBalance();

        muffler = GetComponentInChildren<Muffler>();
    }

    // Update is called once per frame
    void Update()
    {
        engineSound.SetSound(engineRpm, prevAccel);

        SetWheels();

        StartCoroutine(muffler.ApplyBackFire(prevAccel));
    }

    void FixedUpdate()
    {
        float speedVelocity = GetSpeedVelocity(),
            speedKmH = GetSpeedKmH(speedVelocity),
            accel = GetAccel(),
            absAccel = Mathf.Abs(accel);

        if (clutch == 1f)
            engineRpm = WheelRpmToEngineRpm();

        SetCharger(absAccel);

        if (fourWDController && !fourWDController.UserControlled)
            driveBalance = fourWDController.DriveBalance(frontWheels, rearWheels);

        SetWheelMotor(accel);
        ChangeShiftAT(speedKmH);
        Reverse();

        downForce.Apply(speedVelocity);

        prevAccel = accel;
    }

    public void SetClutch(bool onClutch)
    {
        clutch = onClutch ? 1f : 0f;
    }

    void SetCharger(float absAccel)
    {
        if (!charger)
            return;

        charger.SetCharger(absAccel, prevAccel, engineRpm);
    }

    void SetWheelMotor(float accel)
    {
        float motor = GetMotorTorqueNM(accel) / 2f;

        SetWheelMotor(frontWheels, 1f, motor);
        SetWheelMotor(rearWheels, 0f, motor);
    }

    void SetWheelMotor(WheelManager wheelManager, float balanceOrigin, float motor)
    {
        float balance = Mathf.Abs(driveBalance - balanceOrigin);

        if (balance == 0f)
            return;

        motor *= balance;

        wheelManager.SetMotor(motor);
    }

    void ChangeShiftAT(float speedKmH)
    {
        if (!IsAT)
            return;

        bool isDown = IsLowRpm(speedKmH),
            isUp = IsHighRpm(speedKmH);

        ShiftDown(isDown);
        ShiftUp(isUp);
    }

    public void SetInput(float motor, float brake, float steering)
    {
        kurumaInput.SetInput(motor, brake, steering);
    }

    public void ChangeShiftMT(bool isDown, bool isUp)
    {
        if (IsAT)
            return;

        bool _isDown = isDown && CanShiftDown(),
            _isUp = isUp && CanShiftUp();

        ShiftDown(_isDown);
        ShiftUp(_isUp);
    }

    void ShiftDown(bool change)
    {
        if (!change)
            return;

        shiftCount--;

        StartCoroutine(ApplyClutch());
    }

    void ShiftUp(bool change)
    {
        if (!change)
            return;

        shiftCount++;

        StartCoroutine(ApplyClutch());
    }

    public void Reverse()
    {
        float accel = kurumaInput.Motor;

        ReverseOn(accel);
        ReverseOff(accel);
    }

    void ReverseOn(float accel)
    {
        if (shiftCount == 0 || accel >= 0f || CantReverseSetting())
            return;

        shiftCount = 0;

        StartCoroutine(ApplyClutch());
    }

    void ReverseOff(float accel)
    {
        if (shiftCount > 0 || accel <= 0f || CantReverseSetting())
            return;

        shiftCount = 1;

        StartCoroutine(ApplyClutch());
    }

    void SetWheels()
    {
        SetSteering();

        SetBrake();

        frontWheels.SetWheels(tire);

        rearWheels.SetWheels(tire);
    }

    void SetSteering()
    {
        float steering = kurumaInput.Steering;

        frontWheels.SetSteering(steering);

        if (fourWS)
            fourWS.SetSteering(rearWheels, steering);
    }

    void SetBrake()
    {
        float input = kurumaInput.Brake;

        frontWheels.SetBrake(powertrein.BrakeFront, input, torqueControl.FrontBrakeRate);

        rearWheels.SetBrake(powertrein.BrakeRear, input, torqueControl.RearBrakeRate);
    }

    public void GOIS(Transform cam, Rigidbody rb)
    {
        GOISDebug.GOIS(cam, rb, frontWheels, rearWheels);
    }

    public float GetSpeedVelocity()
    {
        return rb.velocity.magnitude;
    }

    public float GetSpeedKmH(float speedVelocity)
    {
        return speedVelocity * 3.6f;
    }

    float GetWheelRate(params WheelManager[] managers)
    {
        float average = managers.Average(m => m.RadiusAverage);

        return Mathf.PI * average * 0.12f;
    }

    float GetDriveBalance()
    {
        return fourWDController ? fourWDController.InitDriveBalance(frontWheels, rearWheels) : GetDriveBalanceBasic();
    }

    float GetDriveBalanceBasic()
    {
        float[] barances = new[] { 0f, 1f, 0.5f };

        return barances[(int)powertrein.DriveType];
    }

    float GetAccel()
    {
        return GetClampedAccel() * clutch;
    }

    float GetClampedAccel()
    {
        float accel = kurumaInput.Motor;

        return shiftCount > 0 ? Mathf.Max(accel, 0f) : Mathf.Min(accel, 0f);
    }

    float GetGearRatio()
    {
        return GetGearRatio(shiftCount);
    }

    float GetGearRatio(int index)
    {
        float[] gearRatio = GearRatio;

        return index > 0 ? gearRatio[index - 1] : gearRatio[0];
    }

    float WheelRpmToEngineRpm()
    {
        float rpm = GetWheelRpm() * GetGearRatio() * FinalGear;

        return GetEngineRpm(rpm);
    }

    float GetWheelRpm()
    {
        float sum = frontWheels.RpmAvarage + rearWheels.RpmAvarage;

        return sum / 2f;
    }

    float SpeedToEngineRpm(float speedKmH, int index)
    {
        float rpm = speedKmH * GetGearRatio(index) * FinalGear / wheelRate;

        return GetEngineRpm(rpm);
    }

    float SpeedToEngineRpmDistance(float speedKmH, int index)
    {
        return RevLimit - SpeedToEngineRpm(speedKmH, index);
    }

    float GetEngineRpm(float rpm)
    {
        int idleRpm = IdleRpm;

        return rpm < idleRpm ? idleRpm : rpm;
    }

    float GetMotorTorqueNM(float accel)
    {
        return GetMotorTorqueKgM(accel) * 9.806652f;
    }

    float GetMotorTorqueKgM(float accel)
    {
        return GetEngineTorqueKgM(accel) * GetGearRatio() * FinalGear;
    }

    float GetEngineTorqueKgM(float accel)
    {
        int revLimit = RevLimit;

        if (engineRpm > revLimit)
            return 0f;

        return engineSpec.MaxTorqueKgM * GetTorqueRatio(revLimit) * accel * GetBoostMultiple();
    }

    float GetTorqueRatio(int revLimit)
    {
        float rpmValue = Mathf.InverseLerp(IdleRpm, revLimit, engineRpm);

        return engineSpec.TorqueRate.Evaluate(rpmValue);
    }

    float GetBoostMultiple()
    {
        return charger ? charger.GetBoostMultiple() : 1f;
    }

    float GetClutch(float targetRpm)
    {
        float currentDistance = Mathf.Abs(targetRpm - engineRpm);

        return Mathf.InverseLerp(powertrein.ClutchDistance, 0f, currentDistance);
    }

    float LerpClutchRpm(float targetRpm)
    {
        float damp = powertrein.ClutchDamp * Time.deltaTime;

        return Mathf.Lerp(engineRpm, targetRpm, damp);
    }

    bool CanShiftDown()
    {
        return shiftCount > 1;
    }

    bool CanShiftUp()
    {
        return shiftCount > 0 && shiftCount < GearRatio.Length;
    }

    bool IsLowRpm(float speedKmH)
    {
        if (!CanShiftDown())
            return false;

        float downDistance = at.Up + at.Down;

        return SpeedToEngineRpmDistance(speedKmH, shiftCount - 1) >= downDistance;
    }

    bool IsHighRpm(float speedKmH)
    {
        if (!CanShiftUp())
            return false;

        return SpeedToEngineRpmDistance(speedKmH, shiftCount) <= at.Up;
    }

    bool CantReverseSetting()
    {
        return GetSpeedKmH(GetSpeedVelocity()) >= 20f;
    }

    Vector3 GetInitVelocity()
    {
        float speed = initialSpeed / 3.6f;
        Vector3 dir = Vector3.forward * speed;

        return transform.TransformDirection(dir);
    }

    IEnumerator ApplyClutch()
    {
        clutch = 0f;

        float targetRpm = WheelRpmToEngineRpm();

        while (clutch < powertrein.ClutchOnDistance)
        {
            engineRpm = LerpClutchRpm(targetRpm);
            clutch = GetClutch(targetRpm);

            yield return new WaitForFixedUpdate();

            targetRpm = WheelRpmToEngineRpm();
        }

        engineRpm = targetRpm;
        clutch = 1f;
    }
}
