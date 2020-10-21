using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public static int CombeNum = 0;
    public Image img_icon;
    public Text text_num;
    public RectTransform rect_self;
    public RectTransform rect_text;
    public GameObject go_explosion;
    [NonSerialized]
    public int Num;
    [NonSerialized]
    public bool hasSpawNew = false;
    [NonSerialized]
    public bool willBeDestory = false;
    private void Awake()
    {
        hasSpawNew = false;
        isExplosion = false;
    }
    public void InitBall(int num)
    {
        Num = num;
        BallBaseData config = ConfigManager.GetBallBaseConfig(num);
        MainController.Instance.RefreshCurrentBallMaxNum();
        img_icon.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Ball, config.BallSpriteName.ToString());
        rect_self.localScale = new Vector3(config.BallSize, config.BallSize);
        if (num > 0)
        {
            string numStr = ToolManager.GetBallNumShowString(num);
            text_num.text = numStr;
            float text_width = numStr.Length * 100;
            rect_text.sizeDelta = new Vector2(text_width, GameManager.ballTextHeight);
            float text_scale = GameManager.ballCircle / (text_width + 50);
            rect_text.localScale = new Vector3(text_scale, text_scale);
            if (!text_num.gameObject.activeSelf)
                text_num.gameObject.SetActive(true);
        }
        else
            text_num.gameObject.SetActive(false);
    }
    public void MergeSelfNum()
    {
        InitBall(Num + Num);
        GameManager.Instance.AddScore(Num);
        PlayMergeEffect();
    }
    public void PlayMergeEffect()
    {
        isExplosion = true;
        go_explosion.SetActive(false);
        go_explosion.SetActive(true);
        CombeNum++;
        GameManager.Instance.PlayMergeBallCombeSound(CombeNum);
        StopCoroutine("WaitForExplosion");
        StartCoroutine("WaitForExplosion");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasSpawNew)
        {
            if (Num != -2)
            {
                GameManager.Instance.AddBallFallNum();
                MainController.Instance.SpawnNewBall();
                CombeNum = 0;
            }
            hasSpawNew = true;
        }
        if (!isExplosion && collision.collider.CompareTag("Ball"))
        {
            Ball otherBall = collision.gameObject.GetComponent<Ball>();
            if (Num < 0)
            {
                if (willBeDestory)
                    return;
                switch (Num)
                {
                    case -1:
                        otherBall.MergeSelfNum();
                        willBeDestory = true;
                        Destroy(gameObject);
                        return;
                    case -2:
                        if (otherBall.rect_self.localPosition.y >= rect_self.localPosition.y)
                        {
                            otherBall.willBeDestory = true;
                            InitBall(otherBall.Num);
                            Destroy(otherBall.gameObject);
                            GameManager.Instance.WhenGetGfitBall();
                            PlayMergeEffect();
                        }
                        return;
                }
            }
            if (!otherBall.willBeDestory && Num == otherBall.Num)
            {
                if (otherBall.rect_self.localPosition.y >= rect_self.localPosition.y)
                {
                    if (!otherBall.isExplosion)
                    {
                        otherBall.willBeDestory = true;
                        Destroy(otherBall.gameObject);
                        isExplosion = true;
                        go_explosion.SetActive(false);
                        go_explosion.SetActive(true);
                        MergeSelfNum();
                    }
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }
    bool isExplosion = false;
    IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(GameManager.ballExplosionTime);
        isExplosion = false;
    }
}
