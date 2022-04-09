using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour {

    [SerializeField] private float coolDownRespawn;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") StartCoroutine(Spin(other.gameObject));
    }

    IEnumerator Spin(GameObject kart) {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        float height = kart.transform.position.y + 0.2f;
        float initRotation = kart.transform.rotation.y;
        kart.GetComponent<Rigidbody>().velocity = Vector3.zero;

        for (int i = 1; i < 11; i++) {
            kart.transform.position = new Vector3(kart.transform.position.x, height, kart.transform.position.z);
            kart.transform.rotation = Quaternion.Euler(0, initRotation + (i * 36), 0);
            
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(coolDownRespawn);

        GetComponent<Collider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

}
