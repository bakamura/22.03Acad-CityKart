using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LapsManager : MonoBehaviour
{
    public static LapsManager Instance { get; private set; }
    [SerializeField] private int NumberOfLaps;
    private List<Checkpoint> chekpointsList;
    [NonSerialized] public GameObject[] players;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            chekpointsList = new List<Checkpoint>();
        }
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponentInChildren<ObjectDetectionData>().playerData.PlayerScore >= chekpointsList.Count * NumberOfLaps)
            {
                FinishGame();
                return;
            }
            else
            {
                int playerID = other.GetComponentInChildren<ObjectDetectionData>().playerData.CarID;
                foreach (Checkpoint checkpoint in chekpointsList) checkpoint.playerPassedCheckpoint[playerID] = false;
            }
        }
    }

    public void UpdateScore(PlayerData data)
    {
        data.PlayerScore++;
    }

    public int[] GetCurrentPlayerPodiumPossitions()
    {
        int[] currentPlayerPodiumPossition = new int[players.Length];
        for (int i = 0; i < currentPlayerPodiumPossition.Length; i++)
        {
            currentPlayerPodiumPossition[i] = players[i].GetComponentInChildren<ObjectDetectionData>().playerData.PlayerScore;
        }
        Array.Reverse(currentPlayerPodiumPossition);
        return currentPlayerPodiumPossition;
    }

    public GameObject GetMySuccessorPlayer(PlayerData myData)
    {
        int[] temp = GetCurrentPlayerPodiumPossitions();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == myData.PlayerScore)
            {
                if (i == 0)return null;                
                else
                {
                    foreach (GameObject player in players)
                    {
                        if (player.GetComponentInChildren<ObjectDetectionData>().playerData.PlayerScore == temp[i - 1])
                        {
                            return player;
                        }
                    }
                }
            }
        }
        return null;
    }

    public GameObject GetPlayerInFirst(PlayerData myData)
    {
        int[] temp = GetCurrentPlayerPodiumPossitions();
        int score = Mathf.Max(temp);
        if (myData.PlayerScore == score) return null;
        foreach (GameObject player in players)
        {
            if (player.GetComponentInChildren<ObjectDetectionData>().playerData.PlayerScore == score)
            {
                return player;
            }
        }
        return null;
    }

    public void AddCheckpointInList(Checkpoint script)
    {
        chekpointsList.Add(script);
    }
    private void FinishGame()
    {
        GameObject[] podiumPositions = new GameObject[players.Length];
        int[] finalScore = GetCurrentPlayerPodiumPossitions();
        for (int i = 0; i < podiumPositions.Length; i++)
        {
            for (int a = 0; a < players.Length; a++)
            {
                if (finalScore[i] == players[a].GetComponentInChildren<ObjectDetectionData>().playerData.PlayerScore)
                {
                    podiumPositions[i] = GameManager.playerCars[a];
                    break;
                }
            }
        }
        GameManager.finalResults = new GameObject[GameManager.playerCars.Count];
        GameManager.finalResults = podiumPositions;
        EndGameTransition.Instance.StartEffect();
    }
}
