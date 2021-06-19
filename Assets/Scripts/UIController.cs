using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    RectTransform hpBar, manaBar, foodBar;
    TextMeshProUGUI depthText;
    private PersistentData data;
    private PlayerController player;
    private GameObject root, dialogueGroup;
    private TextMeshProUGUI dialogueName, prompt;
    private Image dialogueBox;
    private List<TextMeshProUGUI> options;
    private List<string> dialogueOptions;
    private List<Button> optionButtons;
    private System.Action<string> responseAction;
    void Awake()
    {
        root = GameObject.FindWithTag("Root");
        hpBar = GameObject.Find("HP Bar").GetComponent<RectTransform>();
        manaBar = GameObject.Find("MP Bar").GetComponent<RectTransform>();
        foodBar = GameObject.Find("Food Bar").GetComponent<RectTransform>();
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        depthText = GameObject.Find("Depth Text").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        depthText.text = data.depth.ToString();

        optionButtons = new List<Button>();
        options = new List<TextMeshProUGUI>();
        dialogueGroup = GameObject.Find("Dialogue");
        dialogueGroup.SetActive(false);
        dialogueBox = dialogueGroup.GetComponent<Image>();
        foreach (Transform child in dialogueGroup.transform) {
            if (child.name == "Name") {
                dialogueName = child.gameObject.GetComponent<TextMeshProUGUI>();
            } else if (child.name == "Prompt") {
                prompt = child.gameObject.GetComponent<TextMeshProUGUI>();
            } else if (child.name == "Option1") {
                optionButtons.Add(child.gameObject.GetComponent<Button>());
                options.Add(child.GetChild(0).GetComponent<TextMeshProUGUI>());
            } else if (child.name == "Option2") {
                optionButtons.Add(child.gameObject.GetComponent<Button>());
                options.Add(child.GetChild(0).GetComponent<TextMeshProUGUI>());
            } else if (child.name == "Option3") {
                optionButtons.Add(child.gameObject.GetComponent<Button>());
                options.Add(child.GetChild(0).GetComponent<TextMeshProUGUI>());
            }
        }
        dialogueOptions = new List<string>();
    }

    void Update() {

    }

    public void UpdateBars() {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f*((float)player.hp/player.maxhp));
        manaBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f*((float)player.mana/player.maxMana));
        foodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f*(player.food/1000));
    }

    public void OpenInventory() {
        data.loadedMenu = "Inventory";
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
        player.Ability(name);
    }

    public void Dialogue(string name, string _prompt, List<string> opts, System.Action<string> response) {
        dialogueGroup.SetActive(true);
        dialogueName.text = name;
        prompt.text = _prompt;
        for (int i=0; i<3; i++) {
            if (opts.Count > i) {
                if (!optionButtons[i].interactable) optionButtons[i].interactable = true;
                options[i].text = opts[i];
            } else {
                if (optionButtons[i].interactable) optionButtons[i].interactable = false;
                options[i].text = "";
            }
        }
        dialogueOptions = opts;
        responseAction = response;
    }

    public void DialogueOption(int choice) {
        responseAction(dialogueOptions[choice]);
    }

    public void EndDialogue(string s="") {
        dialogueGroup.SetActive(false);
        player.enabled = true;
    }

    public void Die() {
        
    }
}
