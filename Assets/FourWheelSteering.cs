using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class FourWheelSteering : MonoBehaviour
{

    public virtual void SetSteering(WheelManager rearWheels, float steering)
    {
        rearWheels.SetSteering(steering);
    }
}
