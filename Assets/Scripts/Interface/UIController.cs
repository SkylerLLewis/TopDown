using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    Canvas canvas;
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
    private List<SkillController> skillButtons;
    private System.Action<string> responseAction;
    void Awake()
    {
        // Load resources
        canvas = GetComponent<Canvas>();
        root = GameObject.FindWithTag("Root");
        hpBar = GameObject.Find("HP Bar").GetComponent<RectTransform>();
        manaBar = GameObject.Find("MP Bar").GetComponent<RectTransform>();
        foodBar = GameObject.Find("Food Bar").GetComponent<RectTransform>();
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        depthText = GameObject.Find("Depth Text").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        depthText.text = data.depth.ToString();

        skillButtons = new List<SkillController>();
        for (int i=1; i<=6; i++) {
            SkillController skillButton = GameObject.Find("Skill Button "+i)
                .GetComponent<SkillController>();
            skillButtons.Add(skillButton);
        }
        UpdateSkillButtons();

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

    public void UpdateSkillButtons() {
        int buttonNum = 0;
        foreach(string n in data.activeSkills) {
            SkillController skillButton = skillButtons[buttonNum];
            skillButton.gameObject.SetActive(true);
            skillButton.SetName(n);
            buttonNum++;
        }
        for (int i=buttonNum; i < 6; i++) {
            skillButtons[i].gameObject.SetActive(false);
        }
    }

    public void UpdateHp() {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f*((float)player.hp/player.maxhp));
    }

    public void UpdateMana() {
        manaBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f*((float)player.mana/player.maxMana));
        // Activate/deactivate magic skills?
        foreach(SkillController s in skillButtons) {
            if (!s.gameObject.activeInHierarchy) break;
            if (s.skill.abilityType == "magic"){
                if (!s.disabled && player.mana < s.skill.manaCost) {
                    s.Disable();
                } else if (s.disabled && player.mana >= s.skill.manaCost) {
                    s.Undisable();
                }
            }
        }
    }

    public void UpdateFood() {
        foodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f*(player.food/1000));
    }

    public void UsedSkill(string n) {
        foreach(SkillController s in skillButtons) {
            if (s.skillName == n) {
                s.Used();
            }
        }
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

    public void OpenSkills() {
        data.loadedMenu = "SkillsScreen";
        StartCoroutine(LoadSkillsScene());
    }
    // Loads Scene on top of current scene asyncronously
    IEnumerator LoadSkillsScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SkillsScreen", LoadSceneMode.Additive);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    

    public void AbilityActivate(Skill s) {
        player.Ability(s);
    }

    public void CancelAbility(string name) {
        player.CancelAbility(name);
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
