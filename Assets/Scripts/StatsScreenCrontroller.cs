using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsScreenCrontroller : MonoBehaviour
{
    private PersistentData data;
    private GameObject root, eventSystem;
    private TextMeshProUGUI stats;
    private Image armorImage, weaponImage;
    PlayerController player;

    void Start() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get inactive Root object stored in persistent data
        root = data.root;
        root.SetActive(false);
        player = root.GetComponentInChildren<PlayerController>();
        eventSystem = GameObject.Find("EventSystem");
        foreach (Transform child in gameObject.transform) {
            if (child.name == "Armor") {
                armorImage = child.gameObject.GetComponent<Image>();
            } else if (child.name == "Weapon") {
                weaponImage = child.gameObject.GetComponent<Image>();
            } else if (child.name == "Stats") {
                stats = child.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }

        string s = "";
        s += player.hp+"/"+player.maxhp+"\n";
        s += player.mana+"/"+player.maxMana+"\n";
        s += player.mindmg+"-"+player.maxdmg+"\n";
        s += player.attack+"\n";
        s += player.defense+"\n";
        s += Mathf.FloorToInt((1/player.speed)*100)+"%\n";
        stats.text = s;

        if (data.armor != null) {
            armorImage.overrideSprite = data.armor.sprite;
        } else {
            armorImage.color = new Color(0,0,0,0);
        }
        weaponImage.overrideSprite = data.weapon.sprite;
    }


    public void ReturnToScene() {
        eventSystem.SetActive(false);
        SceneManager.UnloadSceneAsync("StatsScreen");
        root.SetActive(true);
    }
}
