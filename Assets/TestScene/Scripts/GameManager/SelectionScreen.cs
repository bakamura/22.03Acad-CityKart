using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionScreen : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text currentPlayerText;
    [SerializeField] private CanvasGroup[] allSelectionScreeens;

    [SerializeField] private InputData[] controlerInputs;

    [Header("Players Info")]
    private int playerAmount;

    [Header("Current Selection Info")]
    private int currentSelectingPlayer = 0;
    private int currentSelectionScreen = 0;
    //used when there is more than 1 controler user
    private int currentSelectingControlerInput = 0;
    private CanvasGroup currentScreen;
    private string LevelSelected;

    AsyncOperation loadingSceneOperation;

    public void ChangeScreen(CanvasGroup screen)
    {
        ChangeCurrentSelectingPlayer();
        if (currentSelectingPlayer == playerAmount || currentSelectionScreen == 0 || currentSelectionScreen >= 3)
        {
            currentSelectingPlayer = 0;
            if (currentScreen == null)
            {
                foreach (CanvasGroup canvas in allSelectionScreeens)
                {
                    if (canvas.alpha != 0)
                    {
                        canvas.alpha = 0;
                        canvas.interactable = false;
                        canvas.blocksRaycasts = false;
                        break;
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
        UpdateCurrentPlayerText();
    }

    public void SetPlayerAmount(int players)
    {
        playerAmount = players;
    }

    private void ChangeCurrentSelectingPlayer()
    { 
        currentSelectingPlayer++;
    }

    private void UpdateCurrentPlayerText()
    {
        //in track seletion screen
        if (currentSelectionScreen == allSelectionScreeens.Length - 3)
        {
            currentPlayerText.text = "Select Track";
            return;
        }
        //in start game screen/loading screen
        else if (currentSelectionScreen > 3)
        {
            currentPlayerText.text = "";
            return;
        }
        currentPlayerText.text = "Player " + (currentSelectingPlayer + 1).ToString() + " Select";
    }

    public void SetPlayerCar(GameObject carPrefab)
    {
        GameSelectionManager.playerCars.Add(carPrefab);
    }

    public void SetPlayerKeyboardControls(InputData controlInfo)
    {
        GameSelectionManager.playerInputs.Add(controlInfo);
    }

    public void SetPlayerControlerControls()
    {
        GameSelectionManager.playerInputs.Add(controlerInputs[currentSelectingControlerInput]);
        currentSelectingControlerInput++;
    }

    public void SetTrack(string sceneName)
    {
        LevelSelected = sceneName;        
    }

    public void StartGame()
    {
        StartCoroutine(LoadingScene());
    }
    IEnumerator LoadingScene()
    {
        loadingSceneOperation = SceneManager.LoadSceneAsync(LevelSelected);
        while (!loadingSceneOperation.isDone) yield return null;
    }

    public void DeactivateButton(GameObject button)
    {
        button.GetComponent<Button>().enabled = false;
        Image btnImage = button.GetComponent<Image>();
        btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, .25f);
    }
}
