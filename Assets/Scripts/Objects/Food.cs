using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : InventoryItem {
    public int healing, damage, food;
    public float speed;
    public static List<List<string>> FoodTiers = new List<List<string>>() {
        new List<string>() {"Moldy bread"}
    };
    public Food(string n) {
        itemType = "Food";
        name = n;
        displayName = n;
        count = 1;
        sprite = Resources.Load<Sprite>("Food/"+name);
        if (sprite == null) {
            Debug.Log("Food name \""+name+"\" does not exist.");
        }
        Classify();
    }

    private void Classify() {
        healing = 0;
        damage = 0;
        speed = 0f;
        if (name == "Moldy Bread") {
            description = "You should really invest in some tupperware.";
            damage = 5;
            food = 100;
            cost = 0;
        }
    }

    public override void Activate(PlayerController player) {
        count--;
        player.Feed(food);
        if (damage > 0) {
            player.Damage(damage, "dmg");
        }
    }

    public override InventoryItem Copy()
    {
        return new Food(this.name);
    }

    public static bool IsFood(string s) {
        foreach (List<string> tier in FoodTiers) {
            if (tier.Contains(s)) {
                return true;
            }
        }
        return false;
    }
}