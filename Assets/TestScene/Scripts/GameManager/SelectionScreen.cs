using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text currentPlayerText;
    [SerializeField] private CanvasGroup[] Screens;

    [Header("Players Info")]
    private int playerAmount;

    [Header("Current Selection Info")]
    private static int currentSelectingPlayer = 0;
    private static int currentSelectionScreen = 0;
    private CanvasGroup currentScreen;
    private string LevelSelected;

    public void ChangeScreen(CanvasGroup screen)
    {
        if (currentSelectingPlayer == playerAmount)
        {
            currentSelectingPlayer = 1;
            if (currentScreen == null)
            {
                foreach (CanvasGroup canvas in Screens)
                {
                    if (canvas.alpha != 0)
                    {
                        canvas.alpha = 0;
                        canvas.interactable = false;
                        canvas.blocksRaycasts = false;
                    }
                }
            }
            else
            {
                currentScreen.alpha = 0;
                currentScreen.interactable = false;
                currentScreen.blocksRaycasts = false;
            }
            screen.alpha = 1;
            screen.interactable = true;
            screen.blocksRaycasts = true;
            currentScreen = screen;
            currentSelectionScreen++;
        }
        else
        {
            ChangeCurrentSelectingPlayer();
        }
    }
    public void SetPlayerAmount(int players)
    {
        playerAmount = players;
        currentSelectingPlayer = 1;
    }
    private void ChangeCurrentSelectingPlayer()
    {
        if (playerAmount > 1) currentSelectingPlayer++;
        if (currentSelectionScreen == 2)
        {
            currentPlayerText.text = "Select Track";
            return;
        }
        currentPlayerText.text = "Player" + currentSelectingPlayer.ToString() + "Select";
    }
    public void SetPlayerCar(GameObject carPrefab)
    {
        GameSelectionManager.playerCars.Add(carPrefab);
    }
    public void SetPlayerControls(InputData controlInfo)
    {
        GameSelectionManager.playerInputs.Add(controlInfo);
    }
    public void SetTrack(string sceneName)
    {
        LevelSelected = sceneName;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(LevelSelected);
    }
}
