using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour {

    private bool loading = false;
    AsyncOperation loadingOperation;
    private float loadProgress;
    public Slider progressBar; // assigned in inspector
    public Text percentLoaded; // assigned in inspector
    public CanvasGroup canvasGroup; // assigned in inspector


    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        DontDestroyOnLoad(canvasGroup.gameObject);
    }

    void Update() {
        if (loading) {
            float progressValue = Mathf.Clamp01(loadProgress / 0.9f);
            progressBar.value = progressValue;
            percentLoaded.text = Mathf.Round(progressValue * 100) + "%";
        }
    }

    public void ToMainScreen() {
        loadingOperation = SceneManager.LoadSceneAsync("AddedPluginScene");
        Debug.Log("AddedPluginScene");
        loadProgress = loadingOperation.progress;
        loading = true;
    }

    public void ToPTP() {
        loadingOperation = SceneManager.LoadSceneAsync("PlantToProductScene");
        Debug.Log("PlantToProductScene");
        loadProgress = loadingOperation.progress;
        loading = true;
    }

    public void ToOIF() {
        loadingOperation = SceneManager.LoadSceneAsync("OxyInFilmScene");
        Debug.Log("OxyInFilmScene");
        loadProgress = loadingOperation.progress;
        loading = true;
    }

    public void ToTH() {
        loadingOperation = SceneManager.LoadSceneAsync("TimelapseHistoryScene");
        Debug.Log("TimelapseHistoryScene");
        loadProgress = loadingOperation.progress;
        loading = true;
    }

    IEnumerator FadeLoadingScreen(float duration) {
        float startValue = canvasGroup.alpha;
        float time = 0;
        while (time < duration) {
            canvasGroup.alpha = Mathf.Lerp(startValue, 1, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

}
