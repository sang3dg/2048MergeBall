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
    public static string GetBallNumShowString(int num)
    {
        if (num <= 4096)
            return num.ToString();
        else
        {
            string numStr = num.ToString();
            int frontNum = numStr.Length % 3;
            if (frontNum == 1 && (numStr[0] == '2' || numStr[0] == '1'))
            {
                return numStr.Substring(0, 4) + GetNumAbb((numStr.Length / 3) - 1);
            }
            else if(frontNum==0)
            {
                return numStr.Substring(0, 3) + GetNumAbb((numStr.Length / 3) - 1);
            }
            else
            {
                return numStr.Substring(0, frontNum) + GetNumAbb(numStr.Length / 3);
            }
        }
    }
    private static string GetNumAbb(int numDevideThree)
    {
        switch (numDevideThree)
        {
            case 1:
                return "K";
            case 2:
                return "M";
            case 3:
                return "B";
            case 4:
                return "T";
            case 5:
                return "Q";
            case 6:
                return "S";
            case 7:
                return "O";
            case 8:
                return "N";
            default:
                return "E+" + numDevideThree * 3;
        }
    }
    public static IEnumerator DelaySecondShowNothanksOrClose(GameObject nothanks,float second = 1)
    {
        nothanks.gameObject.SetActive(false);
        yield return new WaitForSeconds(second);
        nothanks.gameObject.SetActive(true);
    }
}
