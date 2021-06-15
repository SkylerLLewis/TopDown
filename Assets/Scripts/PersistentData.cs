using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public int depth, direction, entrance;
    public string floorDirection, loadedMenu, mapType;
    public int playerHp, gold;
    public float food;
    public Weapon weapon;
    public Armor armor;
    public GameObject root;
    public List<InventoryItem> inventory, shopList;
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        depth = 0;
        entrance = 0;
        gold = 0;
        playerHp = 0;
        food = 500;
        direction = 2;
        floorDirection = "down";
        // Inventory
        inventory = new List<InventoryItem>();
        weapon = new Weapon("Twig");
        armor = null;
        Food bread = new Food("Moldy Bread");
        bread.count = 8;
        inventory.Add(bread);
        inventory.Add(new Food("Roast Squirrel"));
        inventory.Add(new Potion("Health Potion"));
        // Shopkeeper List
        shopList = new List<InventoryItem>();
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
        shopList.Add(new Potion("Health Potion"));
        shopList.Add(new Food("Moldy Bread"));
        shopList.Add(new Food("Moldy Loaf"));
        shopList.Add(new Food("Burnt Toast"));
        shopList.Add(new Food("Roast Squirrel"));
        shopList.Sort(InventoryController.CompareItems);
        shopList.Reverse();
        SceneManager.LoadScene("GreenVillage");
    }

    public void AddToInventory(InventoryItem item) {
        if (item.itemType == "Potion" || item.itemType == "Food") {
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
}
