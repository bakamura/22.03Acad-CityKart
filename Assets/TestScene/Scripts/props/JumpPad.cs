using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float JumpForce;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("a");
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        }
    }
}
