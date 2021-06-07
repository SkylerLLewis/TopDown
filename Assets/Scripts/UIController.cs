using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    RectTransform hpBar, foodBar;
    TextMeshProUGUI depthText;
    private PersistentData data;
    private PlayerController player;
    private GameObject root;
    void Awake()
    {
        root = GameObject.FindWithTag("Root");
        hpBar = GameObject.Find("HP Bar").GetComponent<RectTransform>();
        foodBar = GameObject.Find("Food Bar").GetComponent<RectTransform>();
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        depthText = GameObject.Find("Depth Text").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        depthText.text = data.depth.ToString();
    }

    public void UpdateBars() {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 190f*((player.hp*1.0f)/player.maxhp));
        foodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 190f*(player.food/1000));
    }

    public void OpenInventory() {
        StartCoroutine(LoadInventoryScene());
    }
    // Loads Scene on top of current scene asyncronously
    IEnumerator LoadInventoryScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Inventory", LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

    public void OpenStats() {
        StartCoroutine(LoadStatsScene());
    }
    // Loads Scene on top of current scene asyncronously
    IEnumerator LoadStatsScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StatsScreen", LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    

    public void AbilityActivate(string name) {
        Debug.Log("This: "+this.GetComponent<UIController>().ToString());
        player.Ability(name);
    }

}
