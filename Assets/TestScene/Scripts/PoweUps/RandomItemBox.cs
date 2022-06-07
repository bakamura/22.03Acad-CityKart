using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemBox : MonoBehaviour {
    private MeshRenderer mesh;
    private Collider objCollider;
    private void Awake() {
        mesh = GetComponent<MeshRenderer>();
        objCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<ItemCarUse>().GenerateNewItem();
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        mesh.enabled = false;
        objCollider.enabled = false;

        yield return new WaitForSeconds(GameManager.itemBoxResawnTime);

        mesh.enabled = true;
        objCollider.enabled = true;
    }

}
