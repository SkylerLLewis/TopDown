using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    int selected;
    private PersistentData data;
    private GameObject root, itemArray, eventSystem;
    TextMeshProUGUI title, description, button, stats, gold;
    Image itemImage;
    PlayerController player;

    void Start()
    {
        selected = 0;
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
            } else if (child.name == "Gold") {
                gold = child.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }
        eventSystem = GameObject.Find("EventSystem");

        // Display Everything
        gold.text = data.gold.ToString();
        DisplayItems();
        DisplayItem(0);
    }

    public void RefreshItems() {
        foreach (Transform child in itemArray.transform) {
            Destroy(child.gameObject);
        }
        DisplayItems();
    }

    public void DisplayItems() {
        GameObject itemButton = Resources.Load("Prefabs/InventoryItemButton") as GameObject;
        if (itemButton == null) Debug.Log("ITEMBUTTON IS NULL");
        int i=0, j=0;
        float x, y;
        GameObject clone;
        RectTransform rt;
        // Add all shop items
        foreach (InventoryItem item in data.shopList) {
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
            // Display item count
            if (item.count != 1) {
                clone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.count.ToString();
            }
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
            item = data.shopList[index];
        }
        title.text = item.displayName;
        description.text = item.description;
        itemImage.overrideSprite = item.sprite;
        string stat;
        if (data.gold >= item.cost*5) {
            button.text = "Buy";
            stat = "cost: "+item.cost*5+"\n";
        } else {
            button.text = "-";
            stat = "cost: <color=#700000>"+item.cost*5+"\n";
        }

        // Display various types
        if (item.itemType == "Weapon") {
            Weapon wep = item as Weapon;
            //string stat = "Dmg: "+wep.mindmg+"-"+wep.maxdmg+"\n";
            /*if (wep.atk > 0) {
                stat += "\natk +"+wep.atk;
            } else if (wep.atk < 0) {
                stat += "\natk "+wep.atk;
            }*/
            stat += InventoryController.ColorStat("Dmg: ", wep.mindmg+wep.maxdmg,
                player.weapon.mindmg+player.weapon.maxdmg, wep);
            stat += InventoryController.ColorStat("atk ", wep.atk, player.weapon.atk);
            stat += InventoryController.ColorStat("def ", wep.def, player.weapon.def);
            stat += InventoryController.ColorStat("speed ", wep.speed, player.weapon.speed);
        } else if (item.itemType == "Armor") {
            Armor arm = item as Armor;
            if (player.armor != null) {
                stat += InventoryController.ColorStat("Def: ", arm.def, player.armor.def);
                stat += InventoryController.ColorStat("speed ", arm.speed, player.armor.speed);
            } else {
                stat += InventoryController.ColorStat("Def: ", arm.def, 0);
                stat += InventoryController.ColorStat("speed ", arm.speed, 1f);
            }
        } else if (item.itemType == "Potion") {
            Potion pot = item as Potion;
            stat += "";
            if (pot.healing > 0) {
                stat += "+"+pot.healing+" hp";
            }
        } else if (item.itemType == "Food") {
            Food food = item as Food;
            stat += "Food: "+food.food/10+"%";
            if (food.damage > 0) {
                stat += "\n-"+food.damage+" hp";
            } else if (food.healing > 0) {
                stat += "\n+"+food.healing+" hp";
            }
        }
        stats.text = stat;
    }

    public void BuyItem() {
        InventoryItem item = data.shopList[selected];
        if (data.gold >= item.cost*5) {
            data.AddToInventory(item.Copy());
            data.gold -= item.cost*5;
            gold.text = data.gold.ToString();
        }
    }

    public void BackToScene() {
        eventSystem.SetActive(false);
        SceneManager.UnloadSceneAsync("Shop");
        root.SetActive(true);
        player.enabled = true;
    }
}
