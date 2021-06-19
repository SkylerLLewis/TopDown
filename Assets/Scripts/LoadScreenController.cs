using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadScreenController : MonoBehaviour
{
    PersistentData data;
    GameObject canvas;
    TextMeshProUGUI title;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake() {
        DontDestroyOnLoad(gameObject);
        canvas = transform.GetChild(0).gameObject;
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        foreach (Transform child in canvas.transform) {
            if (child.name == "Title") {
                title = child.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }
        
    }

    public void LoadNextScene(string scene, string style) {
        float duration = 1;
        if (style == "village") {
            title.text = "Erecting houses...";
        } else if (style == "sleep") {
            title.text = "z z z . . .";
            duration = 2;
        } else if (style == "descending") {
            title.text = "The gloom deepens";
            duration = 0.5f;
        } else if (style == "ascending") {
            title.text = "It grows lighter as you ascend";
            duration = 0.5f;
        } else if (style == "death") {
            title.text = "You see a fire approaching...";
            duration = 4;
        }
        gameObject.SetActive(true);
        StartCoroutine(LoadScene(scene, duration));
    }

    IEnumerator LoadScene(string scene, float duration=1) {
        yield return StartCoroutine(FadeLoadingScreen(1,duration));

        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone) {
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(0,duration));
        gameObject.SetActive(false);
    }

    IEnumerator FadeLoadingScreen(float targetValue, float duration) {
        float startValue = canvasGroup.alpha;
        float time = 0;
        while (time < duration) {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time/duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }
/*
    // Update is called once per frame
    void Update() {
        if (enter) {
            if (count < 1) {
                count += 1.0f * 5 * Time.deltaTime;
                canvasGroup.alpha = count;
            } else {
                Debug.Log("Unloading the previous scene: "+data.sceneToUnload);
                enter = false;
                StartCoroutine(UnloadLastScene());
            }
        }
        if (exit) {
            if (count > 0) {
                count -= 1.0f * 5 * Time.deltaTime;
                canvasGroup.alpha = count;
            } else {
                Debug.Log("Unloading load scene ");
                exit = false;
                SceneManager.UnloadSceneAsync("LoadingScreen");
            }
        }
    }

    IEnumerator UnloadLastScene() {
        Debug.Log("Async Unload");
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(data.sceneToUnload);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
        StartCoroutine(LoadNextScene());
    }
    IEnumerator LoadNextScene() {
        Debug.Log("Async Load:"+data.sceneToLoad);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f) {
            yield return null;
        }
        
        exit = true;
        while (exit) {
            yield return 0;
        }
        asyncLoad.allowSceneActivation = true;
    }
*/
    
}
