using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FourWheelDriveController : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField]
    private float userDriveBalance = 0.5f;
    [SerializeField]
    private bool userControlled;

    public bool UserControlled
    {
        get
        {
            return userControlled;
        }
    }

    public float InitDriveBalance(WheelManager frontWheels, WheelManager rearWheels)
    {
        return userControlled ? userDriveBalance : DriveBalance(frontWheels, rearWheels);
    }

    public virtual float DriveBalance(WheelManager frontWheels, WheelManager rearWheels)
    {
        return 0.5f;
    }
}
