using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionScreen : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private TMP_Text currentPlayerText;
    [SerializeField] private CanvasGroup[] allSelectionScreeens;

    [SerializeField] private InputData[] controlerInputs;
    [SerializeField] private Sprite deactivateBtnImage;

    [Header("Players Info")]
    private int playerAmount;

    [Header("Current Selection Info")]
    private int currentSelectingPlayer = 0;
    private int currentSelectionScreen = 0;
    //used when there is more than 1 controler user
    private int currentSelectingControlerInput = 0;
    private CanvasGroup currentScreen;
    private int LevelSelected;
    AsyncOperation loadingSceneOperation;
    private enum ScreenTypes {
        MainMenu,
        PlayerSelection,
        CarSelection,
        ControlSelection,
        TrackSelectionScreen,
        StartGameScreen,
        LoadingScreen
    };

    public void ChangeScreen(CanvasGroup screen) {
        ChangeCurrentSelectingPlayer();
        if (currentSelectingPlayer == playerAmount || currentSelectionScreen == (int)ScreenTypes.MainMenu || currentSelectionScreen == (int)ScreenTypes.PlayerSelection || currentSelectionScreen >= (int)ScreenTypes.TrackSelectionScreen) {
            currentSelectingPlayer = 0;
            if (currentScreen == null) {
                foreach (CanvasGroup canvas in allSelectionScreeens) {
                    if (canvas.alpha != 0) {
                        canvas.alpha = 0;
                        canvas.interactable = false;
                        canvas.blocksRaycasts = false;
                        break;
                    }
                }
            }
            else {
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

    public void SetPlayerAmount(int players) {
        playerAmount = players;
    }

    private void ChangeCurrentSelectingPlayer() {
        currentSelectingPlayer++;
    }

    private void UpdateCurrentPlayerText() {
        switch (currentSelectionScreen) {
            case (int)ScreenTypes.TrackSelectionScreen:
                currentPlayerText.text = "Select Track";
                break;
            case int i when i > (int)ScreenTypes.StartGameScreen://in start game screen/loading screen
                currentPlayerText.text = "";
                break;
            case (int)ScreenTypes.PlayerSelection:
                currentPlayerText.text = "Player Selection";
                break;
            default:
                currentPlayerText.text = "Player " + (currentSelectingPlayer + 1).ToString() + " Select";
                break;
        }
    }

    public void SetPlayerCar(GameObject carPrefab) {
        GameManager.playerCars.Add(carPrefab);
    }

    public void SetPlayerKeyboardControls(InputData controlInfo) {
        GameManager.playerInputs.Add(controlInfo);
    }

    public void SetPlayerControlerControls() {
        GameManager.playerInputs.Add(controlerInputs[currentSelectingControlerInput]);
        currentSelectingControlerInput++;
    }

    public void SetTrack(int index) {
        //LevelSelected = sceneName;
        LevelSelected = index;
    }

    public void StartGame() {
        StartCoroutine(LoadingScene());
    }

    public void QuitGame() {
        Application.Quit();
    }

    IEnumerator LoadingScene() {
        loadingSceneOperation = SceneManager.LoadSceneAsync(LevelSelected);
        while (!loadingSceneOperation.isDone) yield return null;
    }

    public void DeactivateButton(GameObject button) {
        button.GetComponent<Image>().sprite = deactivateBtnImage;
        button.GetComponent<Button>().enabled = false;
    }
}
