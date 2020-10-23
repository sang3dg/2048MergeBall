using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoEnd : MonoBehaviour
{
    Animator self;
    public static bool IsAnimation = false;
    private void Awake()
    {
        self = GetComponent<Animator>();
    }
    public void StopAnimator()
    {
        MainController.Instance.SetCurrentBallState(true);
        self.SetBool("LevelUp", false);
        IsAnimation = false;
        GameManager.Instance.WhenLevelUpAnimationEnd();
    }
    public void RefreshBallNum()
    {
        GameManager.Instance.RefreshTargetBallNum();
    }
}
