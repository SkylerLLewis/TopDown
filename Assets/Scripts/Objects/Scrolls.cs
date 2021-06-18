using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scroll : InventoryItem {
    public int healing, mana;
    public float duration, speed;
    public static List<List<string>> ScrollTiers = new List<List<string>>() {
        new List<string>() {"Scroll of Return"}
    };
    public Scroll(string n) {
        itemType = "Scroll";
        name = n;
        displayName = n;
        count = 1;
        sprite = Resources.Load<Sprite>("Scrolls/"+name);
        if (sprite == null) {
            Debug.Log("Scroll name \""+name+"\" does not exist.");
        }
        Classify();
    }

    private void Classify() {
        healing = 0;
        duration = 0f;
        speed = 1f;
        if (name == "Scroll of Return") {
            description = "This scroll will take me straight back to the surface";
            cost = 2;
        }
    }

    public override void Activate(PlayerController player) {
        count--;
        if (name == "Scroll of Return") {
            PersistentData data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
            data.depth = 1;
            player.dungeonController.NotableActionsRef("stairsUp");
        }
    }

    public override InventoryItem Copy()
    {
        return new Scroll(this.name);
    }

    public static bool IsScroll(string s) {
        foreach (List<string> tier in ScrollTiers) {
            if (tier.Contains(s)) {
                return true;
            }
        }
        return false;
    }
}