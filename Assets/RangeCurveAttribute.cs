using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class RangeCurveAttribute : PropertyAttribute
{
    public readonly Rect ranges;

    public RangeCurveAttribute(float x, float y, float width, float height)
    {
        ranges = new Rect(x, y, width, height);
    }
}
