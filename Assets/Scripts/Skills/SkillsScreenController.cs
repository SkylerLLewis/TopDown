using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SkillsScreenController : MonoBehaviour
{
    private PersistentData data;
    private GameObject root, eventSystem, textFab, singleOption;
    private TextMeshProUGUI title, unlockName, unlockDesc, skillPoints;
    private Button unlockButton;
    private PlayerController player;
    private UIController uiController;
    private Skill activeSkill;
    void Awake() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get inactive Root object stored in persistent data
        root = data.root;
        root.SetActive(false);
        player = root.GetComponentInChildren<PlayerController>();
        uiController = root.GetComponentInChildren<UIController>();
        textFab = Resources.Load("Prefabs/DamageText") as GameObject;

        eventSystem = GameObject.Find("EventSystem");

        unlockButton = GameObject.Find("UnlockButton").GetComponent<Button>();
        skillPoints = GameObject.Find("SkillPoints").GetComponent<TextMeshProUGUI>();
        skillPoints.text = data.skillPoints.ToString();
        title = gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        singleOption = gameObject.transform.Find("SingleOption").gameObject;
        foreach (Transform child in singleOption.transform) {
            if (child.name == "UnlockName") {
                unlockName = child.GetComponent<TextMeshProUGUI>();
            } else if (child.name == "Description") {
                unlockDesc = child.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public void BackToScene() {
        eventSystem.SetActive(false);
        SceneManager.UnloadSceneAsync("SkillsScreen");
        root.SetActive(true);
    }

    public void DisplayUnlock(string n) {
        Skill skill = new Skill(n);
        title.text = skill.name;
        unlockName.text = skill.displayName;
        unlockDesc.text = skill.description;
        activeSkill = skill;
        if (data.skillPoints <= 0) {
            unlockButton.gameObject.SetActive(false);
        } else {
            unlockButton.gameObject.SetActive(true);
        }
    }

    public void UnlockSkill() {
        if (data.skillPoints >= 1) {
            data.skillPoints -= 1;
            skillPoints.text = data.skillPoints.ToString();
            GameObject text = Instantiate(textFab, new Vector3(0,0,0), Quaternion.identity, gameObject.transform);
            DmgTextController textCont = text.GetComponent<DmgTextController>();
            textCont.Init(skillPoints.transform.position, "skillCost", (-1).ToString());
            bool contains = false;
            Skill selected = null;
            foreach (Skill s in data.skills) {
                if (s.name == activeSkill.name) {
                    contains = true;
                    selected = s;
                    break;
                }
            }
            if (contains) {
                selected.magnitude++;
            } else {
                data.skills.Add(activeSkill);
                if (activeSkill.abilityType == "magic") {
                    data.activeSkills.Add(activeSkill.name);
                    uiController.UpdateSkillButtons();
                }
            }
            player.ApplySkills();
            DisplayUnlock(activeSkill.name);
        }
    }
}
