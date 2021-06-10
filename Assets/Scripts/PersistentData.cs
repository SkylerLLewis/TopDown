using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public int depth, entrance;
    public string direction, loadedMenu;
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
        direction = "down";
        // Inventory
        inventory = new List<InventoryItem>();
        weapon = new Weapon("Twig");
        armor = null;
        Food bread = new Food("Moldy Bread");
        bread.count = 8;
        inventory.Add(bread);
        inventory.Add(new Potion("Health Potion"));
        // Shopkeeper List
        shopList = new List<InventoryItem>();
        List<string> weaponOptions = Sample<string>(4, Weapon.WeaponTiers[0]);
        foreach (string item in weaponOptions) {
            shopList.Add(new Weapon(item));
        }
        weaponOptions = Sample<string>(2, Weapon.WeaponTiers[1]);
        foreach (string item in weaponOptions) {
            shopList.Add(new Weapon(item));
        }
        shopList.Add(new Weapon("Woodcutter's Axe"));
        shopList.Add(new Armor(Armor.ArmorTiers[0][Random.Range(0, Armor.ArmorTiers[0].Count)]));
        shopList.Add(new Potion("Health Potion"));
        shopList.Add(new Food("Moldy Bread"));
        shopList.Sort(InventoryController.CompareItems);
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

    public static List<T> Sample<T>(int count, in List<T> list) {
        List<T> newList = new List<T>();
        for (int i=0; i<list.Count; i++) {
            if (Random.value < (float)count/(list.Count-i)) {
                newList.Add(list[i]);
                count--;
                if (count == 0) break;
            }
        }
        return newList;
    }
}
