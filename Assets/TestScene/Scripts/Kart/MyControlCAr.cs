using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;
public class MyControlCAr : MonoBehaviour
{
    [Header("Movement Settings")]

    [Tooltip("How quickly the kart reaches top speed.")]
    public float Velocity;

    [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
    public float ReverseVelocity;

    [Tooltip("How many degrees the wheels can turn in the Y Axis.")]
    [SerializeField] private float TurningDegrees;

    Rigidbody rigidbody;

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
            wheelColliderFE.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
            wheelColliderFD.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
            wheelColliderTE.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
            wheelColliderTD.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
        }
        if (Input.Brake)
        {
            wheelColliderFE.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity;
            wheelColliderFD.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity;
            wheelColliderTE.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity;
            wheelColliderTD.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity;
        }
        if (!Input.Accelerate && !Input.Brake)
        {
            wheelColliderFE.motorTorque = 0;
            wheelColliderFD.motorTorque = 0;
            wheelColliderTE.motorTorque = 0;
            wheelColliderTD.motorTorque = 0;
        }
        wheelColliderFE.steerAngle = Input.TurnInput * TurningDegrees;
        wheelColliderFD.steerAngle = Input.TurnInput * TurningDegrees;
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
