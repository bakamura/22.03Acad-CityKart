using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostEffect : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject BoostParticle;
    [Tooltip("How wide the FOV will be when in boost effects")]
    [SerializeField] private int FOVinBoost;
    [SerializeField] private CanvasGroup BoostEffectUI;
    [SerializeField] private PostProcessControler PPcontroler;
    [Min(.01f), Tooltip("How long the transition will take, in seconds")]
    [SerializeField] private float FOVTransitionDuration;
    [Range(1, 100), Tooltip("How much the FOV will change per tick, in percentage")]
    [SerializeField] private int FOVPercentageIncrease;

    [Header("Data")]
    [SerializeField] private PlayerData data;
    private float baseFOV;
    private Coroutine FOVTransition = null;
    private float currentDriftBoostDuration;
    private bool isEffectActive = false;

    private void Awake() {
        if (data) baseFOV = data.cm.m_Lens.FieldOfView;
        else Debug.LogError("the BoostEffect Script in" + this.gameObject.name + "needs the PlayerData in the inspector");
    }

    private void Update() {
        StopDriftBoost();
    }
    private void StopDriftBoost() {
        currentDriftBoostDuration -= Time.deltaTime;
        if (currentDriftBoostDuration <= 0 && isEffectActive) BoostVisualEffects(false);
    }

    public void BoostVisualEffects(bool isActive, float boostDuration = 0) {
        currentDriftBoostDuration = boostDuration;
        isEffectActive = isActive;
        PPcontroler.Activate_deactivateDepthOfField(isActive);
        BoostEffectUI.alpha = isActive ? 1 : 0;
        BoostParticle.SetActive(isActive);
        if (FOVTransition != null) StopCoroutine(FOVTransition);
        FOVTransition = StartCoroutine(FOVEffect(isActive));
    }

    IEnumerator FOVEffect(bool isActive) {
        float time = FOVTransitionDuration * (FOVPercentageIncrease / 100f);
        float increment = (FOVinBoost - baseFOV) * (FOVPercentageIncrease / 100f);
        if (isActive) {
            while (data.cm.m_Lens.FieldOfView < FOVinBoost) {
                data.cm.m_Lens.FieldOfView += increment;
                yield return new WaitForSeconds(time);
            }
        }
        else {
            while (data.cm.m_Lens.FieldOfView > baseFOV) {
                data.cm.m_Lens.FieldOfView -= increment;
                yield return new WaitForSeconds(time);
            }
        }
        FOVTransition = null;
    }
}
