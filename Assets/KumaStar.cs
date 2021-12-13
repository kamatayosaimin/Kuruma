using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MissMonochro
{

    struct EngineSpecSection
    {
        private float torque, rpm;

        public float Power
        {
            get
            {
                return torque * rpm * 0.001396f;
            }
        }

        public float Torque
        {
            get
            {
                return torque;
            }
        }

        public float Rpm
        {
            get
            {
                return rpm;
            }
        }

        public EngineSpecSection(float torque, float rpm)
        {
            this.torque = torque;
            this.rpm = rpm;
        }

        public string GetPowerString()
        {
            return GetPowerString(Power);
        }

        public static string GetPowerString(float power)
        {
            return power.ToString("0") + " PS";
        }

        public string GetTorqueString()
        {
            return GetTorqueString(torque);
        }

        public static string GetTorqueString(float torque)
        {
            return torque.ToString("0.0") + " kg m";
        }

        public string GetRpmString()
        {
            return GetRpmString(rpm);
        }

        public static string GetRpmString(float rpm)
        {
            return rpm.ToString("0") + " rpm";
        }

        public string GetPowerRpmString()
        {
            return GetPowerRpmString(Power, rpm);
        }

        public static string GetPowerRpmString(float power, float rpm)
        {
            return GetPowerString(power) + " / " + GetRpmString(rpm);
        }

        public string GetTorqueRpmString()
        {
            return GetTorqueRpmString(torque, rpm);
        }

        public static string GetTorqueRpmString(float torque, float rpm)
        {
            return GetTorqueString(torque) + " / " + GetRpmString(rpm);
        }
    }

    [System.Serializable]
    class EngineSpecCurve
    {
        [SerializeField]
        private float rpmScale, powerScale, torqueScale;
        [SerializeField]
        private GameObject powerLineParent, torqueLineParent;
        [SerializeField]
        private RectTransform graphParent, rpmLine;
        [SerializeField]
        private Text powerText, torqueText;

        Vector2 ScaleSize
        {
            get
            {
                return graphParent.sizeDelta;
            }
        }

        public void SetEngineGraph(OYKT.EngineSpec engineSpec)
        {
            EngineSpecSection[] sections = EngineSpecSectionHelper.GetSections(engineSpec);

            SetPowerText(sections);
            SetTorqueText(sections, engineSpec.MaxTorqueKgM);
            SetPowerGraph(sections);
            SetTorqueGraph(sections);
        }

        void SetPowerText(EngineSpecSection[] sections)
        {
            EngineSpecSection maxPower = EngineSpecSectionHelper.GetMaxPower(sections);

            powerText.text = maxPower.GetPowerRpmString();
        }

        void SetTorqueText(EngineSpecSection[] sections, float maxTorqueKgM)
        {
            float maxRpm = EngineSpecSectionHelper.GetMaxTorqueRpm(sections);

            torqueText.text = EngineSpecSection.GetTorqueRpmString(maxTorqueKgM, maxRpm);
        }

        void SetPowerGraph(EngineSpecSection[] sections)
        {
            Vector3 currentPosition = GetPowerPosition(sections[0]);
            Image[] lines = powerLineParent.GetComponentsInChildren<Image>();

            for (int i = 0; i < lines.Length; i++)
                SetPowerGraph(lines, sections, i, ref currentPosition);
        }

        void SetPowerGraph(Image[] lines, EngineSpecSection[] sections, int index, ref Vector3 currentPosition)
        {
            Image l = lines[index];

            if (index >= sections.Length - 1)
            {
                HideLine(l);

                return;
            }

            EngineSpecSection nextSection = sections[index + 1];

            if (nextSection.Rpm > rpmScale)
            {
                HideLine(l);

                return;
            }

            Vector3 nextPosition = GetPowerPosition(nextSection);

            SetGraph(l, ref currentPosition, nextPosition);
        }

        void SetTorqueGraph(EngineSpecSection[] sections)
        {
            Vector3 currentPosition = GetTorquePosition(sections[0]);
            Image[] lines = torqueLineParent.GetComponentsInChildren<Image>();

            for (int i = 0; i < lines.Length; i++)
                SetTorqueGraph(lines, sections, i, ref currentPosition);
        }

        void SetTorqueGraph(Image[] lines, EngineSpecSection[] sections, int index, ref Vector3 currentPosition)
        {
            Image l = lines[index];

            if (index >= sections.Length - 1)
            {
                HideLine(l);

                return;
            }

            EngineSpecSection nextSection = sections[index + 1];

            if (nextSection.Rpm > rpmScale)
            {
                HideLine(l);

                return;
            }

            Vector3 nextPosition = GetTorquePosition(nextSection);

            SetGraph(l, ref currentPosition, nextPosition);
        }

        void SetGraph(Image line, ref Vector3 currentPosition, Vector3 nextPosition)
        {
            RectTransform rectTransform = line.rectTransform;

            rectTransform.anchoredPosition3D = currentPosition;

            SetGraphRotate(rectTransform, currentPosition, nextPosition);
            SetGraphSize(rectTransform, currentPosition, nextPosition);

            currentPosition = nextPosition;
        }

        void SetGraphRotate(RectTransform lineRectTransform, Vector3 currentPosition, Vector3 nextPosition)
        {
            Vector3 direction = nextPosition - currentPosition;

            Quaternion rotate = Quaternion.LookRotation(Vector3.forward, direction);

            rotate.eulerAngles += Vector3.forward * 90f;

            lineRectTransform.localRotation = rotate;
        }

        void SetGraphSize(RectTransform lineRectTransform, Vector3 currentPosition, Vector3 nextPosition)
        {
            Vector2 size = lineRectTransform.sizeDelta;

            size.x = Vector3.Distance(currentPosition, nextPosition);

            lineRectTransform.sizeDelta = size;
        }

        void HideLine(Image line)
        {
            line.gameObject.SetActive(false);
        }

        public void SetRpmLine(float rpm)
        {
            Vector3 position = rpmLine.anchoredPosition3D;

            position.x = GetRpmValue(rpm);

            rpmLine.anchoredPosition3D = position;
        }

        float GetRpmValue(float rpm)
        {
            float scale = ScaleX(rpmScale);

            return rpm * scale;
        }

        float GetPowerValue(float power)
        {
            float scale = ScaleY(powerScale);

            return power * scale;
        }

        float GetTorqueValue(float torque)
        {
            float scale = ScaleY(torqueScale);

            return torque * scale;
        }

        float Scale(float size, float scale)
        {
            return size / scale;
        }

        float ScaleX(float scale)
        {
            return Scale(ScaleSize.x, scale);
        }

        float ScaleY(float scale)
        {
            return Scale(ScaleSize.y, scale);
        }

        Vector3 GetPosition(float rpm, float y)
        {
            float x = GetRpmValue(rpm);

            return new Vector3(x, y, 0f);
        }

        Vector3 GetPowerPosition(EngineSpecSection section)
        {
            float y = GetPowerValue(section.Power);

            return GetPosition(section.Rpm, y);
        }

        Vector3 GetTorquePosition(EngineSpecSection section)
        {
            float y = GetTorqueValue(section.Torque);

            return GetPosition(section.Rpm, y);
        }
    }

    static class EngineSpecSectionHelper
    {

        static float GetTorque(float maxTorqueKgM, AnimationCurve torqueRate, float t)
        {
            float ratio = torqueRate.Evaluate(t);

            return maxTorqueKgM * ratio;
        }

        public static float GetMaxTorqueRpm(EngineSpecSection[] sections)
        {
            EngineSpecSection max = sections[0];

            for (int i = 1; i < sections.Length; i++)
            {
                EngineSpecSection current = sections[i];

                if (current.Torque > max.Torque)
                    max = current;
            }

            return max.Rpm;
        }

        static float[] GetRpmSections(int idleRpm, int revLimit)
        {
            List<int> rpmList = new List<int>();

            rpmList.Add(idleRpm);

            for (int i = 0; i < revLimit; i += 100)
                if (i > idleRpm)
                    rpmList.Add(i);

            rpmList.Add(revLimit);

            float[] sections = new float[rpmList.Count];

            for (int i = 0; i < sections.Length; i++)
                sections[i] = Mathf.InverseLerp(idleRpm, revLimit, rpmList[i]);

            return sections;
        }

        static EngineSpecSection GetSection(OYKT.EngineSpec engineSpec, float t)
        {
            float torque = GetTorque(engineSpec.MaxTorqueKgM, engineSpec.TorqueRate, t),
                rpm = Mathf.Lerp(engineSpec.IdleRpm, engineSpec.RevLimit, t);

            return new EngineSpecSection(torque, rpm);
        }

        public static EngineSpecSection[] GetSections(OYKT.EngineSpec engineSpec)
        {
            float[] rpmSections = GetRpmSections(engineSpec.IdleRpm, engineSpec.RevLimit);
            EngineSpecSection[] sections = new EngineSpecSection[rpmSections.Length];

            for (int i = 0; i < sections.Length; i++)
                sections[i] = GetSection(engineSpec, rpmSections[i]);

            return sections;
        }

        public static EngineSpecSection GetMaxPower(EngineSpecSection[] sections)
        {
            EngineSpecSection max = sections[0];

            for (int i = 1; i < sections.Length; i++)
            {
                EngineSpecSection current = sections[i];

                if (current.Power > max.Power)
                    max = current;
            }

            return max;
        }
    }
}
