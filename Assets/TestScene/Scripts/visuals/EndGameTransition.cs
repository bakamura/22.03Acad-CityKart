using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndGameTransition : MonoBehaviour {
    public static EndGameTransition Instance { get; private set; }
    [SerializeField] private string LevelSelected;
    [SerializeField] private FinalResultsManager podimsetup;
    [SerializeField] private GameObject endText;
    [SerializeField, Tooltip("DurationUnit in seconds")] private float fadeOutDuration;
    private Image fadeOutImage;
    private float aplha;
    private AsyncOperation loadingSceneOperation;
    private CanvasGroup mainMenuBTN;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            mainMenuBTN = GetComponentInChildren<CanvasGroup>();
            fadeOutImage = GetComponentInChildren<Image>();
        }
        else if (Instance != this) Destroy(gameObject);
    }

    public void StartEffect() {
        endText.SetActive(true);
        InvokeRepeating(nameof(FadeOut), 1f, Time.fixedDeltaTime);
    }

    private void FadeOut() {
        float value = Time.fixedDeltaTime / fadeOutDuration;
        aplha += value;
        fadeOutImage.color = new Color(fadeOutImage.color.r, fadeOutImage.color.g, fadeOutImage.color.b, aplha);
        if (aplha >= 1f) {
            CancelInvoke();            
            fadeOutImage.enabled = false;
            endText.SetActive(false);
            podimsetup.SetPodium();
            foreach (GameObject player in LapsManager.Instance.players) {
                Destroy(player.GetComponent<ObjectDetectionData>().playerData.gameObject);
                Destroy(player);
            }
            GameManager.ClearMatchInfo();
            mainMenuBTN.alpha = 1f;
            mainMenuBTN.blocksRaycasts = true;
            mainMenuBTN.interactable = true;
        }
        //StartCoroutine(LoadingScene());
    }

    IEnumerator LoadingScene() {
        loadingSceneOperation = SceneManager.LoadSceneAsync(LevelSelected);
        while (!loadingSceneOperation.isDone) yield return null;
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
