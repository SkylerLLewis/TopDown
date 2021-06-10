using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : InventoryItem {
    public int def;
    public float speed;
    public static List<List<string>> ArmorTiers = new List<List<string>>() {
        new List<string>() {"Leather Tunic", "Cast Iron Plates"},
        new List<string>() {"Patchy Brigandine"},
        new List<string>() {}
    };
    public Armor(string n, int qual=-2) {
        itemType = "Armor";
        name = n;
        sprite = Resources.Load<Sprite>("Armors/"+name);
        if (sprite == null) {
            Debug.Log("Armor name \""+name+"\" does not exist.");
        }
        if (qual == -2) {
            int rand = Random.Range(1,21);
            if (rand <= 2) {
                quality = -1; // Shitty (10%)
            } else if (rand <= 15) {
                quality = 0; // Normal (65%)
            } else if (rand <= 17) {
                quality = 1; // Good (10%)
            } else if (rand <= 19) {
                quality = 2; // Fine (10%)
            } else if (rand == 20) {
                quality = 3; // Masterwork (5%)
            }
        } else {
            quality = qual;
        }
        Classify();
    }

    private void Classify() {
        // Note - speed value is multiplicitive (Higher is better)
        speed = 1f;
        // -- Tier 1 Starter Armor -- //
        if (ArmorTiers[0].Contains(name)) {
            tier = 1;
            cost = 10;
            // Tier 1s have no quality, they're all shit
            if (quality != 0) { quality = 0; }
            if (name == "Leather Tunic") {
                description = "An old, smelly tunic. Hope those blood stains weren't the previous owner's....";
                def = 2;
            } else  if (name == "Cast Iron Plates") {
                description = "Probably made by some hobgoblin, it looks to be crudely strapped pieces of pots and cauldrons.";
                def = 8;
                speed = 0.7f;
            }
        } else if (ArmorTiers[1].Contains(name)) {
            tier = 2;
            cost = 50;
            // Tier 2s are good or worse
             if (quality > 1) {
                quality = 1;
            }
            if (name == "Patchy Brigandine") {
                description = "Made from scrap metal hammered between sheets of leather, this heavy armor keeps out even deep slices.";
                def = 6;
                speed = 0.9f + 0.05f*quality;
            }
        } else if (ArmorTiers[2].Contains(name)) {
            tier = 3;
            cost = 200;
            // Tier 3 Armor can only be fine or worse
            if (quality > 2) {
                quality = 2;
            }
        }

        // Quality affects cost exponentially
        cost = Mathf.RoundToInt(cost * Mathf.Pow(2, quality));
        if (quality == -1) {
            displayName = "Shitty "+name;
            description = description+" This piece is very poorly made.";
        } else if (quality == 0) {
            displayName = name;
        } else if (quality == 1) {
            displayName = "Good "+name;
            description = description+" This armor is high quality.";
        } else if (quality == 2) {
            displayName = "Fine "+name;
            description = description+" This armor is of the finest make.";
        } else if (quality == 3) {
            displayName = "Masterwork "+name;
            description = description+" Whoever crafted this armor was a true master.";
        }
    }

    public override InventoryItem Copy()
    {
        return new Armor(this.name, this.quality);
    }

    public static bool IsArmor(string s) {
        foreach (List<string> tier in ArmorTiers) {
            if (tier.Contains(s)) {
                return true;
            }
        }
        return false;
    }
}