using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
    public GameObject tips;
    public static TipsManager Instance;
    private readonly List<Tip> allTips = new List<Tip>();
    private void Awake()
    {
        Instance = this;
        allTips.Add(new Tip(tips, this));
    }
    public void ShowTips(string content,float delayHideTime=1)
    {
        int count = allTips.Count;
        bool hasShow = false;
        for(int i = 0; i < count; i++)
        {
            Tip tempTip = allTips[i];
            if (tempTip.tipIsUse)
                tempTip.CrowdUp();
            else
            {
                if (hasShow)
                    continue;
                else
                {
                    hasShow = true;
                    tempTip.ShowTip(content, delayHideTime);
                }
            }
        }
        if (!hasShow)
        {
            Tip newTip = new Tip(Instantiate(tips, tips.transform.parent), this);
            newTip.ShowTip(content, delayHideTime);
            allTips.Add(newTip);
        }
    }
    private class Tip
    {
        public Tip(GameObject tip,MonoBehaviour animationPlayer)
        {
            tipTrans = tip.transform as RectTransform;
            tipTrans.localPosition = Vector3.zero;
            tipCG = tip.GetComponent<CanvasGroup>();
            tipCG.alpha = 0;
            tipCG.blocksRaycasts = false;
            tipContent = tip.GetComponentInChildren<Text>();
            player = animationPlayer;
            tipIsUse = false;
        }
        private MonoBehaviour player;
        private RectTransform tipTrans;
        private CanvasGroup tipCG;
        private Text tipContent;
        public bool tipIsUse;
        public void ShowTip(string content,float stayTime = 1)
        {
            if (!tipIsUse)
                player.StartCoroutine(PlayTipsAniamtion(content, stayTime));
        }
        private IEnumerator PlayTipsAniamtion(string content,float stayTime)
        {
            tipIsUse = true;
            tipContent.text = content;
            tipCG.alpha = 1;
            tipTrans.localPosition = Vector3.zero;
            yield return new WaitForSeconds(stayTime);
            float progress = tipCG.alpha;
            while (progress > 0)
            {
                progress -= Time.deltaTime * 2;
                progress = Mathf.Clamp(progress, 0, 1);
                tipCG.alpha = progress;
                yield return null;
            }
            tipIsUse = false;
        }
        public void CrowdUp()
        {
            tipTrans.localPosition += new Vector3(0, tipTrans.sizeDelta.y + 10, 0);
            tipCG.alpha -= 0.1f;
        }
    }
}
