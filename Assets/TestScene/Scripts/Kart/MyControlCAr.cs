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

    [Tooltip("For how long needs to be drifting to gain a boost, in seconds.")]
    [SerializeField] private float[] DriftBoostTime = new float[3];

    [Tooltip("the amount of force applied to the vehicle at each level of boost.")]
    [SerializeField] private float[] DriftBoostAmount = new float[3];

    [Tooltip("For how long the drift boost will last, in seconds.")]
    [SerializeField] private float[] DriftBoostDuration = new float[3];

    [Tooltip("the angle that the vehicle will turn when drifting.")]
    [SerializeField] private float DriftAngle;

    [Tooltip("how much the drift angle will change each FixedDeltaTime.")]
    [SerializeField] private float DriftAngleAmount;

    Rigidbody rigidbody;
    private float currentDriftAmount;
    private float newVehicleDriftRotation = 0;
    private Coroutine driftBoostCoroutine = null;
    private float currentDriftBoostDuration;
    private bool isInBoostEffect = false;

    IInput[] m_Inputs;

    [Header("Components")]
    [SerializeField] private Transform vehicleTransform;
    [Min(1), Tooltip("the amount of wheels that will turn, needs to be the firts elements of the WheelsScript array")]
    [SerializeField] private int turningWheels;
    [SerializeField] private WheelSinc[] WheelsScript;

    public InputData Input { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        m_Inputs = GetComponents<IInput>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        GatherInputs();
        MovmentInputs();
        Drift();
    }
    void MovmentInputs()
    {
        if (Input.Accelerate && !isInBoostEffect) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
        if (Input.Brake) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity;
        if (!Input.Accelerate && !Input.Brake) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
        for (int i = 0; i < turningWheels; i++) WheelsScript[i].wheelCollider.steerAngle = Input.TurnInput * TurningDegrees;//turning the vehicle
    }
    void Drift()
    {
        if (Input.Drift)
        {
            if (Mathf.Abs(WheelsScript[0].wheelCollider.steerAngle) >= .01f)
            {
                currentDriftAmount += Time.fixedDeltaTime;
                foreach (WheelSinc wheel in WheelsScript) if (wheel.trail != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
            }
        }
        else if (currentDriftAmount != 0)
        {
            if (currentDriftAmount >= DriftBoostTime[0] / 2f) DriftBoost();
            foreach (WheelSinc wheel in WheelsScript) if (wheel.trail != null) wheel.TrailEffect(false, currentDriftAmount, DriftBoostTime);
            currentDriftAmount = 0;
        }
        RotateVehicleDrift();
    }
    void DriftBoost()
    {
        isInBoostEffect = true;
        int boostType = 0;
        switch (currentDriftAmount)
        {
            case float f when f <= DriftBoostTime[0]:
                boostType = 0;
                foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = DriftBoostAmount[boostType];
                Debug.Log("boost 1");
                break;
            case float f when f <= DriftBoostTime[1]:
                boostType = 1;
                foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = DriftBoostAmount[boostType];
                Debug.Log("boost 2");
                break;
            case float f when f <= DriftBoostTime[2]:
                boostType = 2;
                foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = DriftBoostAmount[boostType];
                Debug.Log("boost 3");
                break;
        }
        currentDriftBoostDuration = DriftBoostDuration[boostType];
        if (driftBoostCoroutine == null) driftBoostCoroutine = StartCoroutine(StopDriftBoost());
    }
    IEnumerator StopDriftBoost()
    {
        yield return new WaitForSeconds(currentDriftBoostDuration);
        isInBoostEffect = false;
        driftBoostCoroutine = null;
    }
    void RotateVehicleDrift()
    {
        if (Mathf.Abs(newVehicleDriftRotation) < DriftAngle && currentDriftAmount != 0)
        {
            float direction = Mathf.Sign(UnityEngine.Input.GetAxis("Horizontal"));
            newVehicleDriftRotation += direction * DriftAngleAmount;
            vehicleTransform.localRotation = Quaternion.Euler(0, newVehicleDriftRotation, 0);
        }
        else if (Mathf.Abs(newVehicleDriftRotation) > 0 && currentDriftAmount == 0)
        {
            newVehicleDriftRotation += -Mathf.Sign(newVehicleDriftRotation) * DriftAngleAmount;
            vehicleTransform.localRotation = Quaternion.Euler(0, newVehicleDriftRotation, 0);
        }
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
