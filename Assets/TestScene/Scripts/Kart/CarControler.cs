using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarControler : MonoBehaviour
{

    [Header("Movement Settings")]

    [Tooltip("How quickly the kart reaches top speed.")]
    public float Velocity;

    [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
    public float ReverseVelocity;

    [Tooltip("How many degrees the wheels can turn in the Y Axis.")]
    public float TurningDegrees;

    [Tooltip("For how long needs to be drifting to gain a boost, in seconds.")]
    [SerializeField] private float[] DriftBoostTime = new float[3];

    [Tooltip("the velocity of the vehicle at each level of boost.")]
    [SerializeField] private float[] DriftBoostAmount = new float[3];

    [Tooltip("For how long the drift boost will last, in seconds.")]
    [SerializeField] private float[] DriftBoostDuration = new float[3];

    [Tooltip("the angle that the vehicle will turn when drifting.")]
    public float DriftAngle;

    [Tooltip("how much the drift angle will change each FixedDeltaTime.")]
    [SerializeField] private float DriftAngleAmount;

    [Header("Components")]
    [SerializeField] private Transform vehicleTransform;
    [Min(1), Tooltip("the amount of wheels that will turn, needs to be the firts elements of the WheelsScript array")]
    [SerializeField] private int turningWheels;
    [SerializeField] private WheelSinc[] WheelsScript;
    [SerializeField] private GameObject masterCanvas;
    [SerializeField] private GameObject mainCam;
    private float currentDriftAmount;
    private Coroutine driftBoostCoroutine = null;
    private float currentDriftBoostDuration;
    private Rigidbody rbCar;

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
    private float baseFOV;
    private float newVehicleDriftRotation = 0;
    private Coroutine FOVTransition = null;

    private PlayerData data;

    void Awake()
    {
        rbCar = GetComponent<Rigidbody>();
        data = GetComponent<PlayerData>();
        data.inputManager = GetComponent<InputCar>();
        baseFOV = cm.m_Lens.FieldOfView;
        CarStatusManager.CheckCarStatus(this);
    }

    void Update()
    {
        if (data.inputManager.inputData != null)
        {
            MovementInputs();
            Drift();
        }
        else Debug.LogWarning("No Active Controler Found For Car: " + this.gameObject.name);
    }

    void FixedUpdate()
    {
        LockRotation();
    }

    void MovementInputs()
    {
        foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = data.inputManager.VertMov() > 0 ? data.inputManager.VertMov() * Velocity : data.inputManager.VertMov() * ReverseVelocity; /**/
        for (int i = 0; i < turningWheels; i++) WheelsScript[i].wheelCollider.steerAngle = data.inputManager.HorzMov() * TurningDegrees;//turning the vehicle
    }

    void Drift()
    {
        if (data.inputManager.Drift() && data.inputManager.VertMov() > 0 && data.inputManager.HorzMov() != 0 && rbCar.velocity.magnitude > 1f)
        {
            if (Mathf.Abs(WheelsScript[0].wheelCollider.steerAngle) >= .01f && CheckGround())
            {
                foreach (WheelSinc wheel in WheelsScript) if (wheel.trail != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
                if (Mathf.Abs(WheelsScript[0].wheelCollider.steerAngle) >= .01f && CheckGround())
                {
                    currentDriftAmount += Time.deltaTime;
                    foreach (WheelSinc wheel in WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
                }
            }
        }
        else if (currentDriftAmount != 0)
        {
            if (currentDriftAmount >= DriftBoostTime[0] / 2f) DriftBoost();
            foreach (WheelSinc wheel in WheelsScript) if (wheel.trail != null && wheel.driftParticle != null) wheel.TrailEffect(false, currentDriftAmount, DriftBoostTime);
            currentDriftAmount = 0;
        }
        RotateVehicleDrift();
    }

    bool CheckGround()
    {
        foreach (WheelSinc wheels in WheelsScript) if (wheels.wheelCollider.isGrounded) return true;
        if (Mathf.Abs(transform.rotation.eulerAngles.x) >= 10f || Mathf.Abs(transform.rotation.eulerAngles.z) >= 10f) transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        return false;
    }

    void LockRotation()
    {
        if (!CheckGround()) rbCar.freezeRotation = true;
        else rbCar.freezeRotation = false;
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
        rbCar.velocity = rbCar.velocity * DriftBoostAmount[boostType] + (transform.forward * 10);
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
        //turns the car depending on the player input
        if (Mathf.Abs(newVehicleDriftRotation) < DriftAngle && currentDriftAmount != 0)
        {
            newVehicleDriftRotation += Mathf.Sign(data.inputManager.HorzMov()) * DriftAngleAmount;
            vehicleTransform.localRotation = Quaternion.Euler(0, newVehicleDriftRotation, 0);
        }
        //returs back the car from the drift rotation
        else if (Mathf.Abs(newVehicleDriftRotation) > 0 && currentDriftAmount == 0)
        {
            newVehicleDriftRotation += -Mathf.Sign(newVehicleDriftRotation) * DriftAngleAmount;
            vehicleTransform.localRotation = Quaternion.Euler(0, newVehicleDriftRotation, 0);
        }
    }

    public void PodiumSetUp()
    {
        rbCar.useGravity = false;
        cm.gameObject.SetActive(false);
        mainCam.SetActive(false);
        masterCanvas.SetActive(false);
        foreach (WheelSinc wheel in WheelsScript) wheel.enabled = false;
        this.enabled = false;
    }
}
