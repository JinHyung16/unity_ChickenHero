using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;

public class MobileManager : Singleton<MobileManager>
{
    private TouchScreenKeyboard keyboard;
    private string keyboardText;
    public void ActiveMobileKeyBoard(bool active)
    {
        if (active)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable);
        }
        else
        {
            keyboard = null;
        }

    }

    public string GetInputStringInKeyBoard()
    {
        keyboardText = keyboard.text;
        return keyboardText;
    }
}
