using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform teleportPoint;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag == "Player" && TPManager.Instance.curentPoint != teleportPoint)
        {
            TPManager.Instance.curentPoint = teleportPoint;
        }
    }
}
