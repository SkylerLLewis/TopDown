using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryItemController : MonoBehaviour {

    int itemIndex;
    PersistentData data;
    InventoryController controller;
    System.Action<int> DisplayItem;
    void Start() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get container's canvas's controller script
        // That's a doozy
        if (data.loadedMenu == "Inventory") {
            DisplayItem = transform.parent.parent.gameObject.GetComponent<InventoryController>().DisplayItem;
        } else if (data.loadedMenu == "Shop") {
            DisplayItem = transform.parent.parent.gameObject.GetComponent<ShopController>().DisplayItem;
        }
    }

    public void SelectItem() {
        DisplayItem(itemIndex);
    }

    public void SetItemIndex(int index) {
        itemIndex = index;
    }
}
