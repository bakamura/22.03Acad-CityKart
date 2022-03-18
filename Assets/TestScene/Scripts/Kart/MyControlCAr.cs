using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;
public class MyControlCAr : MonoBehaviour
{
    [Header("Movement Settings")]
    [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
    public float TopSpeed;

    [Tooltip("How quickly the kart reaches top speed.")]
    public float Acceleration;

    [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
    public float ReverseSpeed;

    [Tooltip("How quickly the kart slows down when the brake is applied.")]
    public float Braking;
    
    Rigidbody rigidbody;

    private bool isDrifting = false;

    IInput[] m_Inputs;
    public WheelCollider wheelColliderFE;
    public WheelCollider wheelColliderFD;
    public WheelCollider wheelColliderTD;
    public WheelCollider wheelColliderTE;

    public InputData Input { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        m_Inputs = GetComponents<IInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        GatherInputs();
        if (Input.Accelerate)
        {
            wheelColliderFE.motorTorque = 1000;
            wheelColliderFD.motorTorque = 1000;
        }
        else
        {
            wheelColliderFE.motorTorque = 000;
            wheelColliderFD.motorTorque = 000;
        }
        if (Input.Brake)
        {
            
            wheelColliderFE.brakeTorque = 300;
            wheelColliderFD.brakeTorque = 300;
            wheelColliderTD.brakeTorque = 900;
            wheelColliderTE.brakeTorque = 900;
        }
        else
        {
            wheelColliderFE.brakeTorque = 00;
            wheelColliderFD.brakeTorque = 00;
            wheelColliderTD.brakeTorque = 00;
            wheelColliderTE.brakeTorque = 00;
        }

        wheelColliderFD.steerAngle = Input.TurnInput * 30;
        wheelColliderFE.steerAngle = Input.TurnInput * 30;
    }

    void GatherInputs()
    {
        // reset input

        Input = new InputData();
       

        // gather nonzero input from our sources
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            Input = m_Inputs[i].GenerateInput();
           
        }
    }
}
