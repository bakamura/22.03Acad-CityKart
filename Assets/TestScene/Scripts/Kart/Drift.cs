using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Drift : MonoBehaviour {
    [Header("Visuals")]
    //[SerializeField] private CinemachineVirtualCamera cm;
    [SerializeField] private GameObject BoostParticle;
    [Tooltip("How wide the FOV will be when in boost effects")]
    [SerializeField] private int FOVinBoost;
    [SerializeField] private CanvasGroup BoostEffectUI;
    [SerializeField] private PostProcessControler PPcontroler;
    [Min(.01f), Tooltip("How long the transition will take, in seconds")]
    [SerializeField] private float FOVTransitionDuration;
    [Range(.01f, 1f), Tooltip("How much the FOV will change per tick, in percentage")]
    [SerializeField] private float FOVPercentageIncrease;

    [Header("Data")]
    private float baseFOV;
    private float newVehicleDriftRotation = 0;
    private Coroutine FOVTransition = null;
    private PlayerData data;
    private ControlCarWheel carByWheels;
    private ControlCarSphere carBySphere;
    private float currentDriftAmount;
    private Coroutine driftBoostCoroutine = null;
    private float currentDriftBoostDuration;


    [Tooltip("the velocity of the vehicle at each level of boost.")]
    [SerializeField] private float[] DriftBoostAmount = new float[3];

    [Tooltip("For how long the drift boost will last, in seconds.")]
    [SerializeField] private float[] DriftBoostDuration = new float[3];

    [Tooltip("For how long needs to be drifting to gain a boost, in seconds.")]
    [SerializeField] private float[] DriftBoostTime = new float[3];

    [Tooltip("how much the drift angle will change each FixedDeltaTime.")]
    [SerializeField] private float DriftAngleAmount;

    private void Awake() {
        data = GetComponent<PlayerData>();
        data.inputManager = GetComponent<InputCar>();
        data.rb = GetComponent<Rigidbody>();
        carByWheels = GetComponent<ControlCarWheel>();
        carBySphere = GetComponent<ControlCarSphere>();
        baseFOV = data.cm.m_Lens.FieldOfView;
    }

    private void Update() {
        StartDrift();        
    }

    public void StartDrift() {
        if (data.inputManager.Drift() && data.inputManager.VertMov() > 0 && data.inputManager.HorzMov() != 0 && data.rb.velocity.magnitude > 1f) {

                //if (Mathf.Abs(data.WheelsScript[0].wheelCollider.steerAngle) >= .01f && data.isGrounded) {
                    //foreach (WheelSinc wheel in carByWheels.WheelsScript) if (wheel.trail != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
                    //if (Mathf.Abs(carByWheels.WheelsScript[0].wheelCollider.steerAngle) >= .01f && data.isGrounded) {
                    currentDriftAmount += Time.deltaTime;
                    foreach (WheelSinc wheel in data.WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
                    //}
                //}
                //else if (currentDriftAmount != 0) {
                //    if (currentDriftAmount >= DriftBoostTime[0] / 2f) DriftBoost();
                //    foreach (WheelSinc wheel in data.WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(false, currentDriftAmount, DriftBoostTime);
                //    currentDriftAmount = 0;
                //}                
            //if (currentDriftAmount != 0 && !data.inputManager.Drift()) {
            //    if (currentDriftAmount >= DriftBoostTime[0] / 2f) DriftBoost();
            //    foreach (WheelSinc wheel in data.WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(false, currentDriftAmount, DriftBoostTime);
            //    currentDriftAmount = 0;
            //}            
        }
        else if (currentDriftAmount != 0 && !data.inputManager.Drift()) {
            if (currentDriftAmount >= DriftBoostTime[0] / 2f) DriftBoost();
            foreach (WheelSinc wheel in data.WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(false, currentDriftAmount, DriftBoostTime);
            currentDriftAmount = 0;
        }
        RotateVehicleDrift();
    }
    void DriftBoost() {
        int boostType = 0;
        switch (currentDriftAmount) {
            case float f when f <= DriftBoostTime[0]:
                boostType = 0;
                break;
            case float f when f <= DriftBoostTime[1]:
                boostType = 1;
                break;
            case float f when f <= DriftBoostTime[2]:
                boostType = 2;
                break;
        }
        data.rb.velocity = data.rb.velocity * DriftBoostAmount[boostType];//+ (transform.forward * 10);
        BoostVisualEffects(true);
        currentDriftBoostDuration = DriftBoostDuration[boostType];
        if (driftBoostCoroutine == null) driftBoostCoroutine = StartCoroutine(StopDriftBoost());
    }

    IEnumerator StopDriftBoost() {
        yield return new WaitForSeconds(currentDriftBoostDuration);
        BoostVisualEffects(false);
        driftBoostCoroutine = null;
    }

    void BoostVisualEffects(bool isActive) {
        if (isActive) {
            PPcontroler.Activate_deactivateDepthOfField(true);
            BoostEffectUI.alpha = 1;
            BoostParticle.SetActive(true);
            if (FOVTransition != null) StopCoroutine(FOVTransition);
            FOVTransition = StartCoroutine(FOVEffect(true));
        }
        else {
            PPcontroler.Activate_deactivateDepthOfField(false);
            BoostEffectUI.alpha = 0;
            BoostParticle.SetActive(false);
            if (FOVTransition != null) StopCoroutine(FOVTransition);
            FOVTransition = StartCoroutine(FOVEffect(false));
        }
    }

    IEnumerator FOVEffect(bool isActive) {
        float time = FOVTransitionDuration * FOVPercentageIncrease;
        float increment = (FOVinBoost - baseFOV) * FOVPercentageIncrease;
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

    void RotateVehicleDrift() {
        //turns the car depending on the player input
        if (Mathf.Abs(newVehicleDriftRotation) < data.DriftAngle && currentDriftAmount != 0) {
            newVehicleDriftRotation += Mathf.Sign(data.inputManager.HorzMov()) * DriftAngleAmount;
            data.vehicleTransform.localRotation = Quaternion.Euler(0, newVehicleDriftRotation, 0);
        }
        //returs back the car from the drift rotation
        else if (Mathf.Abs(newVehicleDriftRotation) > 0 && currentDriftAmount == 0) {
            newVehicleDriftRotation += -Mathf.Sign(newVehicleDriftRotation) * DriftAngleAmount;
            data.vehicleTransform.localRotation = Quaternion.Euler(0, newVehicleDriftRotation, 0);
        }
    }
}
