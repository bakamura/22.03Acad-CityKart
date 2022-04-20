using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarStatusUIManager : MonoBehaviour
{
    public static CarStatusUIManager Instance { get; private set; }
    [SerializeField] private Image[] sliders;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }
    public void ClearSliders()
    {
        foreach(Image image in sliders) image.fillAmount = 0f;
    }

    public void FillSliders(CarControler carStatus)
    {
        sliders[0].fillAmount = 1f / CarStatusManager.MaxVelocity * carStatus.Velocity;
        sliders[1].fillAmount = 1f / CarStatusManager.MaxHandling * carStatus.TurningDegrees;
        sliders[2].fillAmount = 1f / CarStatusManager.MaxDrift * carStatus.DriftAngle;
    }
}
