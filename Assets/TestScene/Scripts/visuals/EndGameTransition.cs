using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameTransition : MonoBehaviour
{
    public static EndGameTransition Instance { get; private set; }
    [SerializeField] private string LevelSelected;
    [SerializeField] private Image fadeOutImage;
    private float aplha;
    private AsyncOperation loadingSceneOperation;

    private void Awake()
    {
        if (Instance == null)Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void StartEffect()
    {
        InvokeRepeating("FadeOut", 3f, Time.fixedDeltaTime);
    }

    private void FadeOut()
    {
        float value = 1f / 255f;
        aplha += value;
        fadeOutImage.color = new Color(fadeOutImage.color.r, fadeOutImage.color.g, fadeOutImage.color.b, aplha);
        if (aplha >= 1f) StartCoroutine(LoadingScene());
    }

    IEnumerator LoadingScene()
    {
        loadingSceneOperation = SceneManager.LoadSceneAsync(LevelSelected);
        while (!loadingSceneOperation.isDone) yield return null;
    }
}
