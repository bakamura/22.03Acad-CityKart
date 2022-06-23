using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCarWheel : MonoBehaviour {
    [Header("Components")]
    private PlayerData data;
    void Awake() {
        data = GetComponent<PlayerData>();
    }

    void Update() {
        data.isGrounded = CheckGround();
        if (data.inputManager.inputData != null) MovementInputs();
        else Debug.LogWarning("No Active Controler Found For Car: " + this.gameObject.name);
    }

    void FixedUpdate() {
        LockRotation();
    }

    void MovementInputs() {
        foreach (WheelSinc wheel in data.WheelsScript) wheel.wheelCollider.motorTorque = data.inputManager.VertMov() > 0 ? data.inputManager.VertMov() * data.Velocity : data.inputManager.VertMov() * data.ReverseVelocity;
        for (int i = 0; i < data.turningWheels; i++) {
            data.WheelsScript[i].wheelCollider.steerAngle = data.inputManager.HorzMov() * data.TurningDegrees;//turning the vehicle
            data.WheelsScript[i].WheelMovmentVisual();
        }
        data.CurrentMovment = data.rb.velocity.magnitude;
    }

    public bool CheckGround() {
        foreach (WheelSinc wheels in data.WheelsScript) if (wheels.wheelCollider.isGrounded) return true;
        //if (Mathf.Abs(transform.rotation.eulerAngles.x) >= 10f || Mathf.Abs(transform.rotation.eulerAngles.z) >= 10f) transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        return false;
    }

    void LockRotation() {
        if (!data.isGrounded) data.rb.freezeRotation = true;
        else data.rb.freezeRotation = false;
    }
}
