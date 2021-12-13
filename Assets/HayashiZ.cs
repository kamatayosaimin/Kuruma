using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MissMonochro
{

    [Serializable]
    class Pedal : SliderText
    {

        protected override string Style
        {
            get
            {
                return "0.000";
            }
        }

        public void SetValue(float value)
        {
            float _value = Mathf.Abs(value);

            SetSlider(_value);
            SetText(value);
        }
    }

    abstract class SliderText
    {
        [SerializeField]
        protected Slider slider;
        [SerializeField]
        protected Text text;

        protected abstract string Style
        {
            get;
        }

        protected void SetRange(float value, float min, float max)
        {
            float rangedValue = Mathf.InverseLerp(min, max, value);

            SetSlider(rangedValue);
        }

        protected void SetSlider(float value)
        {
            slider.value = value;
        }

        protected void SetText(float value)
        {
            GOISTextSetter.SetFloat(text, value, Style);
        }
    }

    [Serializable]
    class SpeedMeter : SliderText
    {
        [SerializeField]
        private float max;
        [SerializeField]
        private Gradient gradient;

        float Min
        {
            get
            {
                return 0f;
            }
        }

        protected override string Style
        {
            get
            {
                return "000";
            }
        }

        public void SetValue(float speedKmH)
        {
            SetRange(speedKmH, Min, max);
            SetColor(speedKmH);
            SetText(speedKmH);
        }

        void SetColor(float speedKmH)
        {
            Image fill = slider.fillRect.GetComponent<Image>();

            fill.color = GetColor(fill.color, speedKmH);
        }

        Color GetColor(Color color, float speedKmH)
        {
            float t = Mathf.InverseLerp(Min, max, speedKmH);
            Color _color = gradient.Evaluate(t);

            _color.a = color.a;

            return _color;
        }

        float GetR(float speedKmH)
        {
            float distance = Mathf.Abs(speedKmH - 300f);

            return Mathf.InverseLerp(0f, 100f, distance);
        }
    }

    [Serializable]
    class Steering : SliderText
    {

        protected override string Style
        {
            get
            {
                return "00.00";
            }
        }

        public void SetValue(float value, float range)
        {
            SetRange(value, -range, range);
            SetText(value);
        }
    }

    [Serializable]
    class TextManager
    {
        [SerializeField]
        private Text shiftText;
        [SerializeField]
        private Pedal brake, motor;
        [SerializeField]
        private SpeedMeter speedMeter;
        [SerializeField]
        private Steering steering;

        public void SetShiftText(int shiftCount)
        {
            string s = shiftCount > 0 ? shiftCount.ToString() : "R";

            shiftText.text = s;
        }

        public void SetTexts(float speedKmH, float steeringAngle, OYKT.KURUMAInputGetter kurumaInput)
        {
            brake.SetValue(kurumaInput.Brake);

            motor.SetValue(kurumaInput.Motor);

            speedMeter.SetValue(speedKmH);

            steering.SetValue(kurumaInput.Steering, steeringAngle);
        }
    }
}
