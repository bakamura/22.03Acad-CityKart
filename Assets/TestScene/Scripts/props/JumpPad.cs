using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
    [SerializeField] private float JumpForce;

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            other.GetComponent<Rigidbody>().velocity = new Vector3(0, JumpForce, JumpForce / 4);
        }
    }
}
