using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCarSphere : MonoBehaviour
{
    [SerializeField] private Rigidbody sphereRB;
    [SerializeField] private float fowardAccleration;
    [SerializeField] private float backAcceleration;
    [SerializeField] private float turningStrength;
    [SerializeField] private Transform[] turningWheels;
    private float currentMovment;
    private bool onGround;
    [SerializeField] private float gravityForce;
    [SerializeField] private float airDrag;
    [SerializeField] private float rayCastDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform rayCastPoint;
    private float baseDragValue;
    private void Awake()
    {
        sphereRB.transform.parent = null;
        baseDragValue = sphereRB.drag;
    }
    private void Update()
    {
        float horzMov = Input.GetAxis("Horizontal");
        float vertcMov = Input.GetAxis("Vertical");
        if (onGround)
        {
            if (horzMov != 0 && vertcMov != 0) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horzMov * turningStrength * Time.deltaTime, 0f));
            if (vertcMov > 0 ) currentMovment = fowardAccleration * vertcMov;
            else if (vertcMov < 0) currentMovment = backAcceleration * vertcMov;
            RotateWheels(horzMov);
        }
        transform.position = sphereRB.position;//keeps the model with the sphere
    }
    private void FixedUpdate()
    {
        onGround = false;
        RaycastHit hit;
        if (Physics.Raycast(rayCastPoint.position, -transform.up, out hit, rayCastDistance, groundLayer))
        {
            onGround = true;
        }
        if (onGround)
        {
            sphereRB.drag = baseDragValue;
            RotateCar(hit);
            if (Mathf.Abs(currentMovment) > 0) sphereRB.AddForce(transform.forward * currentMovment);
        }
        else
        {
            sphereRB.drag = airDrag;
            sphereRB.AddForce(Vector3.up * -gravityForce);//pulls the car back to ground
        }
    }
    private void RotateWheels(float horzMov)
    { 
        foreach(Transform wheel in turningWheels)
        {
            wheel.rotation = Quaternion.Euler(wheel.localRotation.eulerAngles.x, horzMov * 30, wheel.localRotation.eulerAngles.z);
        }
    }
    void RotateCar(RaycastHit currentSurface)
    {
        transform.rotation = Quaternion.FromToRotation(transform.up, currentSurface.normal) * transform.rotation;//rotates the GameObject related to the current surface
    }
}
