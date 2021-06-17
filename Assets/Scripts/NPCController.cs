using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour
{
    PersistentData data;
    UIController uiController;
    PlayerController player;
    void Start() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        uiController = GameObject.FindWithTag("UICanvas").GetComponent<UIController>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void SpeakToBarkeep() {
        uiController.Dialogue(
            "Barkeep",
            "Looking for a warm bed and mug of ale?\n(-25 gold)",
            new List<string>(){"yes", "no"},
            BarkeepActions);
    }

    public void BarkeepActions(string response) {
        if (response == "yes") {
            if (data.gold >= 25) {
                data.gold -= 25;
                player.food = 1000;
                data.entrance = 2;
                data.direction = 2;
                data.LoadShopList();
                data.LoadingScreenLoad("GreenVillage");
                uiController.EndDialogue();
            } else {
                uiController.Dialogue(
                    "Barkeep",
                    "Come back when you can afford it, then.",
                    new List<string>(){"Oh, ok..."},
                    uiController.EndDialogue);
            }
        } else if (response == "no") {
            uiController.EndDialogue();
        }
    }

    public void SpeakToShopkeeper() {
        uiController.Dialogue(
            "Shopkeeper",
            "Looking to protect yourself, or deal some damage?",
            new List<string>(){"Please sir I'm hungry"},
            StartShopping);
    }

    public void StartShopping(string s="") {
        uiController.EndDialogue();
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
