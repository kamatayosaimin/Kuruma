using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yasuken : MonoBehaviour
{

    class MaikeRyu
    {
        private RaycastHit hf, hr;
        private Vector3 of, or;
        private Transform kabe;

        public MaikeRyu(Transform kabe)
        {
            this.kabe = kabe;

            of = GetOrigin(true);
            or = GetOrigin(false);

            Ray rf = new Ray(of, kabe.TransformDirection(Vector3.back)), rr = new Ray(or, -rf.direction);

            if (!(Physics.Raycast(rf, out hf) && Physics.Raycast(rr, out hr)))
                throw new System.Exception();
        }

        public void SetTransform()
        {
            Vector3 s = kabe.localScale;

            s.z = Vector3.Distance(hf.point, hr.point);

            kabe.localScale = s;
            kabe.position = Vector3.Lerp(hf.point, hr.point, 0.5f) + kabe.TransformVector(Vector3.left * 0.5f);
        }

        public void Draw()
        {
            Draw(of, Color.red * 0.5f);
            Draw(or, Color.green * 0.5f);
            Draw(hf.point, Color.red);
            Draw(hr.point, Color.green);
        }

        void Draw(Vector3 p, Color c)
        {
            Vector3[] da = new[] { Vector3.right, Vector3.up, Vector3.forward };

            foreach (var d in da)
            {
                Vector3 _d = kabe.TransformDirection(d) * 10f;

                Debug.DrawLine(p - _d, p + _d, c);
            }
        }

        Vector3 GetOrigin(bool isFront)
        {
            float a = 1f;

            if (!isFront)
                a = -a;

            return kabe.TransformPoint(new Vector3(0.5f, 0f, 0.5f * a)) + kabe.TransformDirection(Vector3.back * (0.1f * a));
        }
    }

    private MaikeRyu[] maike;

    // Use this for initialization
    void Start()
    {
        maike = new MaikeRyu[transform.childCount];

        for (int i = 0; i < maike.Length; i++)
            maike[i] = new MaikeRyu(transform.GetChild(i));

        foreach (var m in maike)
            m.SetTransform();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var m in maike)
            m.Draw();
    }
}
