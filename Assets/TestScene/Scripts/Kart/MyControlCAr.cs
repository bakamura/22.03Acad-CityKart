using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;
using Cinemachine;
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

    [Tooltip("the velocity of the vehicle at each level of boost.")]
    [SerializeField] private float[] DriftBoostAmount = new float[3];

    [Tooltip("For how long the drift boost will last, in seconds.")]
    [SerializeField] private float[] DriftBoostDuration = new float[3];

    [Tooltip("the angle that the vehicle will turn when drifting.")]
    [SerializeField] private float DriftAngle;

    [Tooltip("how much the drift angle will change each FixedDeltaTime.")]
    [SerializeField] private float DriftAngleAmount;

    [Header("Components")]
    [SerializeField] private Transform vehicleTransform;
    [Min(1), Tooltip("the amount of wheels that will turn, needs to be the firts elements of the WheelsScript array")]
    [SerializeField] private int turningWheels;
    [SerializeField] private WheelSinc[] WheelsScript;

    [Header("Visuals")]
    [SerializeField] private CinemachineVirtualCamera cm;
    [SerializeField] private GameObject BoostParticle;
    [Tooltip("How wide the FOV will be when in boost effects")]
    [SerializeField] private int FOVinBoost;
    [SerializeField] private CanvasGroup UI;
    [SerializeField] private PostProcessControler PPcontroler;
    [Min(.01f), Tooltip("How long the transition will take, in seconds")]
    [SerializeField] private float FOVTransitionDuration;
    [Range(.01f, 1f), Tooltip("How much the FOV will change per tick, in percentage")]
    [SerializeField] private float FOVPercentageIncrease;

    [Header("ItemBoost")]
    [System.NonSerialized] public int currentItem = 0; // 0 = nothing

    Rigidbody rigidbody;
    private float currentDriftAmount;
    private float newVehicleDriftRotation = 0;
    private Coroutine driftBoostCoroutine = null;
    private float currentDriftBoostDuration;
    private float baseFOV;
    private Coroutine FOVTransition = null;
    IInput[] m_Inputs;
    public InputData Inputs { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        m_Inputs = GetComponents<IInput>();
        baseFOV = cm.m_Lens.FieldOfView;
    }
    // Update is called once per frame
    void Update() {
        MovementInputs();
    }

    void FixedUpdate()
    {
        LockRotation();
        GatherInputs();
        Drift();
    }
    void MovementInputs()
    {
        if (Inputs.Accelerate) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
        if (Inputs.Brake) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity;
        if (!Inputs.Accelerate && !Inputs.Brake) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity;
        for (int i = 0; i < turningWheels; i++) WheelsScript[i].wheelCollider.steerAngle = Inputs.TurnInput * TurningDegrees;//turning the vehicle
        if (Input.GetKeyDown(KeyCode.F)) UseItem();
    }

    void UseItem() {
        switch (currentItem) {
            case 0:
                // Speed boost
                rigidbody.velocity = rigidbody.velocity * 1.5f;
                Debug.Log("Used Item " + currentItem);
                return;
            case 1:
                // ???
                Debug.Log("Used Item " + currentItem);
                return;
            case 2:
                // ???
                Debug.Log("Used Item " + currentItem);
                return;
            case 3:
                // ???
                Debug.Log("Used Item " + currentItem);
                return;
            default:
                Debug.Log("Error generating item");
                return;
        }
        currentItem = 0;
    }

        void Drift()
    {
        if (Inputs.Drift && Inputs.Accelerate)
        {
            if (Mathf.Abs(WheelsScript[0].wheelCollider.steerAngle) >= .01f && CheckGround())
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
    bool CheckGround()
    {
        foreach (WheelSinc wheels in WheelsScript) if (wheels.wheelCollider.isGrounded) return true;
        return false;
    }
    void LockRotation()
    {
        if (!CheckGround()) rigidbody.freezeRotation = true;
        else rigidbody.freezeRotation = false;
    }
    void DriftBoost()
    {
        int boostType = 0;
        switch (currentDriftAmount)
        {
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
        rigidbody.velocity *= DriftBoostAmount[boostType];
        BoostVisualEffects(true);
        currentDriftBoostDuration = DriftBoostDuration[boostType];
        if (driftBoostCoroutine == null) driftBoostCoroutine = StartCoroutine(StopDriftBoost());
    }
    IEnumerator StopDriftBoost()
    {
        yield return new WaitForSeconds(currentDriftBoostDuration);
        BoostVisualEffects(false);
        driftBoostCoroutine = null;
    }
    void BoostVisualEffects(bool isActive)
    {
        if (isActive)
        {
            PPcontroler.Activate_deactivateDepthOfField(true);
            UI.alpha = 1;
            BoostParticle.SetActive(true);
            if (FOVTransition != null) StopCoroutine(FOVTransition);
            FOVTransition = StartCoroutine(FOVEffect(true));
        }
        else
        {
            PPcontroler.Activate_deactivateDepthOfField(false);
            UI.alpha = 0;
            BoostParticle.SetActive(false);
            if (FOVTransition != null) StopCoroutine(FOVTransition);
            FOVTransition = StartCoroutine(FOVEffect(false));
        }
    }
    IEnumerator FOVEffect(bool isActive)
    {
        float time = FOVTransitionDuration * FOVPercentageIncrease;
        float increment = (FOVinBoost - baseFOV) * FOVPercentageIncrease;
        //float time = FOVTransitionDuration / FOVFrameAmount;
        //float increment = (FOVinBoost - baseFOV) * time;
        if (isActive)
        {
            while (cm.m_Lens.FieldOfView < FOVinBoost)
            {
                cm.m_Lens.FieldOfView += increment;
                yield return new WaitForSeconds(time);
            }
        }
        else
        {
            while (cm.m_Lens.FieldOfView > baseFOV)
            {
                cm.m_Lens.FieldOfView -= increment;
                yield return new WaitForSeconds(time);
            }
        }
        FOVTransition = null;
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

        Inputs = new InputData();


        // gather nonzero input from our sources
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            Inputs = m_Inputs[i].GenerateInput();

        }
    }
}
