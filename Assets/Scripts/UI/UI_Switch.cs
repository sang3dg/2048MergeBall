using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Switch : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler
{
    public bool interactable = true;
    [SerializeField]
    private bool isOn = false;
    [HideInInspector]
    public bool IsOn 
    {
        get
        {
            return isOn;
        }
        set
        {
            isOn = value;
            OnValueChanged.Invoke(isOn);
            StopCoroutine("SwitchAnimation");
            StartCoroutine("SwitchAnimation");
        } 
    }
    public Image targetOffGraphic;
    public Image targetOnGraphic;
    [Space(10)]
    public Color normalColor = Color.white;
    public Color pressColor = new Color32(200, 200, 200, 255);
    public Color highlightColor = Color.white;
    public Color disableColor = Color.gray;
    [Space(10)]
    public RectTransform handle;
    public float handleMoveSpeed = 5;
    [HideInInspector]
    public SwitchHandleMoveType handleMoveType = SwitchHandleMoveType.Horizontal;
    [HideInInspector]
    public SwitchHandleHorizontalMoveDirection handleHorizontalMoveDirection = SwitchHandleHorizontalMoveDirection.LeftToRight;
    [HideInInspector]
    public SwitchHandleVerticalMoveDirection handleVerticalMoveDirection = SwitchHandleVerticalMoveDirection.BottomToTop;
    [HideInInspector]
    public SwitchHandleDiagonalMoveDirection handleDiagonalMoveDirection = SwitchHandleDiagonalMoveDirection.LeftTopToRightBottom;
    [HideInInspector]
    public RectTransform handleMoveArea;
    [HideInInspector]
    public Toggle.ToggleEvent OnValueChanged;
    private void Awake()
    {
        switch (handleMoveType)
        {
            case SwitchHandleMoveType.Vertical:
                switch (handleVerticalMoveDirection)
                {
                    case SwitchHandleVerticalMoveDirection.TopToBottom:
                        offPosition = new Vector2(0, (handleMoveArea.sizeDelta.y - handle.sizeDelta.y) * 0.5f);
                        onPosition = -offPosition;
                        break;
                    case SwitchHandleVerticalMoveDirection.BottomToTop:
                        offPosition = new Vector2(0, (handle.sizeDelta.y - handleMoveArea.sizeDelta.y) * 0.5f);
                        onPosition = -offPosition;
                        break;
                }
                break;
            case SwitchHandleMoveType.Horizontal:
                switch (handleHorizontalMoveDirection)
                {
                    case SwitchHandleHorizontalMoveDirection.LeftToRight:
                        offPosition = new Vector2((handle.sizeDelta.x - handleMoveArea.sizeDelta.x) * 0.5f, 0);
                        onPosition = -offPosition;
                        break;
                    case SwitchHandleHorizontalMoveDirection.RightToLeft:
                        offPosition = new Vector2((handleMoveArea.sizeDelta.x - handle.sizeDelta.x) * 0.5f, 0);
                        onPosition = -offPosition;
                        break;
                }
                break;
            case SwitchHandleMoveType.Diagonal:
                switch (handleDiagonalMoveDirection)
                {
                    case SwitchHandleDiagonalMoveDirection.LeftTopToRightBottom:
                        offPosition = new Vector2((handle.sizeDelta.x - handleMoveArea.sizeDelta.x) * 0.5f, (handleMoveArea.sizeDelta.y - handle.sizeDelta.y) * 0.5f);
                        onPosition = -offPosition;
                        break;
                    case SwitchHandleDiagonalMoveDirection.RightBottomToLeftTop:
                        offPosition = new Vector2((handleMoveArea.sizeDelta.x - handle.sizeDelta.x) * 0.5f, (handle.sizeDelta.y - handleMoveArea.sizeDelta.y) * 0.5f);
                        onPosition = -offPosition;
                        break;
                    case SwitchHandleDiagonalMoveDirection.RightTopToLeftBottom:
                        offPosition = new Vector2((handleMoveArea.sizeDelta.x - handle.sizeDelta.x) * 0.5f, (handleMoveArea.sizeDelta.y - handle.sizeDelta.y) * 0.5f);
                        onPosition = -offPosition;
                        break;
                    case SwitchHandleDiagonalMoveDirection.LeftBottomTopRightTop:
                        offPosition = new Vector2((handle.sizeDelta.x - handleMoveArea.sizeDelta.x) * 0.5f, (handle.sizeDelta.y - handleMoveArea.sizeDelta.y) * 0.5f);
                        onPosition = -offPosition;
                        break;
                }
                break;
        }
        if (isOn)
        {
            handle.localPosition = onPosition;
            targetOnGraphic.fillAmount = 1;
        }
        else
        {
            handle.localPosition = offPosition;
            targetOnGraphic.fillAmount = 0;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        isOn = !isOn;
        OnValueChanged.Invoke(isOn);
        StopCoroutine("SwitchAnimation");
        StartCoroutine("SwitchAnimation");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetOnGraphic.color = pressColor;
        targetOffGraphic.color = pressColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetOnGraphic.color = normalColor;
        targetOffGraphic.color = normalColor;
    }
    Vector2 onPosition = Vector2.zero;
    Vector2 offPosition = Vector2.zero;
    private IEnumerator SwitchAnimation()
    {
        Vector2 offset = onPosition - offPosition;
        bool isH = onPosition.y - offPosition.y == 0;
        float progress = isH ? (handle.localPosition.x - offPosition.x) / (onPosition.x - offPosition.x) : (handle.localPosition.y - offPosition.y) / (onPosition.y - offPosition.y);
        while (true)
        {
            progress += Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f) * handleMoveSpeed * (isOn ? 1 : -1);
            handle.localPosition = offPosition + offset * progress;
            targetOnGraphic.fillAmount = progress;
            yield return null;
            if (progress >= 1)
            {
                handle.localPosition = onPosition;
                targetOnGraphic.fillAmount = 1;
                yield break;
            }
            else if (progress <= 0)
            {
                handle.localPosition = offPosition;
                targetOnGraphic.fillAmount = 0;
                yield break;
            }
        }
    }
    private void Reset()
    {
        interactable = true;
        isOn = false;
        targetOffGraphic = null;
        targetOnGraphic = null;
        handle = null;
        handleMoveArea = null;
        handleMoveSpeed = 5;
        normalColor = Color.white;
        pressColor = new Color32(200, 200, 200, 255);
        highlightColor = Color.white;
        disableColor = Color.gray;
        OnValueChanged.RemoveAllListeners();
    }
    public enum SwitchHandleMoveType
    {
        Vertical,
        Horizontal,
        Diagonal
    }
    public enum SwitchHandleVerticalMoveDirection
    {
        TopToBottom,
        BottomToTop
    }
    public enum SwitchHandleHorizontalMoveDirection
    {
        LeftToRight,
        RightToLeft
    }
    public enum SwitchHandleDiagonalMoveDirection
    {
        LeftTopToRightBottom,
        RightBottomToLeftTop,
        RightTopToLeftBottom,
        LeftBottomTopRightTop
    }
}
