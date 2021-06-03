using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public int depth;
    public int entrance;
    public string direction;
    public int playerHp;
    public float food;
    public Weapon weapon;
    public Armor armor;
    public GameObject root;
    public List<InventoryItem> inventory;
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        depth = 0;
        entrance = 0;
        playerHp = 0;
        food = 500;
        direction = "down";
        inventory = new List<InventoryItem>();
        weapon = new Weapon("Twig");
        armor = null;
        inventory.Add(new Potion("Healing Potion"));
        SceneManager.LoadScene("GreenVillage");
    }

    public void AddToInventory(InventoryItem item) {
        if (item.itemType == "Weapon" || item.itemType == "Armor") {
            inventory.Add(item);
        } else if (item.itemType == "Potion") {
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
        }
    } 
}
