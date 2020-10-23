using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public static MainController Instance;
    public GameObject prefab_ball;
    public float f_ballStartSpeed = 5;
    public RectTransform rect_ballPool;
    public RectTransform rect_leftBorder;
    public RectTransform rect_rightBorder;
    public RectTransform rect_line;
    public List<FailArea> FailAreas = new List<FailArea>();
    public bool AutoFire = true;
    [Space(15)]
    public GameObject prefab_lighting;
    [Space(15)]
    public RectTransform gamePanelRect;
    public RectTransform gamePanelTargetBallRect;
    [Space(15)]
    public Text energyText;
    public Text energyTimedown;
    [NonSerialized]
    public int BallMaxNum;
    private GameObject go_currentBall;
    private GameObject go_currentGiftBall = null;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartCoroutine("NaturalIncreaseEnergy");
        SpawnNewBall();
    }
    public void SpawnNewBall()
    {
        GameObject newBall = Instantiate(prefab_ball, rect_ballPool);
        RefreshCurrentBallMaxNum();
        BallSpawnData data = ConfigManager.GetBallSpawnConfig(BallMaxNum);
        List<BallNumWeight> ballNumWeights = data.ballNumWeights;
        int total = 0;
        foreach (BallNumWeight weight in ballNumWeights)
            total += weight.weight;
        int result = UnityEngine.Random.Range(0, total);
        int num = 0;
        int count = ballNumWeights.Count;
        for(int i = 0; i < count; i++)
        {
            int maxValue = 0;
            for(int j = 0; j <= i; j++)
            {
                maxValue += ballNumWeights[j].weight;
            }
            if (result < maxValue)
            {
                num = ballNumWeights[i].num;
                break;
            }
        }

        go_currentBall = newBall;
        float halfCircle = GetBallCircleHalf(num);
        go_currentBall.transform.localPosition = new Vector3(0, -halfCircle);
        go_currentBall.GetComponent<Rigidbody2D>().isKinematic = true;
        go_currentBall.GetComponent<CircleCollider2D>().isTrigger = true;
        go_currentBall.GetComponent<Ball>().InitBall(num);
        rect_line.gameObject.SetActive(true);
        rect_line.localPosition = new Vector3(0, -2 * halfCircle);
        InputManager.Instance.SetEnable(true);
#if UNITY_EDITOR
        if (AutoFire)
            FireBall();
#endif
    }
    public void OnStartDrag()
    {
        if (AnimationAutoEnd.IsAnimation) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect_ballPool, Input.mousePosition, Camera.main, out Vector2 mousePos);
        float x = Mathf.Clamp(mousePos.x, rect_leftBorder.localPosition.x, rect_rightBorder.localPosition.x);
        go_currentBall.transform.localPosition = new Vector3(x, go_currentBall.transform.localPosition.y);
        rect_line.localPosition = new Vector3(x, rect_line.localPosition.y);
    }
    public void OnDrag()
    {
        if (AnimationAutoEnd.IsAnimation) return;
        OnStartDrag();
    }
    public void OnEndDrag()
    {
        if (AnimationAutoEnd.IsAnimation) return;
        rect_line.gameObject.SetActive(false);
        FireBall();
    }
    public bool hasShowBuyEnergyPanel = false;
    void FireBall()
    {
        if (fail)
            return;
        if (go_currentBall is object)
        {
            if (GameManager.Instance.GetCurrentEnergy() <= 0l)
            {
                if (!hasShowBuyEnergyPanel)
                {
                    hasShowBuyEnergyPanel = true;
                    GameManager.Instance.UIManager.ShowPopPanelByType(UI.UI_Panel.UI_PopPanel.BuyEnergyPanel);
                }
                return;
            }
            GameManager.Instance.AddEnergy(-1);
            go_currentBall.GetComponent<Rigidbody2D>().isKinematic = false;
            go_currentBall.GetComponent<CircleCollider2D>().isTrigger = false;
            go_currentBall.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -f_ballStartSpeed, 0);
            InputManager.Instance.SetEnable(false);
        }
    }
    float GetBallCircleHalf(int num)
    {
        return ConfigManager.GetBallBaseConfig(num).BallSize * GameManager.ballCircle * 0.5f;
    }
    Coroutine stageUp = null;
    public void RefreshCurrentBallMaxNum()
    {
        List<Ball> balls = GetInOrderBallList();
        if (balls.Count > 0)
            BallMaxNum = balls[balls.Count - 1].Num;
        else
            BallMaxNum = 0;
        if (BallMaxNum >= GameManager.Instance.GetTargetLevelBallNum())
        {
            if (stageUp != null) return;
            SaveData();
            if (balls.Count > 0)
            {
               stageUp= StartCoroutine(MoveToTargetBall(balls[balls.Count - 1].transform));
            }
        }
    }
    private IEnumerator MoveToTargetBall(Transform ball)
    {
        AnimationAutoEnd.IsAnimation = true;
        ball.SetParent(gamePanelRect);
        float progress = 0;
        Vector3 startPos = ball.transform.localPosition;
        Vector3 targetPos = gamePanelTargetBallRect.localPosition - new Vector3(0, gamePanelTargetBallRect.sizeDelta.y / 2);
        ball.GetComponent<Rigidbody2D>().isKinematic = true;
        while (progress < 1)
        {
            progress += Time.deltaTime;
            progress = Mathf.Clamp(progress, 0, 1);
            ball.localPosition = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }
        Destroy(ball.gameObject);
        gamePanelTargetBallRect.GetComponent<Ball>().PlayMergeEffect();
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.LevelUp();
        stageUp = null;
    }
    bool fail = false;
    public bool CheckFail(bool delay = true)
    {
        bool fail = true;
        foreach(FailArea failArea in FailAreas)
        {
            if (failArea.ballNum <= 0)
            {
                fail = false;
                break;
            }
        }
        this.fail = fail;
        if (fail && delay)
        {
            StopCoroutine("DelayCheckFail");
            StartCoroutine("DelayCheckFail");
        }
        return fail;
    }
    IEnumerator DelayCheckFail()
    {
        yield return new WaitForSeconds(GameManager.checkFailDelayTime);
        if (CheckFail(false))
        {
            GameManager.Instance.UIManager.ShowPopPanelByType(UI.UI_Panel.UI_PopPanel.GameOverPanel);
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveData();
        }
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void SaveData()
    {
        if (AnimationAutoEnd.IsAnimation) return;
        List<Vector2> ballPos = new List<Vector2>();
        List<int> ballNum = new List<int>();
        Ball[] allBall = rect_ballPool.GetComponentsInChildren<Ball>();
        int count = allBall.Length;
        for (int i = 0; i < count; i++)
        {
            if (allBall[i].GetComponent<Rigidbody2D>().isKinematic == true)
                continue;
            ballPos.Add(allBall[i].transform.localPosition);
            ballNum.Add(allBall[i].Num);
        }
        GameManager.Instance.SaveBallData(ballPos, ballNum, go_currentBall.GetComponent<Ball>().Num);
    }
    public void LoadSaveData()
    {
        List<Vector2> ballPos;
        List<int> ballNum;
        int currentBallNum;
        ballPos = GameManager.Instance.GetBallData(out ballNum,out currentBallNum);
        if (currentBallNum != 0)
        {
            float halfCircle = GetBallCircleHalf(currentBallNum);
            go_currentBall.transform.localPosition = new Vector3(0, -halfCircle);
            go_currentBall.GetComponent<Ball>().InitBall(currentBallNum);
            rect_line.localPosition = new Vector3(0, -2 * halfCircle);
        }
        int count = ballPos.Count;
        for(int i = 0; i < count; i++)
        {
            GameObject newBall = Instantiate(prefab_ball, rect_ballPool);
            newBall.transform.localPosition = ballPos[i];
            newBall.GetComponent<Ball>().InitBall(ballNum[i]);
            newBall.GetComponent<Ball>().hasSpawNew = true;
            if (ballNum[i] == -2)
            {
                if (go_currentGiftBall is null)
                    go_currentGiftBall = newBall;
                else
                    Debug.LogError("存档错误，存在两个礼盒球");
            }
        }
        go_currentBall.transform.SetAsLastSibling();
        if (GameManager.Instance.GetWhetherFirstPlay())
        {
            GameManager.Instance.SetFirstPlayFalse();
            SpawnNewGiftBall();
        }
    }
    public void OnContinueGame()
    {
        List<Ball> ballList = GetInOrderBallList();
        if (ballList.Count == 0) return;
        int count = ballList.Count;
        int lastNum = ballList[0].Num;
        int deleteTypeNum = 0;
        for(int i = 0; i < count; i++)
        {
            if (ballList[i].Num != lastNum)
            {
                lastNum = ballList[i].Num;
                deleteTypeNum++;
            }
            if (deleteTypeNum >= 3)
                break;
            Destroy(ballList[i].gameObject);
        }
    }
    public void OnRestartGame()
    {
        Ball[] allBall = rect_ballPool.GetComponentsInChildren<Ball>();
        int count = allBall.Length;
        for (int i = 0; i < count; i++)
        {
            if (allBall[i].GetComponent<Rigidbody2D>().isKinematic == true)
                continue;
            Destroy(allBall[i].gameObject);
        }
        RefreshCurrentBallMaxNum();
        go_currentBall.GetComponent<Ball>().InitBall(2);
        SpawnNewGiftBall();
    }
    /// <summary>
    /// from small to big
    /// </summary>
    /// <returns></returns>
    public List<Ball> GetInOrderBallList()
    {
        Ball[] allBall = rect_ballPool.GetComponentsInChildren<Ball>();
        if (allBall.Length == 1) return new List<Ball>();
        int count = allBall.Length;
        List<Ball> ballList = new List<Ball>();
        for (int i = 0; i < count; i++)
        {
            if (allBall[i].GetComponent<Rigidbody2D>().isKinematic == true)
                continue;
            ballList.Add(allBall[i]);
        }
        ballList.Sort((a, b) =>
        {
            if (a.Num > b.Num) return 1;
            else if (a.Num == b.Num) return 0;
            else return -1;
        });
        return ballList;
    }
    const int Prop1NumIndex = -1;
    public void UseProp1()
    {

        float halfCircle = GetBallCircleHalf(Prop1NumIndex);
        go_currentBall.transform.localPosition = new Vector3(0, -halfCircle);
        go_currentBall.GetComponent<Ball>().InitBall(Prop1NumIndex);
        rect_line.localPosition = new Vector3(0, -2 * halfCircle);
        GameManager.Instance.ShowUsePropGiftPanel();
    }
    public bool UseProp2()
    {
        List<Ball> ballList = GetInOrderBallList();
        int count = ballList.Count;
        int hasMergeCount = 0;
        List<Ball> willDestoryBalls = new List<Ball>();
        List<Ball> willMergeSelfBalls = new List<Ball>();

        Ball currentMergeBall = null;
        for(int i = 0; i < count; i++)
        {
            Ball thisBall = ballList[i];
            if (currentMergeBall is null|| thisBall.Num != currentMergeBall.Num)
                currentMergeBall = thisBall;
            else
            {
                Vector2 mergeBall1Pos = currentMergeBall.transform.localPosition;
                Vector2 mergeBall2Pos = thisBall.transform.localPosition;
                bool destory1 = false;
                if (mergeBall1Pos.y <= mergeBall2Pos.y)
                    destory1 = true;
                else if(mergeBall1Pos.x<=mergeBall2Pos.x)
                    destory1 = true;
                if (destory1)
                {
                    willDestoryBalls.Add(currentMergeBall);
                    willMergeSelfBalls.Add(thisBall);
                }
                else
                {
                    willDestoryBalls.Add(thisBall);
                    willMergeSelfBalls.Add(currentMergeBall);
                }
                currentMergeBall = null;
                hasMergeCount++;
                if (hasMergeCount >= 3)
                    break;
            }
        }
        if (hasMergeCount > 0)
            StartCoroutine(Prop2Animation(willDestoryBalls, willMergeSelfBalls));
        return hasMergeCount != 0;
    }
    private List<GameObject> allLighting = new List<GameObject>();
    [Space(10)]
    public GameObject prop2;
    private IEnumerator Prop2Animation(List<Ball> willDestoryBall, List<Ball> willMergeSelfBall)
    {
        AnimationAutoEnd.IsAnimation = true;
        rect_line.gameObject.SetActive(false);
        go_currentBall.SetActive(false);
        prop2.SetActive(true);
        yield return new WaitForSeconds(1);
        float speed = 3000;
        int allLightingCount = willDestoryBall.Count + willMergeSelfBall.Count;
        while(allLighting.Count< allLightingCount)
        {
            GameObject oneLighting = Instantiate(prefab_lighting, rect_ballPool);
            oneLighting.SetActive(false);
            allLighting.Add(oneLighting);
        }
        while (allLighting.Count > allLightingCount)
        {
            Destroy(allLighting[allLighting.Count - 1]);
            allLighting.RemoveAt(allLighting.Count - 1);
        }
        yield return null;
        for(int i = 0; i < allLightingCount; i++)
        {
            GameObject one = allLighting[i];
            one.transform.localPosition = go_currentBall.transform.localPosition;
            one.GetComponent<TrailRenderer>().Clear();
            one.SetActive(true);
        }
        Vector3 startPos = go_currentBall.transform.localPosition;
        List<Vector3> targetPos = new List<Vector3>();
        List<Vector3> targetDir = new List<Vector3>();
        foreach(var ball in willDestoryBall)
        {
            targetPos.Add(ball.transform.localPosition);
        }
        foreach(var ball in willMergeSelfBall)
        {
            targetPos.Add(ball.transform.localPosition);
        }

        if (allLighting.Count != targetPos.Count)
        {
            Debug.LogError("闪电个数与球的个数不匹配");
            yield break;
        }
        for(int i = 0; i < allLightingCount; i++)
        {
            targetDir.Add((targetPos[i] - startPos).normalized);
        }

        int movingCount = allLightingCount;
        float Y = startPos.y;
        while (movingCount >0)
        {
            yield return null;
            Y -= Time.deltaTime * speed;
            movingCount = 0;
            for(int i = 0; i < allLightingCount; i++)
            {
                Vector3 target = targetPos[i];
                if (Y < target.y)
                    continue;
                else
                {
                    movingCount++;
                    allLighting[i].transform.localPosition = new Vector3(Y / targetDir[i].y * targetDir[i].x, Y);
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
        AnimationAutoEnd.IsAnimation = false;
        foreach(var ball in willDestoryBall)
        {
            Destroy(ball.gameObject);
        }
        foreach(var ball in willMergeSelfBall)
        {
            ball.MergeSelfNum();
        }
        yield return new WaitForSeconds(0.3f);
        go_currentBall.SetActive(true);
        rect_line.gameObject.SetActive(true);
        prop2.SetActive(false);
        GameManager.Instance.ShowUsePropGiftPanel();
    }
    public void SpawnNewGiftBall()
    {
        if (go_currentGiftBall != null && go_currentGiftBall.GetComponent<Ball>().Num == -2)
            Destroy(go_currentGiftBall);
        GameObject newBall = Instantiate(prefab_ball, rect_ballPool);
        go_currentGiftBall = newBall;
        newBall.GetComponent<Ball>().InitBall(-2);
        float offset = GetBallCircleHalf(-2);
        float x = UnityEngine.Random.Range(rect_leftBorder.localPosition.x, rect_rightBorder.localPosition.x);
        newBall.transform.localPosition = new Vector2(x, -offset * 2);
        GameManager.Instance.AddGiftBallAppearTime();
        GameManager.Instance.SendAdjustSpawnGiftballEvent();
    }
    public void SetCurrentBallState(bool show)
    {
        go_currentBall.SetActive(show);
        rect_line.gameObject.SetActive(show);
    }
    public void RefreshEnergyText()
    {
        energyText.text = GameManager.Instance.GetCurrentEnergy().ToString();
    }
    IEnumerator NaturalIncreaseEnergy()
    {
        int timer = 60;
        energyTimedown.text = "1:00";
        WaitForSeconds oneMinutes = new WaitForSeconds(1);
        while (true)
        {
            yield return oneMinutes;
            timer--;
            if (GameManager.Instance.GetCurrentEnergy() < GameManager.maxEnergy)
                energyTimedown.text = "0:" + (timer > 9 ? timer.ToString() : "0" + timer);
            else
                energyTimedown.text = "1:00";
            if (timer <= 0)
            {
                timer = 60;
                GameManager.Instance.AddEnergy(1);
            }
        }
    }
}
