using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboCharger : Charger
{
    private float maxBoostRpm, idleRpm, revLimit;
    [Range(1, 10)]
    [SerializeField]
    private int maxBoostRpmRate;
    [Range(0f, 1f)]
    [SerializeField]
    private float minMaxBoostRatio, boostDamp;

    public override void Init(float idleRpm, float revLimit)
    {
        this.idleRpm = idleRpm;
        this.revLimit = revLimit;

        maxBoostRpm = GetMaxBoostRpm();
    }

    protected override void SetBoost(float accel, float rpm, float vacuum)
    {
        if (IsDecel)
        {
            boost = Mathf.Lerp(boost, vacuum, DecelDamp);

            return;
        }

        if (accel > 0f)
        {
            BoostOn(accel, rpm, vacuum);

            return;
        }

        boost = vacuum;
    }

    void BoostOn(float accel, float rpm, float vacuum)
    {
        if (boost < vacuum)
            boost = vacuum;

        currentMaxBoost = GetCurrentMaxBoost(rpm);
        boost = GetBoost(accel);
    }

    float GetMaxBoostRpm()
    {
        float rate = maxBoostRpmRate / 10f;

        return Mathf.Lerp(idleRpm, revLimit, rate);
    }

    float GetCurrentMaxBoost(float rpm)
    {
        float minMaxBoost = MaxBoost * minMaxBoostRatio,
            maxBoostRatio = Mathf.InverseLerp(idleRpm, maxBoostRpm, rpm);

        return Mathf.Lerp(minMaxBoost, MaxBoost, maxBoostRatio);
    }

    float GetBoost(float accel)
    {
        float targetBoost = Mathf.Lerp(0f, currentMaxBoost, accel);

        return Mathf.Lerp(boost, targetBoost, boostDamp);
    }
}
