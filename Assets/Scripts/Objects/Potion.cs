using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : InventoryItem {
    public int healing, mana, regen;
    public float duration, speed;
    public static List<List<string>> PotionTiers = new List<List<string>>() {
        new List<string>() {"Health Potion", "Mana Potion", "Potion of Speed"},
        new List<string>() {"Potion of Regeneration"}
    };
    public Potion(string n) {
        itemType = "Potion";
        name = n;
        displayName = n;
        count = 1;
        sprite = Resources.Load<Sprite>("Potions/"+name);
        if (sprite == null) {
            Debug.Log("Potion name \""+name+"\" does not exist.");
        }
        Classify();
    }

    private void Classify() {
        healing = 0;
        regen = 0;
        mana = 0;
        duration = 0f;
        speed = 1f;
        if (PotionTiers[0].Contains(name)) {
            cost = 4;
            if (name == "Health Potion") {
                description = "A pungent, herbal smelling healing potion.";
                healing = 20;
            } else if (name == "Mana Potion") {
                description = "You see swirls of color in the glowing blue liquid.";
                mana = 20;
            } else if (name == "Potion of Speed") {
                description = "Static shocks your hand when you touch it.";
                speed = 1.5f;
                duration = 20f;
            }
        } else if (PotionTiers[1].Contains(name)) {
            cost = 10;
            if (name == "Potion of Regeneration") {
                description = "This swirling blue-green potion smells earthy.";
                regen = 1;
                duration = 20f;
            }
        }
    }

    public override void Activate(PlayerController player) {
        count--;
        if (healing > 0) {
            player.Heal(healing);
        }
        if (mana > 0) {
            player.GetMana(mana);
        }
        if (speed != 1f) {
            player.SpeedEffect(speed, duration);
        }
        if (regen != 0) {
            player.RegenEffect(regen, duration);
        }
    }

    public override InventoryItem Copy()
    {
        return new Potion(this.name);
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