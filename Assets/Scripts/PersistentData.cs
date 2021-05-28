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
    public Weapon weapon;
    public GameObject root;
    public List<InventoryItem> inventory;
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        depth = 0;
        entrance = 0;
        playerHp = 0;
        direction = "down";
        inventory = new List<InventoryItem>();
        weapon = new Weapon("Twig");
        inventory.Add(new Weapon("Sharp Twig"));
        inventory.Add(new Weapon("Plank with a Nail"));
        inventory.Add(new Weapon("Club"));
        inventory.Add(new Weapon("Long Stick"));
        inventory.Add(new Weapon("Log"));
        inventory.Add(new Weapon("Rusty Shortsword"));
        inventory.Add(new Weapon("Half a Scissor"));
        inventory.Add(new Weapon("Copper Hatchet"));
        inventory.Add(new Weapon("Mallet"));
        inventory.Add(new Weapon("Flint Spear"));
        inventory.Add(new Weapon("Grain Scythe"));
        SceneManager.LoadScene("GreenVillage");
    }
}
