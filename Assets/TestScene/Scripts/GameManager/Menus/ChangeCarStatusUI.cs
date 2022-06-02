using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCarStatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlayerData carStatus;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CarStatusUIManager.Instance.FillSliders(carStatus);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CarStatusUIManager.Instance.ClearSliders();
    }
}
