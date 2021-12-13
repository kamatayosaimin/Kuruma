using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct GOISInput
{
    public KeyCode[] keys;
    public string[] buttons;

    public GOISInput(string button, KeyCode key)
        : this(new[] { button }, new[] { key })
    {
    }

    public GOISInput(KeyCode key, params string[] buttons)
        : this(buttons, new[] { key })
    {
    }

    public GOISInput(string button, params KeyCode[] keys)
        : this(new[] { button }, keys)
    {
    }

    public GOISInput(string[] buttons, KeyCode[] keys)
    {
        this.keys = keys;
        this.buttons = buttons;
    }

    public static bool GetButtons(params string[] buttons)
    {
        return buttons.Any(b => Input.GetButton(b));
    }

    public static bool GetButtonsDown(params string[] buttons)
    {
        return buttons.Any(b => Input.GetButtonDown(b));
    }

    public static bool GetButtonsUp(params string[] buttons)
    {
        return buttons.Any(b => Input.GetButtonUp(b));
    }

    public static bool GetKeys(params KeyCode[] keys)
    {
        return keys.Any(k => Input.GetKey(k));
    }

    public static bool GetKeysDown(params KeyCode[] keys)
    {
        return keys.Any(k => Input.GetKeyDown(k));
    }

    public static bool GetKeysUp(params KeyCode[] keys)
    {
        return keys.Any(k => Input.GetKeyUp(k));
    }

    public bool GetInput()
    {
        return GetInput(buttons, keys);
    }

    public static bool GetInput(string button, KeyCode key)
    {
        return Input.GetButton(button) || Input.GetKey(key);
    }

    public static bool GetInput(KeyCode key, params string[] buttons)
    {
        return GetButtons(buttons) || Input.GetKey(key);
    }

    public static bool GetInput(string button, params KeyCode[] keys)
    {
        return Input.GetButton(button) || GetKeys(keys);
    }

    public static bool GetInput(string[] buttons, KeyCode[] keys)
    {
        return GetButtons(buttons) || GetKeys(keys);
    }

    public bool GetInputDown()
    {
        return GetInputDown(buttons, keys);
    }

    public static bool GetInputDown(string button, KeyCode key)
    {
        return Input.GetButtonDown(button) || Input.GetKeyDown(key);
    }

    public static bool GetInputDown(KeyCode key, params string[] buttons)
    {
        return GetButtonsDown(buttons) || Input.GetKeyDown(key);
    }

    public static bool GetInputDown(string button, params KeyCode[] keys)
    {
        return Input.GetButtonDown(button) || GetKeysDown(keys);
    }

    public static bool GetInputDown(string[] buttons, KeyCode[] keys)
    {
        return GetButtonsDown(buttons) || GetKeysDown(keys);
    }

    public bool GetInputUp()
    {
        return GetInputUp(buttons, keys);
    }

    public static bool GetInputUp(string button, KeyCode key)
    {
        return Input.GetButtonUp(button) || Input.GetKeyUp(key);
    }

    public static bool GetInputUp(KeyCode key, params string[] buttons)
    {
        return GetButtonsUp(buttons) || Input.GetKeyUp(key);
    }

    public static bool GetInputUp(string button, params KeyCode[] keys)
    {
        return Input.GetButtonUp(button) || GetKeysUp(keys);
    }

    public static bool GetInputUp(string[] buttons, KeyCode[] keys)
    {
        return GetButtonsUp(buttons) || GetKeysUp(keys);
    }
}
