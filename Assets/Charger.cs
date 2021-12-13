using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Charger : MonoBehaviour
{
    protected float boost, currentMaxBoost;
    private float vacuumRatio = 1f;
    [SerializeField]
    private float maxBoost, decelEndDistance;
    [Range(-1f, 0f)]
    [SerializeField]
    private float idleVacuum, decelVacuum;
    [Range(0f, 1f)]
    [SerializeField]
    private float decelDamp, idleDamp;
    private bool isDecel;

    public float Boost
    {
        get
        {
            return boost;
        }
    }

    protected float MaxBoost
    {
        get
        {
            return maxBoost;
        }
    }

    protected float DecelDamp
    {
        get
        {
            return decelDamp;
        }
    }

    protected bool IsDecel
    {
        get
        {
            return isDecel;
        }
    }

    public abstract void Init(float idleRpm, float revLimit);
    protected abstract void SetBoost(float accel, float rpm, float vacuum);

    public void SetCharger(float accel, float prevAccel, float rpm)
    {
        CheckDecel(accel, prevAccel);

        prevAccel = accel;
        vacuumRatio = GetVacuumRatio();

        float vacuum = Mathf.Lerp(decelVacuum, idleVacuum, vacuumRatio);

        SetBoost(accel, rpm, vacuum);
    }

    void CheckDecel(float accel, float prevAccel)
    {
        if (isDecel)
        {
            DecelEnd();

            return;
        }

        DecelStart(accel, prevAccel);
    }

    void DecelStart(float accel, float prevAccel)
    {
        if (accel == 0f && prevAccel > 0f)
            isDecel = true;
    }

    void DecelEnd()
    {
        if (vacuumRatio <= decelEndDistance)
            isDecel = false;
    }

    float GetVacuumRatio()
    {
        return isDecel ? Mathf.Lerp(vacuumRatio, 0f, decelDamp) : Mathf.Lerp(vacuumRatio, 1f, idleDamp);
    }

    public float GetBoostMultiple()
    {
        float current = 1f + boost,
            divide = 1f + currentMaxBoost;

        return current / divide;
    }
}
