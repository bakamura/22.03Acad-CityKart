using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LapsManager : MonoBehaviour
{
    public static LapsManager Instance { get; private set; }
    [SerializeField] private int NumberOfLaps;
    private int[] finalScore;
    private int[] playersScore;
    private List<Checkpoint> chekpointsList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playersScore = new int[GameManager.playerCars.Count];
            finalScore = new int[GameManager.playerCars.Count];
            chekpointsList = new List<Checkpoint>();
        }
        else if (Instance != this) Destroy(gameObject);
    }

    public void UpdateScore(int playerID)
    {
        playersScore[playerID]++;
        finalScore[playerID]++;
    }

    public void AddCheckpointInList(Checkpoint script)
    {
        chekpointsList.Add(script);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            int playerID = other.GetComponent<PlayerData>().CarID;
            if (finalScore[playerID] >= chekpointsList.Count * NumberOfLaps) FinishGame();
            else foreach (Checkpoint checkpoint in chekpointsList) checkpoint.playerPassedCheckpoint[playerID] = false;
        }
    }
    private void FinishGame()
    {
        GameObject[] podiumPositions = new GameObject[playersScore.Length];
        Array.Reverse(finalScore);
        for (int i = 0; i < podiumPositions.Length; i++)
        {
            for (int a = 0; a < playersScore.Length; a++)
            {
                if (finalScore[i] == playersScore[a])
                {
                    podiumPositions[i] = GameManager.playerCars[a];
                    break;
                }
            }
        }
        GameManager.finalResults = new GameObject[GameManager.playerCars.Count];
        GameManager.finalResults = podiumPositions;
        for (int i = 0; i < GameManager.finalResults.Length; i++) Debug.Log(i+1 + " Place " + GameManager.finalResults[i].name);
    }
}
