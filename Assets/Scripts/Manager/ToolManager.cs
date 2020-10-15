using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager
{
    public static string GetCashShowString(int num)
    {
        string str = num.ToString();
        if (str.Length == 1)
            return "0.0" + str;
        else if (str.Length == 2)
            return "0." + str;
        else
            return str.Insert(str.Length - 2, ".");
    }
}
