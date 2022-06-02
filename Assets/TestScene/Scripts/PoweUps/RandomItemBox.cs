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

        yield return new WaitForSeconds(10);

        mesh.enabled = true;
        objCollider.enabled = true;
    }

}
