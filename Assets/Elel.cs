using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MissMonochro
{

    [Serializable]
    class BoostMeter
    {
        [SerializeField]
        private GameObject meter;
        [SerializeField]
        private Transform arrow;
        private Charger charger;

        public void Init(Charger charger)
        {
            this.charger = charger;

            meter.SetActive(charger);
        }

        public void SetBoostArrow()
        {
            if (!charger)
                return;

            arrow.eulerAngles = Vector3.back * GetAngle();
        }

        float GetAngle()
        {
            return GetBoostAngle() * 90f - 90f;
        }

        float GetBoostAngle()
        {
            float boost = charger.Boost;

            return GetNegativeBoost(boost) + GetPositiveBoost(boost);
        }

        float GetNegativeBoost(float boost)
        {
            boost /= 2f;

            return Mathf.Clamp(boost, -0.5f, 0f);
        }

        float GetPositiveBoost(float boost)
        {
            return Mathf.Clamp(boost, 0f, 2f);
        }
    }

    [Serializable]
    class TachoMeter
    {
        [Range(0, 12)]
        [SerializeField]
        private int memoryCount, redZone;
        [SerializeField]
        private float angleScale, originAngle;
        [SerializeField]
        private Transform arrow, memoryParent;

        public void SetTachoMemory()
        {
            for (int i = 0; i < memoryParent.childCount; i++)
            {
                Transform memory = memoryParent.GetChild(i);

                memory.gameObject.SetActive(i <= memoryCount);
                memory.GetComponent<Image>().color = i >= redZone ? new Color(1f, 0.25f, 0f, 1f) : Color.white;
            }
        }

        public void SetTachoArrow(float engineRpm)
        {
            arrow.eulerAngles = Vector3.back * GetAngle(engineRpm);
        }

        float GetAngle(float engineRpm)
        {
            return GetRpm(engineRpm) * angleScale - originAngle;
        }

        float GetRpm(float engineRpm)
        {
            float limit = 1000f * memoryCount;

            return Mathf.Min(engineRpm, limit);
        }
    }

    [Serializable]
    class UIManager
    {
        [SerializeField]
        private BoostMeter boostMeter;
        [SerializeField]
        private EngineSpecCurve egSpec;
        [SerializeField]
        private TachoMeter tachoMeter;
        [SerializeField]
        private TextManager textManager;

        public void Init(int shiftCount, OYKT.EngineSpec engineSpec, Charger charger)
        {
            egSpec.SetEngineGraph(engineSpec);

            tachoMeter.SetTachoMemory();

            SetShiftText(shiftCount);

            boostMeter.Init(charger);
        }

        public void Update(float engineRpm, float speedKmH, float steering, OYKT.KURUMAInputGetter kurumaInput)
        {
            boostMeter.SetBoostArrow();

            egSpec.SetRpmLine(engineRpm);

            tachoMeter.SetTachoArrow(engineRpm);

            textManager.SetTexts(speedKmH, steering, kurumaInput);
        }

        public void SetShiftText(int shiftCount)
        {
            textManager.SetShiftText(shiftCount);
        }
    }
}
