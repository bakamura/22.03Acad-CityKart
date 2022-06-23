using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }
    [NonSerialized] public Transform[] curentPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            curentPoint = new Transform[GameManager.playerCars.Count];
        }
        else if (Instance != this) Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && curentPoint[other.GetComponent<ObjectDetectionData>().playerData.CarID] != null)
        {
            PlayerData plData = other.GetComponent<ObjectDetectionData>().playerData;
            other.transform.SetPositionAndRotation(curentPoint[plData.CarID].position, curentPoint[plData.CarID].rotation); //Quaternion.Euler(0f, curentPoint[plData.CarID].eulerAngles.y, 0f)
            plData.CurrentMovment = 0;
            other.attachedRigidbody.velocity = Vector3.zero;
            //Debug.Log(other.transform.rotation);
        }
    }
}
