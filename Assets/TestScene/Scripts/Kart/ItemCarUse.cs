using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCarUse : MonoBehaviour
{
    [Header("ItemComponents")]
    [SerializeField] private Image itemImage;
    [SerializeField] private Sprite[] powerUpImages;
    [SerializeField] private ParticleSystem teleportParticles;
    [SerializeField] private ParticleSystem invertControlsParticles;
    [SerializeField] private LayerMask carsLayerMask;
    [SerializeField] private GameObject drillPrefab;
    [SerializeField] private GameObject oilPrefab;
    [SerializeField] private CanvasGroup breakImage;
    [SerializeField] private MeshRenderer[] shieldMesh = new MeshRenderer[4];
    [SerializeField] private GameObject shieldParent;

    [Header("Status")]
    [SerializeField] private float teleportRange = 2;
    [SerializeField] private float shieldAnimSpeed;
    [Tooltip("The amount of time the effect lasts")]
    [SerializeField] private float invertControlsEffectDuration;
    [SerializeField] private float invertControlsDistance;
    [Tooltip("The amount of time the effect lasts")]
    [SerializeField] private float breakEffectDuration;

    [System.NonSerialized] public bool isShielded = false;
    private Rigidbody rbCar;
    private PlayerData data;
    [System.NonSerialized] public int currentItem = -1; // -1 = nothing

    private void Awake()
    {
        data = GetComponent<PlayerData>();
        data.inputManager = GetComponent<InputCar>();
        rbCar = GetComponentInChildren<Rigidbody>();
        invertControlsParticles.gameObject.transform.localScale = new Vector3(invertControlsDistance, 1, invertControlsDistance);
    }
    private void Update()
    {
        if (data.inputManager.UseItem() && currentItem != -1) UseItem();
    }

    public void GenerateNewItem()
    {
        currentItem = Random.Range(0, 8);
        ChangeUi();
    }
    void UseItem()
    {
        switch (currentItem)
        {
            case 0://boost
                rbCar.velocity = rbCar.velocity * 1.75f;
                break;
            case 1://jump
                rbCar.velocity = new Vector3(rbCar.velocity.x, 15, rbCar.velocity.z);
                break;
            case 2://invert controls
                CheckForPlayersInRange();
                invertControlsParticles.Play();
                break;
            case 3://teleport
                GameObject successorPlayer = LapsManager.Instance.GetMySuccessorPlayer(data);
                if (successorPlayer != null)
                {
                    transform.position = successorPlayer.transform.position + (teleportRange * successorPlayer.transform.forward);
                    teleportParticles.Play();
                }
                break;
            case 4://missle
                GameObject missile = Instantiate(drillPrefab, transform.position + (transform.forward * 4), transform.rotation);
                missile.GetComponent<Rigidbody>().velocity = transform.forward * 25;
                break;
            case 5://shield
                isShielded = true;
                for (int i = 0; i < 4; i++) shieldMesh[i].enabled = isShielded;
                StartCoroutine(ShieldAnimation());
                break;
            case 6://break
                // Break oponent
                GameObject targetPlayer = LapsManager.Instance.GetMySuccessorPlayer(data);
                if (targetPlayer != null) StartCoroutine(targetPlayer.GetComponent<ItemCarUse>().BreakWheels());
                break;
            case 7://oil
                Instantiate(oilPrefab, transform.position - (transform.forward * 4), transform.rotation);
                break;
            default:
                Debug.Log("Error generating item");
                break;
        }
        currentItem = -1;
        ChangeUi();
    }

    void ChangeUi()
    {
        if (currentItem + 1 == 0) itemImage.enabled = false;
        else
        {
            itemImage.sprite = powerUpImages[currentItem];
            itemImage.enabled = true;
        }
    }

    IEnumerator ShieldAnimation()
    {
        for (int i = 0; i < shieldMesh.Length; i++) shieldMesh[i].enabled = isShielded;
        while (isShielded)
        {
            shieldParent.transform.Rotate(0, 0, 90 * shieldAnimSpeed, Space.Self);
            yield return new WaitForSeconds(shieldAnimSpeed);
        }
        for (int i = 0; i < shieldMesh.Length; i++) shieldMesh[i].enabled = isShielded;
    }

    void CheckForPlayersInRange()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, invertControlsDistance, transform.forward, .1f, carsLayerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) StartCoroutine(hit.collider.gameObject.GetComponent<ItemCarUse>().InvertControl());
        }
    }

    public IEnumerator InvertControl()
    {
        data.inputManager.invertControls = -1;
        invertControlsParticles.Play();

        yield return new WaitForSeconds(invertControlsEffectDuration);

        data.inputManager.invertControls = 1;
    }

    IEnumerator BreakWheels()
    {
        rbCar.velocity = Vector3.zero;
        breakImage.alpha = 1;

        yield return new WaitForSeconds(breakEffectDuration);

        breakImage.alpha = 0;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, invertControlsDistance);
    }
}
