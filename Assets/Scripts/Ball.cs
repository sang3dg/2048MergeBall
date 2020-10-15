using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
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
        MainController.Instance.CheckNewMaxNum(num);
        BallBaseData config = ConfigManager.GetBallBaseConfig(num);
        img_icon.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Main, config.BallSpriteName.ToString());
        string numStr = num.ToString();
        text_num.text = numStr;
        rect_self.localScale = new Vector3(config.BallSize, config.BallSize);
        float text_width = numStr.Length * 100;
        rect_text.sizeDelta = new Vector2(text_width, GameManager.ballTextHeight);
        float text_scale = GameManager.ballCircle / (text_width+50);
        rect_text.localScale = new Vector3(text_scale, text_scale);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasSpawNew)
        {
            MainController.Instance.SpawnNewBall();
            hasSpawNew = true;
        }
        if (!isExplosion && collision.collider.CompareTag("Ball"))
        {
            Ball otherBall = collision.gameObject.GetComponent<Ball>();
            if (!otherBall.willBeDestory && Num == otherBall.Num)
            {
                if (otherBall.rect_self.localPosition.y >= rect_self.localPosition.y)
                {
                    if (!otherBall.isExplosion)
                    {
                        otherBall.willBeDestory = true;
                        MainController.Instance.RecycleBall(otherBall.gameObject);
                        isExplosion = true;
                        go_explosion.SetActive(false);
                        go_explosion.SetActive(true);
                        InitBall(Num * 2);
                        GameManager.Instance.AddScore(Num);
                        if (gameObject.activeSelf)
                            StartCoroutine("WaitForExplosion");
                        else
                        {
                            isExplosion = false;
                        }
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
