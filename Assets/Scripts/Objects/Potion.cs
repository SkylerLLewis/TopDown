using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : InventoryItem {
    public int healing, quality;
    public float speed;
    public static List<List<string>> PotionTiers = new List<List<string>>() {
        new List<string>() {"Healing Potion"}
    };
    public Potion(string n) {
        itemType = "Potion";
        name = n;
        displayName = n;
        sprite = Resources.Load<Sprite>("Potions/"+name);
        if (sprite == null) {
            Debug.Log("Potion name \""+name+"\" does not exist.");
        }
        Classify();
    }

    private void Classify() {
        healing = 0;
        speed = 0f;
        if (name == "Healing Potion") {
            description = "A pungent, herbal smelling healing potion.";
            healing = 20;
        }
    }

    public override void Activate(PlayerController player) {
        if (name == "Healing Potion") {
            player.hp += healing;
            if (player.hp > player.maxhp) {
                player.hp = player.maxhp;
            }
            player.FloatText("heal", "20");
        }
    }

    public static bool IsPotion(string s) {
        foreach (List<string> tier in PotionTiers) {
            if (tier.Contains(s)) {
                return true;
            }
        }
        return false;
    }
}