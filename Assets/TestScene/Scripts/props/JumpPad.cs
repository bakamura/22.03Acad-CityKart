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
            other.GetComponent<Rigidbody>().AddForce(transform.up * JumpForce, ForceMode.VelocityChange);
        }
    }
}
