using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MissMonochro : MonoBehaviour
{
    private int shiftCount;
    private bool isPlaing, isDriversView;
    [SerializeField]
    private GameObject tama, backMirror, gois;
    private OYKT oykt;
    [SerializeField]
    private BehindView behindView;
    [SerializeField]
    private DriversView driversView;
    [SerializeField]
    private UIManager ui;

    int ShiftCount
    {
        get
        {
            return oykt.ShiftCount;
        }
    }

    // Use this for initialization
    void Start()
    {
        isPlaing = true;
        oykt = GetComponent<OYKT>();

        shiftCount = ShiftCount;

        StartCoroutine(behindView.PositionState());

        ui.Init(shiftCount, oykt.Engine, GetComponent<Charger>());

        Physics.queriesHitTriggers = false;
    }

    // Update is called once per frame
    void Update()
    {
        float steering = oykt.Steering;

        SetInput(steering);

        StartCoroutine(TamaFire());

        ChangeView();

        if (isPlaing)
            ChangeShift();

        ui.Update(oykt.EngineRpm, GetSpeedKmH(), steering, oykt.Input);
    }

    void LateUpdate()
    {
        CurrentView().SetCamera();

        Rigidbody rb = GetComponent<Rigidbody>();
        Transform cam = Camera.main.transform;

        oykt.GOIS(cam, rb);

        GOIS();
    }

    void GOIS()
    {
        foreach (var g in gois.GetComponentsInChildren<Camera>())
            g.transform.LookAt(transform);
    }

    void SetInput(float steering)
    {
        float inputMotor = Input.GetAxis("Motor"),
            inputBrake = Input.GetAxis("Brake"),
            _steering = steering * Input.GetAxis("Steering");

        oykt.SetInput(inputMotor, inputBrake, _steering);
    }

    void SetShiftCount(int current)
    {
        shiftCount = current;
    }

    void ChangeShift()
    {
        bool isDown = GOISInput.GetInputDown("X", KeyCode.Comma),
            isUp = GOISInput.GetInputDown("A", KeyCode.Period);

        oykt.ChangeShiftMT(isDown, isUp);

        int currentShiftCount = ShiftCount;

        if (currentShiftCount == shiftCount)
            return;

        ui.SetShiftText(currentShiftCount);

        shiftCount = currentShiftCount;
    }

    void ChangeView()
    {
        if (!GOISInput.GetInputDown("LB", KeyCode.Return))
            return;

        isDriversView = !isDriversView;

        backMirror.SetActive(isDriversView);
    }

    float GetSpeedKmH()
    {
        float velocity = oykt.GetSpeedVelocity();

        return oykt.GetSpeedKmH(velocity);
    }

    ViewBase CurrentView()
    {
        return isDriversView ? (ViewBase)driversView : behindView;
    }

    IEnumerator TamaFire()
    {
        if (!Input.GetButtonDown("RB"))
            yield break;

        float pow = 10f;

        while (!Input.GetButtonUp("RB"))
        {
            yield return null;

            pow += 10f * Time.deltaTime;
        }

        Vector3 d = transform.eulerAngles + Vector3.left * 30f;
        GameObject t = Instantiate(tama, transform.TransformPoint(new Vector3(0f, 1f, 3f)), Quaternion.identity);

        t.GetComponent<Rigidbody>().velocity = Quaternion.Euler(d) * Vector3.forward * pow;
    }
}
