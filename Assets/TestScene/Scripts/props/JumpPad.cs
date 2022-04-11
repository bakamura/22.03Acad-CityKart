using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
    [SerializeField] private float JumpForce;

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            foreach (Animation anim in GetComponentsInChildren<Animation>()) anim.Play();
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
            //new Vector3(0, JumpForce, JumpForce / 4)
        }
    }
}
