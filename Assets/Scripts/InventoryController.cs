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
    Image itemImage;
    PlayerController player;
    static Dictionary<string, int> ItemTypeOrder = new Dictionary<string, int>{
        {"Armor", 1},
        {"Weapon", 2},
        {"Potion", 3},
        {"Food", 4}};

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
            } else if (child.name == "ItemImage") {
                itemImage = child.gameObject.GetComponent<Image>();
            }
        }

        // Display List of Items
        DisplayItems();
        DisplayItem(-1);
    }

    public void RefreshItems() {
        foreach (Transform child in itemArray.transform) {
            Destroy(child.gameObject);
        }
        DisplayItems();
    }

    public void DisplayItems() {
        data.inventory.Sort(CompareItems);
        GameObject itemButton = Resources.Load("Prefabs/InventoryItemButton") as GameObject;
        int i=0, j=0;
        float x, y;
        GameObject clone;
        RectTransform rt;
        // Add all inventory items
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
            i++;
            if (i%11 == 0) { j++; }
        }
        // Display Equipped Weapon
        clone = Instantiate(
            itemButton,
            itemArray.transform);
        rt = clone.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(7.5f, 2f, 0);
        clone.GetComponent<Image>().overrideSprite =  data.weapon.sprite;
        clone.GetComponent<InventoryItemController>().SetItemIndex(-1);
        // Display Equipped Armor
        if (data.armor != null) {
            clone = Instantiate(
                itemButton,
                itemArray.transform);
            rt = clone.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3(6.25f, 2f, 0);
            clone.GetComponent<Image>().overrideSprite =  data.armor.sprite;
            clone.GetComponent<InventoryItemController>().SetItemIndex(-2);
        }
    }

    public void DisplayItem(int index) {
        selected = index;
        InventoryItem item;
        if (index == -1) {
            item = data.weapon;
        } else if (index == -2) {
            item = data.armor;
        } else {
            item = data.inventory[index];
        }
        title.text = item.displayName;
        description.text = item.description;
        itemImage.overrideSprite = item.sprite;
        if (item.itemType == "Weapon") {
            Weapon wep = item as Weapon;
            if (index >= 0) {
                button.text = "Equip";
            } else {
                button.text = "-";
            }
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
            if (wep.speed > 1) {
                stat += "\nspeed +"+Mathf.RoundToInt((wep.speed - 1)*100)+"%";
            } else if (wep.speed < 1) {
                stat += "\nspeed "+Mathf.RoundToInt((wep.speed - 1)*100)+"%";
            }
            stats.text = stat;
        } else if (item.itemType == "Armor") {
            Armor arm = item as Armor;
            if (index >= 0) {
                button.text = "Equip";
            } else {
                button.text = "-";
            }
            string stat = "Def +"+arm.def;
            if (arm.speed > 1) {
                stat += "\nspeed +"+Mathf.RoundToInt((arm.speed - 1)*100)+"%";
            } else if (arm.speed < 1) {
                stat += "\nspeed "+Mathf.RoundToInt((arm.speed - 1)*100)+"%";
            }
            stats.text = stat;
        } else if (item.itemType == "Potion") {
            if (item.count > 1) {
                title.text = item.displayName+" ("+item.count+")";
            }
            button.text = "Drink";
            Potion pot = item as Potion;
            string stat = "";
            if (pot.healing > 0) {
                stat += "+"+pot.healing+" hp";
            }
            stats.text = stat;
        } else if (item.itemType == "Food") {
            if (item.count > 1) {
                title.text = item.displayName+" ("+item.count+")";
            }
            button.text = "Eat";
            Food food = item as Food;
            string stat = "Food: "+food.food/10+"%";
            if (food.damage > 0) {
                stat += "\n-"+food.damage+" hp";
            } else if (food.healing > 0) {
                stat += "\n+"+food.healing+" hp";
            }
            stats.text = stat;
        }
    }

    public void ActivateButton() {
        if (selected < 0) { return; }
        InventoryItem item = data.inventory[selected];
        if (item.itemType == "Weapon") {
            EquipWeapon();
        } else if (item.itemType == "Armor") {
            EquipArmor();
        } else if (item.itemType == "Potion") {
            item.Activate(player);
            if (item.count == 0) {
                data.inventory.RemoveAt(selected);
            }
        } else if (item.itemType == "Food") {
            item.Activate(player);
            if (item.count == 0) {
                data.inventory.RemoveAt(selected);
            }
        }
        BackToScene();
        player.EndTurn();
    }

    public void EquipWeapon() {
        player.EquipWeapon(data.inventory[selected] as Weapon);
        Weapon old = data.weapon;
        data.weapon = data.inventory[selected] as Weapon;
        data.inventory.RemoveAt(selected);
        data.inventory.Add(old);
        player.FloatText("msg", "Equipped "+data.weapon.displayName);
    }

    public void EquipArmor() {
        player.EquipArmor(data.inventory[selected] as Armor);
        Armor old = data.armor;
        data.armor = data.inventory[selected] as Armor;
        data.inventory.RemoveAt(selected);
        if (old != null) {
            data.inventory.Add(old);
        }
        player.FloatText("msg", "Equipped "+data.armor.displayName);
    }

    private int CompareItems(InventoryItem left, InventoryItem right) {
        if (left.displayName == right.displayName) {
            return 0;
        }
        if (ItemTypeOrder[left.itemType] < ItemTypeOrder[right.itemType]) {
            return -1;
        } else if (ItemTypeOrder[left.itemType] > ItemTypeOrder[right.itemType]) {
            return 1;
        } else {
            if (left.tier > right.tier) {
                return -1;
            } else if (left.tier < right.tier) {
                return 1;
            } else {
                if (string.Compare(left.name, right.name) < 0) {
                    return -1;
                } else if (string.Compare(left.name, right.name) > 0) {
                    return 1;
                } else {
                    if (left.quality > right.quality) {
                        return -1;
                    } else if (left.quality < right.quality) {
                        return 1;
                    } else {
                        return 0;
                    }
                }
            }
        }
    }

    public void BackToScene() {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("GreenVillage"));
        SceneManager.UnloadSceneAsync("Inventory");
        root.SetActive(true);
    }

}
