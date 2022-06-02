using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillPowerup : MonoBehaviour, IObjectPollingManager {
    private MeshRenderer meshRenderer;
    private bool isActive;
    private Collider objCollider;
    public bool IsActive { get { return isActive; } set { IsActive = isActive; } }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) StartCoroutine(Spin(other.GetComponent<ObjectDetectionData>()));
    }
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        objCollider = GetComponent<Collider>();
    }

    IEnumerator Spin(ObjectDetectionData kart) {
        if (kart.itemData.isShielded) yield break;
        float height = kart.kartTransform.position.y;
        float initRotation = kart.kartTransform.rotation.y;
        kart.playerData.rb.velocity = Vector3.zero;

        for (int i = 1; i < 11; i++) {
            kart.kartTransform.SetPositionAndRotation(new Vector3(kart.kartTransform.position.x, height, kart.kartTransform.position.z), Quaternion.Euler(0, initRotation + (i * 36), 0));

            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(Activate(false, 0));
    }
    private void StartMovment()
    {
        InvokeRepeating(nameof(Movment), 0f, .05f);
        StartCoroutine(Activate(false, GameManager.drillDuration));
    }
    private void Movment()
    {
        transform.position += GameManager.drillSpeed * transform.forward;
    }

    public IEnumerator Activate(bool state, float delay, float[] initialLocation = null, Transform playerTransform = null) {
        if (!state && !IsActive) {
            yield return null;
        }
        else {
            yield return new WaitForSeconds(delay);
            if (initialLocation !=null) transform.SetPositionAndRotation(new Vector3(initialLocation[0], initialLocation[1], initialLocation[2]), playerTransform.transform.rotation);
            meshRenderer.enabled = state;
            objCollider.enabled = state;
            if (state) StartMovment();
            isActive = state;
        }
    }
}
