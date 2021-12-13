using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antiphase4WS : FourWheelSteering
{

    public override void SetSteering(WheelManager rearWheels, float steering)
    {
        base.SetSteering(rearWheels, -steering);
    }
}
