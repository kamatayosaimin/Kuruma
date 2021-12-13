using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangeCurveAttribute))]
public class RangeCurveDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.AnimationCurve)
        {
            EditorGUI.PropertyField(position, property, label);

            return;
        }

        RangeCurveAttribute range = (RangeCurveAttribute)attribute;

        EditorGUI.CurveField(position, property, Color.green, range.ranges, label);
    }
}
