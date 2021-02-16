using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyInputs {
    //K_ is keyboard, 1_ is first GC, 2_ is second etc
    public static string[] prefixes = new string[] { "K_", "1_", "2_", "3_", "4_"};


    public static void ChangeControllers(string[] newPrefixes)
    {
        prefixes = newPrefixes;
    }

    public static float GetAxis(int player, string axis)
    {
        return Input.GetAxis(prefixes[player] + axis);
    }
    public static float GetAxisRaw(int player, string axis)
    {
        return Input.GetAxisRaw(prefixes[player] + axis);
    }

    public static bool GetButton(int player, string axis)
    {
        return Input.GetButton(prefixes[player] + axis);
    }

    public static bool GetButtonDown(int player, string axis)
    {
        return Input.GetButtonDown(prefixes[player] + axis);
    }

    public static bool GetButtonUp(int player, string axis)
    {
        return Input.GetButtonUp(prefixes[player] + axis);
    }
}
