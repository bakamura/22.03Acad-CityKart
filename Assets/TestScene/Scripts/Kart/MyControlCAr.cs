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

    [SerializeField] private bool canBrutllyStop;

    [Tooltip("How quickly the kart slows down when no keys are being pressed.")]
    public float Stopping = 2f;

    [Tooltip("How many degrees the wheels can turn in the Y Axis.")]
    [SerializeField] private float TurningDegrees;

    Rigidbody rigidbody;
    private float baseDragValue;
    private float currentDrift;
    private float currentActiveBoostDrift;

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
        baseDragValue = rigidbody.drag;
        m_Inputs = GetComponents<IInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (Input.Drift)
        //{
        //    currentDrift += .02f;
        //}
        //else
        //{
        //
        //}

        GatherInputs();
        if (Input.Accelerate)
        {
            wheelColliderFE.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Acceleration;
            wheelColliderFD.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Acceleration;
            //if (wheelColliderFE.motorTorque < TopSpeed && wheelColliderFD.motorTorque < TopSpeed)
            //{
            //    if (canBrutllyStop) rigidbody.drag = baseDragValue;
            //    float addVelocity =  Acceleration;
            //    wheelColliderFE.motorTorque += addVelocity;
            //    wheelColliderFD.motorTorque += addVelocity;
            //}
            //else
            //{
            //    wheelColliderFE.motorTorque = TopSpeed;
            //    wheelColliderFD.motorTorque = TopSpeed;
            //}
        }
        if (Input.Brake)
        {
            wheelColliderFE.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseSpeed;
            wheelColliderFD.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseSpeed;
            //    if (Mathf.Abs(wheelColliderFE.motorTorque) < ReverseSpeed && Mathf.Abs(wheelColliderFD.motorTorque) <  ReverseSpeed)
            //    {
            //        if (canBrutllyStop) rigidbody.drag = baseDragValue;
            //        float addVelocity = Braking;
            //        wheelColliderFE.motorTorque -= addVelocity;
            //        wheelColliderFD.motorTorque -= addVelocity;
            //    }
            //    else
            //    {
            //        wheelColliderFE.motorTorque = -1*ReverseSpeed;
            //        wheelColliderFD.motorTorque = -1*ReverseSpeed;
            //    }
        }
        //if (!Input.Accelerate && !Input.Brake)
        //{
        //    wheelColliderFE.motorTorque = 0;
        //    wheelColliderFD.motorTorque = 0;
        //    wheelColliderTE.motorTorque = 0;
        //    wheelColliderTD.motorTorque = 0;
        //    if (canBrutllyStop) rigidbody.drag = Stopping;
        //}
        
        wheelColliderFD.steerAngle = Input.TurnInput * TurningDegrees;
        wheelColliderFE.steerAngle = Input.TurnInput * TurningDegrees;
    }

    //IEnumerator DriftBoost()
    //{
    //    this.TopSpeed
    //}
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
