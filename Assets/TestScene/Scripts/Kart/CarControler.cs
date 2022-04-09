using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KartGame.KartSystems;
using Cinemachine;

public class CarControler : MonoBehaviour
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
    [System.NonSerialized] public int currentItem = -1; // -1 = nothing
    [SerializeField] private Image itemImage;
    [SerializeField] private Sprite[] powerUpImages;
    [SerializeField] private float teleportRange = 2;
    [SerializeField] private ParticleSystem teleportParticles;
    [SerializeField] private ParticleSystem invertControlsParticles;
    [SerializeField] private GameObject drillPrefab;
    public float isControlInverted = 1;
    public bool isShielded = false;
    [SerializeField] private GameObject oilPrefab;

    Rigidbody rbCar;
    private float currentDriftAmount;
    private float newVehicleDriftRotation = 0;
    private Coroutine driftBoostCoroutine = null;
    private float currentDriftBoostDuration;
    private float baseFOV;
    private Coroutine FOVTransition = null;
    // NOTE: if dosent work errase all lines with /**/ and turn on all with /***/
    [System.NonSerialized] public InputCar inputManager = null; /**/
    IInput[] m_Inputs;
    public KartGame.KartSystems.InputData Inputs { get; private set; }


    void Awake()
    {
        rbCar = GetComponent<Rigidbody>();
        m_Inputs = GetComponents<IInput>();
        inputManager = GetComponent<InputCar>();/**/
        baseFOV = cm.m_Lens.FieldOfView;
    }

    void Update()
    {
        //GatherInputs();/***/
        if (inputManager.inputData != null)
        {
            MovementInputs();
            Drift();
        }
        else Debug.LogWarning("No Active Controler Found For Car: " + this.gameObject.name);
        ChangeUi();
    }

    void FixedUpdate()
    {
        LockRotation();
    }

    void MovementInputs()
    {
        if (inputManager.VertMov() != 0) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = inputManager.VertMov() > 0 ?  inputManager.VertMov() * Velocity : inputManager.VertMov() * ReverseVelocity; /**/
        else foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = 0; /**/
        for (int i = 0; i < turningWheels; i++) WheelsScript[i].wheelCollider.steerAngle = inputManager.HorzMov() * TurningDegrees * isControlInverted;//turning the vehicle /**/
        if (inputManager.UseItem()) UseItem();
        //if (Input.GetKeyDown(KeyCode.F)) UseItem(); /***/
        //if (Inputs.Accelerate) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity; /***/
        //if (Inputs.Brake) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = Mathf.Abs(UnityEngine.Input.GetAxis("Vertical")) * -ReverseVelocity; /***/
        //if (!Inputs.Accelerate && !Inputs.Brake) foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = UnityEngine.Input.GetAxis("Vertical") * Velocity; /***/
        //for (int i = 0; i < turningWheels; i++) WheelsScript[i].wheelCollider.steerAngle = Inputs.TurnInput * TurningDegrees;//turning the vehicle /***/
        //if (Input.GetKeyDown(KeyCode.F)) UseItem() /***/
    }

    void UseItem()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        switch (currentItem)
        {
            case 0:
                rbCar.velocity = rbCar.velocity * 1.75f;
                Debug.Log(gameObject.name + "boosted speed");
                break;
            case 1:
                rbCar.velocity = new Vector3(rbCar.velocity.x, 15, rbCar.velocity.z);
                Debug.Log(gameObject.name + "Jumped");
                break;
            case 2:
                foreach (GameObject player in players) if (player != this.gameObject) StartCoroutine(InvertControl(player.GetComponent<CarControler>()));
                Debug.Log(gameObject.name + "inverted other player's control");
                break;
            case 3:
                // Teleport In Front of Other
                foreach (GameObject player in players) if (player != this.gameObject) transform.position = player.transform.position + (teleportRange * player.transform.forward);
                Debug.Log(gameObject.name + "teleported in front of the other player");
                break;
            case 4:
                // Missile
                GameObject missile = Instantiate(drillPrefab, transform.position + (transform.forward * 4), transform.rotation);
                missile.GetComponent<Rigidbody>().velocity = transform.forward * 25;
                break;
                Debug.Log(gameObject.name + "Missile'd");
            case 5:
                isShielded = true;
                // Activate shield animation
                break;
            case 6:
                // Break oponent
                foreach (GameObject player in players) if (player != this.gameObject) StartCoroutine(BreakWheels(player.GetComponent<CarControler>()));
                break;
            case 7:
                // SpawnOil
                Instantiate(oilPrefab, transform.position - (transform.forward * 4), transform.rotation);
                break;
            default:
                Debug.Log("Error generating item");
                break;
        }
        currentItem = -1;
    }

    void ChangeUi()
    {
        itemImage.sprite = powerUpImages[currentItem + 1];
    }

    IEnumerator InvertControl(CarControler target)
    {
        target.isControlInverted *= -1;

        yield return new WaitForSeconds(3);

        target.isControlInverted *= -1;
    }

    IEnumerator BreakWheels(CarControler target) {
        float normalSpeed = target.Velocity;
        target.Velocity = 0;

        yield return new WaitForSeconds(3);

        target.Velocity = normalSpeed;
    }

    void Drift()
    {
        //if (Inputs.Drift && Inputs.Accelerate && Inputs.TurnInput != 0)/***/
        //{/***/
        if (inputManager.Drift() && inputManager.VertMov() > 0 && inputManager.HorzMov() != 0)/**/
        {/**/
            if (Mathf.Abs(WheelsScript[0].wheelCollider.steerAngle) >= .01f && CheckGround())
            {
                currentDriftAmount += Time.fixedDeltaTime;
                foreach (WheelSinc wheel in WheelsScript) if (wheel.trail != null) wheel.TrailEffect(true, currentDriftAmount, DriftBoostTime);
            }
        }/**/
        //}/***/
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
        rbCar.velocity *= DriftBoostAmount[boostType];
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

        Inputs = new KartGame.KartSystems.InputData();

        // gather nonzero input from our sources
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            Inputs = m_Inputs[i].GenerateInput();

        }
    }
}
