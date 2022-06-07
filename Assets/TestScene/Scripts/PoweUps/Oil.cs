using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour, IObjectPollingManager {
    private MeshRenderer meshRenderer;
    private Collider objCollider;
    private bool isActive;
    public bool IsActive { get { return isActive; } set { IsActive = isActive; } }
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        objCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) StartCoroutine(SpinCar(other.GetComponent<ObjectDetectionData>()));
    }

    IEnumerator SpinCar(ObjectDetectionData kart) {
        if (kart.itemData.isShielded) yield break;
        float height = kart.kartTransform.position.y;
        float initRotation = kart.kartTransform.rotation.y;
        kart.playerData.rb.velocity = Vector3.zero;

        for (int i = 1; i < 11; i++) {
            kart.kartTransform.SetPositionAndRotation(new Vector3(kart.kartTransform.position.x, height, kart.kartTransform.position.z), Quaternion.Euler(0, initRotation + (i * 36), 0));
            
            yield return new WaitForSeconds(0.05f);
        }
       Activate(false);
    }
    public void Activate(bool state, float[] initialLocation = null, Transform playerTransform = null) {
        if (initialLocation != null) transform.SetPositionAndRotation(new Vector3(initialLocation[0], initialLocation[1], initialLocation[2]), playerTransform.transform.rotation);
        meshRenderer.enabled = state;
        objCollider.enabled = state;
        isActive = state;
    }
}
