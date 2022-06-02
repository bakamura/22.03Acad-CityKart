using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Drift : MonoBehaviour {
    [Header("Data")]
    [SerializeField] private PlayerData data;
    [SerializeField] private BoostEffect boostEffect;
    private float newVehicleDriftRotation = 0;
    private float currentDriftAmount;

    [Tooltip("the velocity of the vehicle at each level of boost. Multiplicative")]
    [SerializeField] private float[] DriftBoostAmount = new float[3];

    [Tooltip("For how long the drift boost will last, in seconds.")]
    [SerializeField] private float[] DriftBoostDuration = new float[3];

    [Tooltip("For how long needs to be drifting to gain a boost, in seconds.")]
    [SerializeField] private float[] DriftBoostTime = new float[3];

    [Tooltip("how much the drift angle will change each FixedDeltaTime.")]
    [SerializeField] private float DriftAngleAmount;

    private void Update() {
        StartDrift();
    }

    public void StartDrift() {
        if (data.inputManager.Drift() && data.inputManager.VertMov() > 0 && data.inputManager.HorzMov() != 0 && data.rb.velocity.magnitude > 1f) {
            currentDriftAmount += Time.deltaTime;
            foreach (WheelSinc wheel in data.WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
        }
        else if (currentDriftAmount != 0 && !data.inputManager.Drift()) {
            if (currentDriftAmount >= DriftBoostTime[0]) DriftBoost();
            foreach (WheelSinc wheel in data.WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(false, currentDriftAmount, DriftBoostTime);
            currentDriftAmount = 0;
        }
        RotateVehicleDrift();
    }
    void DriftBoost() {
        var boostType = currentDriftAmount switch {
            float f when f >= DriftBoostTime[1] && f < DriftBoostTime[2] => 1,
            float f when f >= DriftBoostTime[2] => 2,
            _ => 0,
        };
        data.rb.velocity *= DriftBoostAmount[boostType];
        if (boostEffect)boostEffect.BoostVisualEffects(true, DriftBoostDuration[boostType]);
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
