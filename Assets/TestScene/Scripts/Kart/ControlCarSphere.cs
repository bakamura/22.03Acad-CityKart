using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCarSphere : MonoBehaviour
{
    [Header("Vehicle Handling")]
    [Min(0f), Tooltip("The force multiplier pushing down the car.")]
    [SerializeField] private float gravityForce;
    [Min(0f), Tooltip("How much the air makes the car lose momentum.")]
    [SerializeField] private float airDrag;
    [Header("Visuals")]
    [Min(0f), Tooltip("How many degrees the car Mesh can turn in the X Axis.")]
    [SerializeField] private float maxXMeshTurn;
    [Min(0f), Tooltip("How many degrees the car Mesh can turn in the Z Axis.")]
    [SerializeField] private float maxZMeshTurn;
    [Header("Base Values")]
    [SerializeField] private SphereCollider sphereCollider;
    //[SerializeField] private Transform[] turningWheels;
    [SerializeField] private Rigidbody rbCar;
    private float baseDragValue;
    private float currentMovment;
    private PlayerData data;
    private void Awake()
    {
        data = GetComponent<PlayerData>();
        data.inputManager = GetComponent<InputCar>();
        data.rb = rbCar;
        data.rb.transform.parent = null;
        baseDragValue = data.rb.drag;
    }
    private void Update()
    {        
        InputCheck();        
        transform.position = data.rb.position;//keeps the model with the sphere
    }
    private void FixedUpdate()
    {
        data.isGrounded = false;
        RotateCar();
        MovmentCar();

    }
    //private void RotateWheels(float horzMov)
    //{ 
    //    foreach(Transform wheel in turningWheels)
    //    {
    //        wheel.rotation = Quaternion.Euler(0, horzMov * data.TurningDegrees, 0);
    //    }
    //}
    void RotateCar()
    {
        RaycastHit currentSurface;
        //if(Physics.Raycast(transform.position, -transform.up, out currentSurface, sphereCollider.radius + .5f)) if wants to make the car not tur back
        if (Physics.Raycast(transform.position, Vector3.down, out currentSurface, sphereCollider.radius + .5f))
        {
            data.isGrounded = true;
            Quaternion rot = Quaternion.FromToRotation(transform.up, currentSurface.normal) * transform.rotation;
            if (rot.x < -maxXMeshTurn || rot.x > maxXMeshTurn) rot.x = maxXMeshTurn * Mathf.Sign(rot.x);
            if (rot.z < -maxZMeshTurn || rot.x > maxZMeshTurn) rot.z = maxZMeshTurn * Mathf.Sign(rot.z);
            transform.rotation = rot;//rotates the GameObject related to the current surface
        }
    }
    void MovmentCar()
    {
        if (data.isGrounded) {
            data.rb.drag = baseDragValue;
            if (Mathf.Abs(currentMovment) > 0) data.rb.AddForce(transform.forward * currentMovment);
        }
        else
        {
            data.rb.drag = airDrag;
            data.rb.AddForce(Vector3.up * -gravityForce);//pulls the car back to ground
        }
    }
    void InputCheck()
    {
        float horzMov = data.inputManager.HorzMov();//Input.GetAxis("Horizontal");
        float vertcMov = data.inputManager.VertMov();//Input.GetAxis("Vertical");
        if (data.isGrounded) {
            if (horzMov != 0 && vertcMov != 0) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, horzMov * data.TurningDegrees * Time.deltaTime, 0f));
            if (vertcMov > 0) currentMovment = data.Velocity * vertcMov;
            else if (vertcMov < 0) currentMovment = data.ReverseVelocity * vertcMov;
            //RotateWheels(horzMov);
        }
    }
}
