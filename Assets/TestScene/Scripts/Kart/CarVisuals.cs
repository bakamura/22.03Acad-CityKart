using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarVisuals : MonoBehaviour
{
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

    private void Awake()
    {
        baseFOV = cm.m_Lens.FieldOfView;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
