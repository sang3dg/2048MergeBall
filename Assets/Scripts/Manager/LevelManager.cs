using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    public Ball targetBall_A;
    [Space(15)]
    public Ball targetBall_B;

    public Animator levelUpAnimator;
    bool currentGamePanelIsB = false;
    [NonSerialized]
    public bool isLevelUping = false;
    public void WhenLevelUp()
    {
        currentGamePanelIsB = !currentGamePanelIsB;
        AnimationAutoEnd.IsAnimation = true;
        levelUpAnimator.SetBool("LevelUp", true);
    }
    public void SetTargetBallNum(int num)
    {
        targetBall_B.InitTargetBall(num);
        targetBall_A.InitTargetBall(num);
    }
}
