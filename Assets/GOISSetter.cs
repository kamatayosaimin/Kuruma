using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class GOISTextSetter
{

    public static void SetText(Text text, string value)
    {
        text.text = value;
    }

    public static void SetInt(Text text, int value)
    {
        SetText(text, value.ToString());
    }

    public static void SetInt(Text text, int value, string style)
    {
        SetText(text, value.ToString(style));
    }

    public static void SetFloat(Text text, float value)
    {
        SetText(text, value.ToString());
    }

    public static void SetFloat(Text text, float value, string style)
    {
        SetText(text, value.ToString(style));
    }
}
