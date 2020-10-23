using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlotsPanel : UI_PopPanelBase
{
    public Button closeButton;
    public Button spinButton;
    public Image rewards_L;
    public Image rewards_M;
    public Image rewards_R;
    public GameObject adIcon;
    public RectTransform spinContentRect;
    static readonly float[] rewards_Y_Offsets = new float[4]
    {
        -0.02f,
        0.22f,
        0.477f,
        0.725f
    };
    static readonly bool[] rewards_isCash = new bool[4]
    {
        true,
        false,
        true,
        false
    };
    protected override void Awake()
    {
        base.Awake();
        PanelType = UI_Panel.UI_PopPanel.SlotsPanel;
        closeButton.onClick.AddListener(OnCloseClick);
        spinButton.onClick.AddListener(OnSpinClick);
    }
    private void OnCloseClick()
    {
        GameManager.PlayButtonClickSound();
        if (isSpining) return;
        UIManager.ClosePopPanel(this);
    }
    float endOffset = 0;
    int reward = 0;
    bool isSpining = false;
    private void OnSpinClick()
    {
        GameManager.PlayButtonClickSound();
        if (isSpining) return;
        isSpining = true;
        if (needAd)
        {
            Debug.Log("观看广告转动老虎机");
            needAd = true;
            adIcon.SetActive(true);
            spinContentRect.localPosition = adSpincontentPos;
        }
        else
        {
            Debug.Log("免费转动老虎机");
        }
        GameManager.AddSpinSlotsTime();
        GameManager.SendAdjustSpinSlotsEvent();
        reward = GameManager.RandomSlotsReward();
        if (reward < 0)
        {
            endOffset = rewards_Y_Offsets[0];
        }
        else
            endOffset = rewards_Y_Offsets[1];
        StartCoroutine("AutoSpinSlots");
    }
    const float SpinSpeed = 8;
    const float SpinTime = 1;
    const string ShaderProperty = "_MainTex";
    private IEnumerator AutoSpinSlots()
    {
        float timer = 0;
        float currentOffset = endOffset;
        Material material_L = rewards_L.material;
        Material material_M = rewards_M.material;
        Material material_R = rewards_R.material;
        Vector2 offset = new Vector2(0, endOffset);
        material_L.SetTextureOffset(ShaderProperty, offset);
        material_M.SetTextureOffset(ShaderProperty, offset);
        material_R.SetTextureOffset(ShaderProperty, offset);
        AudioSource tempAs = GameManager.PlaySpinSound();
        while (timer < SpinTime)
        {
            yield return null;
            float delta = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
            timer += delta;
            currentOffset += delta * SpinSpeed;
            offset = new Vector2(0, currentOffset);
            material_L.SetTextureOffset(ShaderProperty, offset);
            material_M.SetTextureOffset(ShaderProperty, offset);
            material_R.SetTextureOffset(ShaderProperty, offset);
        }
        tempAs.Stop();
        currentOffset = endOffset + 0.03f;
        offset = new Vector2(0, currentOffset);
        material_L.SetTextureOffset(ShaderProperty, offset);
        material_M.SetTextureOffset(ShaderProperty, offset);
        material_R.SetTextureOffset(ShaderProperty, offset);
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }
        yield return null;
        offset = new Vector2(0, endOffset);
        material_L.SetTextureOffset(ShaderProperty, offset);
        material_M.SetTextureOffset(ShaderProperty, offset);
        material_R.SetTextureOffset(ShaderProperty, offset);

        for (int i = 0; i < 60; i++)
        {
            yield return null;
        }

        UIManager.ClosePopPanel(this);
        Reward type = reward < 0 ? Reward.Cash : Reward.Coin;
        reward = Mathf.Abs(reward);
        GameManager.ShowConfirmRewardPanel(type, reward);
        isSpining = false;
    }
    static Vector2 noadSpincontentPos = new Vector2(0, 2.4f);
    static Vector2 adSpincontentPos = new Vector2(47.5f, 2.4f);
    bool needAd = false;
    Coroutine closeDelay = null;
    protected override void OnStartShow()
    {
        base.OnStartShow();
        ResetSlotsReward();
        if (GameManager.nextSlotsIsUpgradeSlots)
        {
            needAd = false;
            adIcon.SetActive(false);
            spinContentRect.localPosition = noadSpincontentPos;
        }
        else
        {
            needAd = true;
            adIcon.SetActive(true);
            spinContentRect.localPosition = adSpincontentPos;
        }
        closeDelay= StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(closeButton.gameObject));
    }
    protected override void OnEndClose()
    {
        StopCoroutine(closeDelay);
    }
    private void ResetSlotsReward()
    {
        Material material_L = rewards_L.material;
        Material material_M = rewards_M.material;
        Material material_R = rewards_R.material;
        Vector2 offset = new Vector2(0, rewards_Y_Offsets[1]);
        material_L.SetTextureOffset(ShaderProperty, offset);
        material_M.SetTextureOffset(ShaderProperty, offset);
        material_R.SetTextureOffset(ShaderProperty, offset);
    }
}
