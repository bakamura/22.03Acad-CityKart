using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform teleportPoint;
    [System.NonSerialized] public bool[] playerPassedCheckpoint;

    void Awake()
    {
        playerPassedCheckpoint = new bool[GameManager.playerCars.Count];
    }

    private void Start()
    {
        LapsManager.Instance.AddCheckpointInList(this);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !playerPassedCheckpoint[other.GetComponent<PlayerData>().CarID])
        {
            int playerID = other.GetComponent<PlayerData>().CarID;
            playerPassedCheckpoint[playerID] = true;
            TeleportManager.Instance.curentPoint[playerID] = teleportPoint;
            LapsManager.Instance.UpdateScore(other.gameObject.GetComponent<PlayerData>());
        }
    }
}
