using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    int selected;
    private PersistentData data;
    private GameObject root, itemArray;
    TextMeshProUGUI title, description, button, stats;
    PlayerController player;
    void Start() {
        selected = -1;
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get inactive Root object stored in persistent data
        root = data.root;
        root.SetActive(false);
        player = root.GetComponentInChildren<PlayerController>();

        foreach (Transform child in gameObject.transform) {
            if (child.name == "ItemArray") {
                itemArray = child.gameObject;
            } else if (child.name == "Title") {
                title = child.gameObject.GetComponent<TextMeshProUGUI>();
            } else if (child.name == "Description") {
                description = child.gameObject.GetComponent<TextMeshProUGUI>();
            } else if (child.name == "Activate") {
                button = child.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            } else if (child.name == "Stats") {
                stats = child.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }

        // Display List of Items
        DisplayItems();
    }

    public void RefreshItems() {
        foreach (Transform child in itemArray.transform) {
            Destroy(child.gameObject);
        }
        DisplayItems();
    }

    public void DisplayItems() {
        GameObject itemButton = Resources.Load("Prefabs/InventoryItemButton") as GameObject;
        int i=0, j=0;
        float x, y;
        GameObject clone;
        RectTransform rt;
        foreach (InventoryItem item in data.inventory) {
            x = -7.5f + (i%11)*1.25f;
            y = 4f - j*1.25f;
            clone = Instantiate(
                itemButton,
                itemArray.transform);
            rt = clone.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3(x, y, 0);
            clone.GetComponent<Image>().overrideSprite =  item.sprite;
            clone.GetComponent<InventoryItemController>().SetItemIndex(i);
            Debug.Log("Added item "+item.name+" to inventory display.");
            i++;
            if (i%11 == 0) { j++; }
        }
        clone = Instantiate(
            itemButton,
            itemArray.transform);
        rt = clone.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(7f, 1.75f, 0);
        clone.GetComponent<Image>().overrideSprite =  data.weapon.sprite;
        clone.GetComponent<InventoryItemController>().SetItemIndex(-1);
    }

    public void DisplayItem(int index) {
        selected = index;
        InventoryItem item;
        if (index == -1) {
            // ! This will not work for all equipment!
            item = data.weapon;
        } else {
            item = data.inventory[index];
        }
        title.text = item.name;
        description.text = item.description;
        if (item.itemType == "Weapon") {
            Weapon wep = item as Weapon;
            button.text = "Equip";
            string stat = "Dmg: "+wep.mindmg+"-"+wep.maxdmg;
            if (wep.atk > 0) {
                stat += "\natk +"+wep.atk;
            } else if (wep.atk < 0) {
                stat += "\natk "+wep.atk;
            }
            if (wep.def > 0) {
                stat += "\ndef +"+wep.def;
            } else if (wep.def < 0) {
                stat += "\ndef "+wep.def;
            }
            if (wep.speed < 1) {
                stat += "\nspeed +"+Mathf.RoundToInt((1/wep.speed-1)*100)+"%";
            } else if (wep.speed > 1) {
                stat += "\nspeed -"+Mathf.RoundToInt((1-1/wep.speed)*100)+"%";
            }
            stats.text = stat;
        }
    }

    public void ActivateButton() {
        if (selected == -1) { return; }
        InventoryItem item = data.inventory[selected];
        if (item.itemType == "Weapon" && item != data.weapon) {
            EquipWeapon();
        }
    }

    public void EquipWeapon() {
        player.EquipWeapon(data.inventory[selected] as Weapon);
        Weapon old = data.weapon;
        data.weapon = data.inventory[selected] as Weapon;
        data.inventory.RemoveAt(selected);
        data.inventory.Add(old);
        RefreshItems();
    }

    public void BackToScene() {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("GreenVillage"));
        SceneManager.UnloadSceneAsync("Inventory");
        root.SetActive(true);
    }

}
