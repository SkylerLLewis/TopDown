using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : InventoryItem {
    public int def, quality;
    public float speed;
    public static List<List<string>> ArmorTiers = new List<List<string>>() {
        new List<string>() {"Leather Tunic", "Cast Iron Plates"},
        new List<string>() {"Patchy Brigandine"},
        new List<string>() {}
    };
    public Armor(string n) {
        itemType = "Armor";
        name = n;
        sprite = Resources.Load<Sprite>("Armors/"+name);
        if (sprite == null) {
            Debug.Log("Armor name \""+name+"\" does not exist.");
        }
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
        Classify();
    }

    private void Classify() {
        // Note - speed value is multiplicitive (Higher is better)
        speed = 1f;
        // -- Tier 0 Starter Armor -- //
        if (ArmorTiers[0].Contains(name)) {
            // Tier 0s have no quality, they're all shit
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
            // Tier 1s are good or worse
             if (quality > 1) {
                quality = 1;
            }
            if (name == "Patchy Brigandine") {
                description = "Made from scrap metal hammered between sheets of leather, this heavy armor keeps out even deep slices.";
                def = 6;
                speed = 0.9f + 0.05f*quality;
            }
        } else if (ArmorTiers[2].Contains(name)) {
            // Tier 2 Armor can only be fine or worse
            if (quality > 2) {
                quality = 2;
            }
        }
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

    public static bool IsArmor(string s) {
        foreach (List<string> tier in ArmorTiers) {
            if (tier.Contains(s)) {
                return true;
            }
        }
        return false;
    }
}