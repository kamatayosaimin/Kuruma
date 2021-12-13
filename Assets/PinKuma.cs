using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MissMonochro
{

    [Serializable]
    class BehindView : ViewBase
    {
        [SerializeField]
        private float distanceLimit, damp;
        private Vector3 position;
        [SerializeField]
        private Transform targetLook;

        protected override void SetCamera(Transform cam)
        {
            cam.position = position;

            cam.LookAt(targetLook, targetPoint.up);
        }

        Vector3 GetPosition()
        {
            float _damp = damp * Time.deltaTime;
            Vector3 target = targetPoint.position,
                _position = Vector3.Lerp(position, target, _damp),
                offset = _position - target;

            offset = Vector3.ClampMagnitude(offset, distanceLimit);

            return target + offset;
        }

        public IEnumerator PositionState()
        {
            position = targetPoint.position;

            while (true)
            {
                position = GetPosition();

                yield return new WaitForFixedUpdate();
            }
        }
    }

    [Serializable]
    class DriversView : ViewBase
    {

        protected override void SetCamera(Transform cam)
        {
            cam.rotation = targetPoint.rotation;
            cam.position = targetPoint.position;
        }
    }

    abstract class ViewBase
    {
        [SerializeField]
        protected Transform targetPoint;

        protected abstract void SetCamera(Transform cam);

        public void SetCamera()
        {
            SetCamera(Camera.main.transform);
        }
    }
}
