using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemController : MonoBehaviour {

    int itemIndex;
    PersistentData data;
    InventoryController controller;
    void Start() {
        // Get container's canvas's controller script
        // That's a doozy
        controller = transform.parent.parent.gameObject.GetComponent<InventoryController>();
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
    }

    public void SelectItem() {
        Debug.Log("The Item "+data.inventory[itemIndex].name+" has been selected!");
        controller.DisplayItem(itemIndex);
    }

    public void SetItemIndex(int index) {
        itemIndex = index;
    }
}
