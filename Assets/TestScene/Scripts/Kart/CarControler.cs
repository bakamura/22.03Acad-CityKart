using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("ItemBoost")]
    [SerializeField] private Image itemImage;
    [System.NonSerialized] public int currentItem = -1; // -1 = nothing
    [SerializeField] private Sprite[] powerUpImages;
    [SerializeField] private float teleportRange = 2;
    [SerializeField] private ParticleSystem teleportParticles;
    [SerializeField] private ParticleSystem invertControlsParticles;
    [SerializeField] private GameObject drillPrefab;
    public float isControlInverted = 1;
    public bool isShielded = false;
    [SerializeField] private GameObject oilPrefab;
    [SerializeField] private CanvasGroup breakImage;
    [SerializeField] private MeshRenderer[] shieldMesh = new MeshRenderer[4];
    [SerializeField] private GameObject shieldParent;

    private PlayerData data;

    void Awake()
    {
        rbCar = GetComponent<Rigidbody>();
        data = GetComponent<PlayerData>();
        data.inputManager = GetComponent<InputCar>();/**/
        baseFOV = cm.m_Lens.FieldOfView;
        CarStatusManager.CheckCarStatus(this);
    }

    void Update()
    {
        if (data.inputManager.Drift())Debug.Log("drift");
        if (data.inputManager.inputData != null)
        {
            MovementInputs();
            Drift();
        }
        else Debug.LogWarning("No Active Controler Found For Car: " + this.gameObject.name);
        ChangeUi();
        for(int i = 0; i < 4; i++) shieldMesh[i].enabled = isShielded;
        if (isShielded) shieldParent.transform.Rotate(0, 0, 90 * Time.deltaTime, Space.Self);
    }

    void FixedUpdate()
    {
        LockRotation();
    }

    void MovementInputs()
    {
        foreach (WheelSinc wheel in WheelsScript) wheel.wheelCollider.motorTorque = data.inputManager.VertMov() > 0 ? data.inputManager.VertMov() * Velocity : data.inputManager.VertMov() * ReverseVelocity; /**/
        for (int i = 0; i < turningWheels; i++) WheelsScript[i].wheelCollider.steerAngle = data.inputManager.HorzMov() * TurningDegrees * isControlInverted;//turning the vehicle /**/
        if (data.inputManager.UseItem() && currentItem != -1) UseItem();
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
                foreach (GameObject player in players) if (player != this.gameObject)
                    {
                        if (player.GetComponent<CarControler>().isShielded) player.GetComponent<CarControler>().isShielded = false;
                        else StartCoroutine(InvertControl(player.GetComponent<CarControler>()));
                    }
                Debug.Log(gameObject.name + "inverted other player's control");
                break;
            case 3:
                // Teleport In Front of Other
                foreach (GameObject player in players) if (player != this.gameObject)
                    {
                        if (player.GetComponent<CarControler>().isShielded) player.GetComponent<CarControler>().isShielded = false;
                        else transform.position = player.transform.position + (teleportRange * player.transform.forward);
                    }
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
        if (currentItem + 1 == 0) itemImage.enabled = false;
        else
        {
            itemImage.sprite = powerUpImages[currentItem + 1];
            itemImage.enabled = true;
        }
    }

    IEnumerator InvertControl(CarControler target)
    {
        target.isControlInverted *= -1;
        target.invertControlsParticles.Play(); // 

        yield return new WaitForSeconds(3);

        target.isControlInverted *= -1;
    }

    IEnumerator BreakWheels(CarControler target)
    {
        float normalSpeed = target.Velocity;
        target.Velocity = 0;
        target.breakImage.alpha = 1;

        yield return new WaitForSeconds(3);

        target.Velocity = normalSpeed;
        target.breakImage.alpha = 0;
    }



    void Drift()
    {
        if (data.inputManager.Drift() && data.inputManager.VertMov() > 0 && data.inputManager.HorzMov() != 0 && rbCar.velocity.magnitude > 1f)/**/
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
}
