using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour, IDragHandler,IPointerUpHandler,IPointerDownHandler
{
    public static InputManager Instance;
    Image img;
    private void Awake()
    {
        Instance = this;
        img = gameObject.GetComponent<Image>();
    }
    public void SetEnable(bool enable)
    {
        img.raycastTarget = enable;
    }
    public void OnDrag(PointerEventData eventData)
    {
        MainController.Instance.OnDrag();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        MainController.Instance.OnStartDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MainController.Instance.OnEndDrag();
    }
}
