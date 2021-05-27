using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventoryItem {
    
    public string itemType, name, description;
    public Sprite sprite;
    public InventoryItem() {

    }

    public override string ToString() {
        return "Parent class for Inventory items, you should never see this. ";
    }

    // Used to activate items in inventory, if possible
    public void Activate() {}
}