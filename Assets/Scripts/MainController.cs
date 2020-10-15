using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public static MainController Instance;
    public GameObject prefab_ball;
    public float f_ballStartSpeed = 5;
    public RectTransform rect_ballStartPos;
    public RectTransform rect_ballPool;
    public RectTransform rect_grid;
    public RectTransform rect_leftBorder;
    public RectTransform rect_rightBorder;
    public RectTransform rect_line;
    public List<FailArea> FailAreas = new List<FailArea>();
    public bool AutoFire = true;
    [NonSerialized]
    public int BallMaxNum;
    private GameObject go_currentBall;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SpawnNewBall();
        LoadSaveData();
    }
    public void SpawnNewBall()
    {
        GameObject newBall = Instantiate(prefab_ball, rect_ballPool);
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
    public void RecycleBall(GameObject ball)
    {
        Destroy(ball);
    }
    Vector3 lastMousePos;
    Vector3 currenntMousePos;
    public void OnStartDrag()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect_ballPool, Input.mousePosition, Camera.main, out Vector2 mousePos);
        float x = Mathf.Clamp(mousePos.x, rect_leftBorder.localPosition.x, rect_rightBorder.localPosition.x);
        go_currentBall.transform.localPosition = new Vector3(x, go_currentBall.transform.localPosition.y);
        rect_line.localPosition = new Vector3(x, rect_line.localPosition.y);
    }
    public void OnDrag()
    {
        OnStartDrag();
    }
    public void OnEndDrag()
    {
        rect_line.gameObject.SetActive(false);
        FireBall();
    }
    void FireBall()
    {
        if (fail)
            return;
        if (go_currentBall is object)
        {
            GameManager.Instance.AddBallFallNum();
            go_currentBall.GetComponent<Rigidbody2D>().isKinematic = false;
            go_currentBall.GetComponent<CircleCollider2D>().isTrigger = false;
            go_currentBall.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -f_ballStartSpeed, 0);
            InputManager.Instance.SetEnable(false);
        }
    }
    float GetBallMoveX(Vector3 lastMousePos,Vector3 currentMousePos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect_grid, lastMousePos, Camera.main, out Vector2 lastMouseInRectanglePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect_grid, currentMousePos, Camera.main, out Vector2 currentMouseInRectanglePos);
        return currentMouseInRectanglePos.x - lastMouseInRectanglePos.x;
    }
    float GetBallCircleHalf(int num)
    {
        return ConfigManager.GetBallBaseConfig(num).BallSize * GameManager.ballCircle * 0.5f;
    }
    public void CheckNewMaxNum(int num)
    {
        if (num > BallMaxNum)
            BallMaxNum = num;
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
            Debug.LogError("Fail");
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
    private void SaveData()
    {
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
    private void LoadSaveData()
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
        }
    }
}
