using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCarSphere : MonoBehaviour
{
    [SerializeField] private Rigidbody sphereRB;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float fowardAccleration;
    [SerializeField] private float backAcceleration;
    [SerializeField] private float turningStrength;
    [SerializeField] private Transform[] turningWheels;
    private float currentMovment;
    private bool onGround;
    [SerializeField] private float maxXVehicleTurn;
    [SerializeField] private float maxZVehicleTurn;
    [SerializeField] private float gravityForce;
    [SerializeField] private float airDrag;
    [SerializeField] private LayerMask groundLayer;
    private float baseDragValue;
    private void Awake()
    {
        sphereRB.transform.parent = null;
        baseDragValue = sphereRB.drag;
    }
    private void Update()
    {        
        InputCheck();        
        transform.position = sphereRB.position;//keeps the model with the sphere
    }
    private void FixedUpdate()
    {
        onGround = false;
        RotateCar();
        MovmentCar();

    }
    private void RotateWheels(float horzMov)
    { 
        foreach(Transform wheel in turningWheels)
        {
            wheel.rotation = Quaternion.Euler(0, horzMov * turningStrength, 0);
        }
    }
    void RotateCar()
    {
        RaycastHit currentSurface;
        //if(Physics.Raycast(transform.position, -transform.up, out currentSurface, sphereCollider.radius + .5f)) if wants to make the car not tur back
        if (Physics.Raycast(transform.position, Vector3.down, out currentSurface, sphereCollider.radius + .5f))
        {
            onGround = true;
            Quaternion rot = Quaternion.FromToRotation(transform.up, currentSurface.normal) * transform.rotation;
            if (rot.x < -maxXVehicleTurn || rot.x > maxXVehicleTurn) rot.x = maxXVehicleTurn * Mathf.Sign(rot.x);
            if (rot.z < -maxZVehicleTurn || rot.x > maxZVehicleTurn) rot.z = maxZVehicleTurn * Mathf.Sign(rot.z);
            transform.rotation = rot;//rotates the GameObject related to the current surface
        }
    }
    void MovmentCar()
    {
        if (onGround)
        {
            sphereRB.drag = baseDragValue;
            if (Mathf.Abs(currentMovment) > 0) sphereRB.AddForce(transform.forward * currentMovment);
        }
        else
        {
            sphereRB.drag = airDrag;
            sphereRB.AddForce(Vector3.up * -gravityForce);//pulls the car back to ground
        }
    }
    void InputCheck()
    {
        float horzMov = Input.GetAxis("Horizontal");
        float vertcMov = Input.GetAxis("Vertical");
        if (onGround)
        {
            if (horzMov != 0 && vertcMov != 0) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horzMov * turningStrength * Time.deltaTime, 0f));
            if (vertcMov > 0) currentMovment = fowardAccleration * vertcMov;
            else if (vertcMov < 0) currentMovment = backAcceleration * vertcMov;
            RotateWheels(horzMov);
        }
    }
}
