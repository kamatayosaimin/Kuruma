using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OYKT
{

    [System.Serializable]
    class DownForce
    {
        [SerializeField]
        private float frontBumperForce, sideSkirtForce, rearBumperForce, rearSpoilerForce;
        [SerializeField]
        private Rigidbody frontBumper, rearBumper, rearSpioler;
        private Rigidbody[] sideSkirt;
        [SerializeField]
        private Transform sideSkirtParent;

        public void Init()
        {
            sideSkirt = sideSkirtParent.GetComponentsInChildren<Rigidbody>();
        }

        public void Apply(float speedVelocity)
        {
            Apply(frontBumper, speedVelocity, frontBumperForce);

            foreach (var s in sideSkirt)
                Apply(s, speedVelocity, sideSkirtForce);

            Apply(rearBumper, speedVelocity, rearBumperForce);

            if (rearSpioler)
                Apply(rearSpioler, speedVelocity, rearSpoilerForce);
        }

        void Apply(Rigidbody aeroParts, float speedVelocity, float force)
        {
            Vector3 _force = GetForce(speedVelocity, force);

            aeroParts.AddRelativeForce(_force);
        }

        Vector3 GetForce(float speedVelocity, float force)
        {
            float velocity = speedVelocity * force;

            return Vector3.down * velocity;
        }
    }
}
