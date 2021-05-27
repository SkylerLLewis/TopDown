using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    private PersistentData data;
    private GameObject root, itemArray;
    PlayerController player;
    void Start() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get inactive Root object stored in persistent data
        root = data.root;
        root.SetActive(false);
        player = root.GetComponentInChildren<PlayerController>();

        foreach (Transform child in gameObject.transform) {
            if (child.name == "ItemArray") {
                itemArray = child.gameObject;
                break;
            }
        }

        // Display List of Items
        DisplayItems();
    }

    public void DisplayItems() {
        GameObject itemButton = Resources.Load("Prefabs/InventoryItemButton") as GameObject;
        int i=0, j=0;
        float x, y;
        foreach (InventoryItem item in data.inventory) {
            x = -7.5f + i*1.25f;
            y = 4f - j*1.25f;
            GameObject clone = Instantiate(
                itemButton,
                itemArray.transform);
            RectTransform rt = clone.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3(x, y, 0);
            clone.GetComponent<Image>().overrideSprite =  item.sprite;
            clone.GetComponent<InventoryItemController>().SetItemIndex(i);
            Debug.Log("Added item "+item.name+" to inventory display.");
            i++;
            if (i%10 == 9) { j++; }
        }
    }

    public void EquipWeapon(int index) {
        data.weapon = data.inventory[index] as Weapon;
        player.EquipWeapon(data.inventory[index] as Weapon);
    }

    public void BackToScene() {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("GreenVillage"));
        SceneManager.UnloadSceneAsync("Inventory");
        root.SetActive(true);
    }

}
