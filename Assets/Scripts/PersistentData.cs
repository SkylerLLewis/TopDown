using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public string floorDirection, loadedMenu, mapType, sceneToLoad, sceneToUnload;
    public int depth, direction, entrance,
               playerHp, mana, gold, baseHp, baseMana,
               xp, level, nextLevel, skillPoints;
    public float food;
    public List<PlayerController.Effect> playerEffects;
    public Weapon weapon;
    public Armor armor;
    public GameObject root;
    public LoadScreenController loadingScreen;
    public List<InventoryItem> inventory, shopList;
    public List<string> followingEnemies, activeSkills;
    public List<Skill> skills;
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        loadingScreen = GameObject.Find("LoadingScreen").GetComponent<LoadScreenController>();
        level = 0;
        xp = 0;
        nextLevel = Mathf.RoundToInt(50 * Mathf.Pow(2, level/4f));
        depth = 0;
        entrance = 0;
        gold = 5;
        playerHp = 0;
        mana = 0;
        baseHp = 20;
        baseMana = 10;
        food = 500;
        direction = 2;
        floorDirection = "down";
        followingEnemies = new List<string>();
        // Inventory
        inventory = new List<InventoryItem>();
        weapon = new Weapon("Twig");
        armor = null;
        Food bread = new Food("Moldy Bread");
        bread.count = 8;
        inventory.Add(bread);
        inventory.Add(new Food("Roast Squirrel"));
        inventory.Add(new Potion("Health Potion"));
        inventory.Add(new Scroll("Scroll of Return"));
        inventory.Add(new Scroll("Scroll of Descent"));
        // Shopkeeper List
        shopList = new List<InventoryItem>();
        LoadShopList();
        // Skills
        skills = new List<Skill>();
        skillPoints = 0;
        activeSkills = new List<string>();
    }

    void Start() {
        LoadingScreenLoad("GreenVillage", "village");
    }

    public void LoadingScreenLoad(string sceneName, string style) {
        loadingScreen.LoadNextScene(sceneName, style);
    }

    public void AddToInventory(InventoryItem item) {
        if (item.itemType == "Potion" || item.itemType == "Food" || item.itemType == "Scroll") {
            bool contains = false;
            int index = 0;
            foreach (InventoryItem i in inventory) {
                if (i.name == item.name) {
                    contains = true;
                    break;
                }
                index++;
            }
            if (contains) {
                inventory[index].count++;
            } else {
                inventory.Add(item);
            }
        } else {
            inventory.Add(item);
        }
    }

    public bool RemoveFromInventory(int index) {
        InventoryItem item = inventory[index];
        if (item.itemType == "Potion" || item.itemType == "Food") {
            if (item.count > 1) {
                inventory[index].count--;
                return false;
            } else {
                inventory.RemoveAt(index);
            }
        } else {
            inventory.RemoveAt(index);
        }
        return true;
    }

    public void LoadShopList() {
        shopList.Clear();
        List<string> weaponOptions = Utilities.Sample(4, Weapon.WeaponTiers[0]);
        foreach (string item in weaponOptions) {
            shopList.Add(new Weapon(item));
        }
        weaponOptions = Utilities.Sample(3, Weapon.WeaponTiers[1]);
        foreach (string item in weaponOptions) {
            shopList.Add(new Weapon(item));
        }
        shopList.Add(new Weapon(Utilities.Choice(Weapon.WeaponTiers[2])));
        shopList.Add(new Armor(Utilities.Choice(Armor.ArmorTiers[0])));
        shopList.Add(new Armor(Utilities.Choice(Armor.ArmorTiers[1])));
        shopList.Add(new Armor(Utilities.Choice(Armor.ArmorTiers[2])));
        shopList.Add(new Potion(Utilities.Choice(Potion.PotionTiers[0])));
        shopList.Add(new Potion(Utilities.Choice(Potion.PotionTiers[1])));
        shopList.Add(new Scroll("Scroll of Return"));
        shopList.Add(new Food("Moldy Bread"));
        shopList.Add(new Food("Moldy Loaf"));
        shopList.Add(new Food("Burnt Toast"));
        shopList.Add(new Food("Roast Squirrel"));
        shopList.Sort(InventoryController.CompareItems);
        shopList.Reverse();
    }

    public void LevelUp() {
        level++;
        skillPoints++;
        xp -= nextLevel;
        nextLevel = Mathf.RoundToInt(50 * Mathf.Pow(2, level/4f));
    }
}
