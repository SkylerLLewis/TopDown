using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour
{
    PersistentData data;
    public UIController uiController;
    PlayerController player;
    void Start() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        uiController = GameObject.FindWithTag("UICanvas").GetComponent<UIController>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void SpeakToBarkeep() {
        uiController.Dialogue(
            "Barkeep",
            "Looking for a warm bed and mug of ale?\n(-5 gold)",
            new List<string>(){"yes", "no"},
            BarkeepActions);
    }

    public void BarkeepActions(string response) {
        if (response == "yes") {
            if (data.gold >= 5) {
                data.gold -= 5;
                player.food = 1000;
                player.hp = player.maxhp;
                player.mana = player.maxMana;
                data.entrance = 2;
                data.direction = 2;
                data.LoadShopList();
                data.LoadingScreenLoad("GreenVillage","sleep");
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

    public void RecoverFromDeath() {
        uiController.Dialogue(
            "Barkeep",
            "You're a lucky one. Some adventurers drug you out of the dungeon.",
            new List<string>(){"I'm in pain"},
            RecoverFromDeath);
    }
    public void RecoverFromDeath(string response) {
        uiController.Dialogue(
            "Barkeep",
            "Sadly, they didn't pay for your stay or their drinks. I took it out of your pocket.",
            new List<string>(){"Aw, man"},
            uiController.EndDialogue);
    }

    public void SpeakToShopkeeper() {
        uiController.Dialogue(
            "Shopkeeper",
            "Looking to protect yourself, or deal some damage?",
            new List<string>(){"Trade"},
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
