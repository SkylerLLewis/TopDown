using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour
{
    PersistentData data;
    void Start() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
    }

    public void StartShopping() {
        data.loadedMenu = "Shop";
        StartCoroutine(LoadShopScene());
    }
    // Loads Scene on top of current scene asyncronously
    IEnumerator LoadShopScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
