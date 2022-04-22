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
        if (other.tag == "Player" && curentPoint[other.GetComponent<PlayerData>().CarID] != null)
        {
            other.transform.position = curentPoint[other.GetComponent<PlayerData>().CarID].position;
            other.transform.rotation = Quaternion.Euler(0f, curentPoint[other.GetComponent<PlayerData>().CarID].eulerAngles.y, 0f);
        }
    }
}
