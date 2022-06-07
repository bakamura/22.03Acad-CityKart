using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCarUse : MonoBehaviour {
    [Header("ItemComponents")]
    [SerializeField] private PlayerData data;
    [SerializeField] private Image itemImage;
    [SerializeField] private Sprite[] powerUpImages;
    [SerializeField] private ParticleSystem teleportParticles;
    [SerializeField] private ParticleSystem invertControlsParticles;
    [SerializeField] private LayerMask carsLayerMask;
    [SerializeField] private GameObject drillPrefab;
    [SerializeField] private GameObject oilPrefab;
    [SerializeField] private CanvasGroup breakImage;
    [SerializeField] private GameObject shieldParent;
    [SerializeField] private Transform oilParent;
    [SerializeField] private Transform drillParent;
    [SerializeField] private Transform kartPivot;//used to make instantiated objects face the right way

    private MeshRenderer[] shieldMesh = new MeshRenderer[4];
    [HideInInspector] public bool isShielded = false;
    [HideInInspector] public int currentItem = -1; // -1 = nothing

    //NOTE: to see stats for each PowerUp Go To GameManager

    private void Awake() {
        shieldMesh = shieldParent.GetComponentsInChildren<MeshRenderer>();
        invertControlsParticles.gameObject.transform.localScale = new Vector3(GameManager.invertControlsDistance, 1, GameManager.invertControlsDistance);
        if (!data) Debug.LogError("the ItemCarUse Script in" + this.gameObject.name + "needs the PlayerData in the inspector");
        if (GameManager.OilPoolsList == null) {
            GameManager.OilPoolsList = new List<IObjectPollingManager>();
            GameManager.DrillList = new List<IObjectPollingManager>();
        }
    }
    private void Update() {
        if (data) if (data.inputManager.UseItem() && currentItem != -1) UseItem();
    }

    public void GenerateNewItem() {
        currentItem = Random.Range(6, 7/*powerUpImages.Length*/);
        ChangeUi();
    }
    void UseItem() {
        switch (currentItem) {
            case 0://boost
                data.rb.velocity *= GameManager.boostForce;
                break;
            case 1://jump
                data.rb.velocity = new Vector3(data.rb.velocity.x, GameManager.jumpForce, data.rb.velocity.z);
                break;
            case 2://invert controls
                CheckForPlayersInRange();
                invertControlsParticles.Play();
                break;
            case 3://teleport
                GameObject successorPlayer = LapsManager.Instance.GetMySuccessorPlayer(data);
                if (successorPlayer != null) {
                    transform.position = successorPlayer.transform.position + (GameManager.teleportRange * successorPlayer.transform.forward);
                    teleportParticles.Play();
                }
                break;
            case 4:// Break oponent
                GameObject targetPlayer = LapsManager.Instance.GetMySuccessorPlayer(data);
                if (targetPlayer != null) StartCoroutine(targetPlayer.GetComponent<ItemCarUse>().BreakWheels());                
                break;
            case 5://shield
                isShielded = true;
                for (int i = 0; i < 4; i++) shieldMesh[i].enabled = isShielded;
                StartCoroutine(ShieldAnimation());
                break;
            case 6://missle
                ItemPrefabObjPolling(drillPrefab, GameManager.DrillList, drillParent.position, kartPivot.rotation);
                break;
            case 7://oil
                ItemPrefabObjPolling(oilPrefab, GameManager.OilPoolsList, oilParent.position, kartPivot.rotation);
                break;
            default:
                Debug.Log("Error generating item");
                break;
        }
        currentItem = -1;
        ChangeUi();
    }
    private void ItemPrefabObjPolling(GameObject itemPrefab, List<IObjectPollingManager> listToSearch, Vector3 positionToPlace, Quaternion rotationToInhert) {
        IObjectPollingManager objDeactivated = null;
        float[] pos = new float[3];
        pos.SetValue(positionToPlace.x, 0);
        pos.SetValue(positionToPlace.y, 1);
        pos.SetValue(positionToPlace.z, 2);
        foreach (IObjectPollingManager obj in listToSearch) if (!obj.IsActive) objDeactivated = obj;
        if (objDeactivated != null) {
            objDeactivated.Activate(true, pos, kartPivot);
            Debug.Log("recicla");
        }
        else if (listToSearch.Count < GameManager.MaxPowerUpInScene(currentItem)) {
            Debug.Log("cria");
            GameObject item = Instantiate(itemPrefab, positionToPlace, rotationToInhert);
            listToSearch.Add(item.GetComponent<IObjectPollingManager>());
            item.GetComponent<IObjectPollingManager>().Activate(true, pos, kartPivot);
        }
        else Debug.Log("max items in scene for" + kartPivot.gameObject.name + "reached");
    }

    void ChangeUi() {
        if (currentItem + 1 == 0) itemImage.enabled = false;
        else {
            itemImage.sprite = powerUpImages[currentItem];
            itemImage.enabled = true;
        }
    }

    IEnumerator ShieldAnimation() {
        for (int i = 0; i < shieldMesh.Length; i++) shieldMesh[i].enabled = isShielded;
        while (isShielded) {
            shieldParent.transform.Rotate(0, 0, 90 * GameManager.shieldAnimSpeed, Space.Self);
            yield return new WaitForSeconds(GameManager.shieldAnimSpeed);
        }
        for (int i = 0; i < shieldMesh.Length; i++) shieldMesh[i].enabled = isShielded;
    }

    void CheckForPlayersInRange() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, GameManager.invertControlsDistance, transform.forward, .1f, carsLayerMask);
        foreach (RaycastHit hit in hits) {
            if (hit.collider.gameObject != this.gameObject) StartCoroutine(hit.collider.gameObject.GetComponent<ItemCarUse>().InvertControl());
        }
    }

    public IEnumerator InvertControl() {
        data.inputManager.invertControls = -1;
        invertControlsParticles.Play();

        yield return new WaitForSeconds(GameManager.invertControlsEffectDuration);

        data.inputManager.invertControls = 1;
    }

    IEnumerator BreakWheels() {
        data.rb.velocity = Vector3.zero;
        breakImage.alpha = 1;

        yield return new WaitForSeconds(GameManager.breakEffectDuration);

        breakImage.alpha = 0;
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GameManager.invertControlsDistance);
    }
}