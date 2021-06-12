using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : InventoryItem {
    public int def, dmg, armor;
    public float speed;
    public static List<List<string>> ArmorTiers = new List<List<string>>() {
        new List<string>() {"Leather Tunic", "Bone Armor"},
        new List<string>() {"Fang Bracers", "Padded Vest", "Cast Iron Plates"},
        new List<string>() {"Light Leather Armor", "Gambeson", "Patchy Brigandine"}
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
        dmg = 0;
        armor = 0;
        // -- Tier 1 Starter Armor -- //
        if (ArmorTiers[0].Contains(name)) {
            tier = 1;
            cost = 10;
            // Tier 1s have no quality, they're all shit
            // Tier 1 comes with 10% effect
            if (quality != 0) { quality = 0; }
            if (name == "Leather Tunic") {
                description = "An old, smelly tunic. Hope those blood stains weren't the previous owner's....";
                def = 2;
            } else  if (name == "Bone Armor") {
                description = "Probably made by some hobgoblin, it looks to be crudely strapped pieces thick bone on leather.";
                def = 5;
                speed = 0.85f;
            }
        } else if (ArmorTiers[1].Contains(name)) {
            tier = 2;
            cost = 50;
            // Tier 2s are good or worse
            // Tier 2 comes with 20% effect
             if (quality > 1) {
                quality = 1;
            }
            if (name == "Fang Bracers") {
                description = "Sharp, long fangs extend from these leather bracers. No one's escaping these unscathed.";
                def = -2 + 1*quality;
                dmg = 1;
            } else if (name == "Padded Vest") {
                description = "A simple stitched and padded vest, light but thick.";
                def = 4;
                speed = 1f + 0.05f*quality;
            } else if (name == "Cast Iron Plates") {
                description = "Someone desperate must have strapped these pieces of cauldron together for armor.";
                def = 8 + 1*quality;
                speed = 0.8f;
            }
        } else if (ArmorTiers[2].Contains(name)) {
            tier = 3;
            cost = 200;
            // Tier 3 Armor can only be fine or worse
            // Tier 3 comes with 30% effect
            if (quality > 2) {
                quality = 2;
            }
            if (name == "Light Leather Armor") {
                description = "Just enough extra hide in a few important places to stay light on your feet.";
                def = 4;
                speed = 1.1f + 0.1f*quality;
            } else if (name == "Gambeson") {
                description = "Thick padded armor, issued to footsoldiers and archers.";
                def = 6 + 1*quality;
                speed = 1f + 0.05f*quality;
            } else if (name == "Patchy Brigandine") {
                description = "Made from scrap metal hammered between sheets of leather, this heavy armor keeps out even deep slices.";
                def = 6 + 2*quality;
                speed = 0.75f;
                armor = 1;
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