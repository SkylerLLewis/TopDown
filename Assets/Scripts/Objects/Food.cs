using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : InventoryItem {
    public int healing, damage, food;
    public float speed;
    public static List<List<string>> FoodTiers = new List<List<string>>() {
        new List<string>() {"Moldy Bread", "Moldy Loaf"},
        new List<string>() {"Burnt Toast", "Roast Squirrel"}
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
        if (FoodTiers[0].Contains(name)) {
            tier = 1;
            if (name == "Moldy Bread") {
                description = "You should really invest in some tupperware.";
                damage = 4;
                food = 100;
                cost = 1;
            } else if (name == "Moldy Loaf") {
                description = "That's... a lot of mold.";
                damage = 9;
                food = 500;
                cost = 10;
            }
        } else if (FoodTiers[1].Contains(name)) {
            tier = 2;
            if (name == "Burnt Toast") {
                description = "Mmmm, crunchy!";
                damage = 1;
                food = 100;
                cost = 5;
            } else if (name == "Roast Squirrel") {
                description = "Hey, that's actually pretty tasty!";
                food = 200;
                cost = 15;
            }
        }
    }

    public override void Activate(PlayerController player) {
        count--;
        player.Feed(food);
        if (damage > 0) {
            player.Damage(damage, "dmg", combat:false);
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