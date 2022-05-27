using UnityEngine;
using Cinemachine;

public class PlayerData : MonoBehaviour
{
    [Header("CarComponents")]
    public Transform vehicleTransform;
    public CinemachineVirtualCamera cm;
    [Tooltip("first the cinemachine camera, then the UI camera")]public Camera[] cameras = new Camera[2];
    public CanvasGroup UI;
    [Tooltip("The panel inside the canvas")]public RectTransform screenSize;
    [Min(1), Tooltip("the amount of wheels that will turn, needs to be the firts elements of the WheelsScript array")]
    public int turningWheels;
    public WheelSinc[] WheelsScript;

    [Header("CarStatus")]
    [Range(GameManager.MinVelocity, GameManager.MaxVelocity), Tooltip("How quickly the kart reaches top speed.")]
    public float Velocity;
    [Range(GameManager.MinVelocity, GameManager.MaxVelocity), Tooltip("Top speed attainable when moving backward.")]
    public float ReverseVelocity;
    [Range(GameManager.MinHandling, GameManager.MaxHandling), Tooltip("How many degrees the wheels can turn in the Y Axis.")]
    public float TurningDegrees;
    [Range(GameManager.MinDrift, GameManager.MaxDrift), Tooltip("the angle that the vehicle will turn when drifting.")]
    public float DriftAngle;
    [HideInInspector] public InputCar inputManager = null;
    [HideInInspector] public int CarID;
    [HideInInspector] public int PlayerScore;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool isGrounded;
    private ControlCarWheel carWheeels;
    private ControlCarSphere carSphere;

    private void Awake() {
        carSphere = GetComponent<ControlCarSphere>();
        carWheeels = GetComponent<ControlCarWheel>();
        GameManager.CheckCarStatus(this);
    }

    public void PodiumSetUp() {
        rb.useGravity = false;
        cm.gameObject.SetActive(false);
        foreach (Camera cm in cameras) cm.gameObject.SetActive(false);
        UI.gameObject.SetActive(false);
        foreach (WheelSinc wheel in WheelsScript) wheel.enabled = false;
        if (carWheeels) carWheeels.enabled = false;        
        else if(carSphere) carSphere.enabled = false;
    }
}
