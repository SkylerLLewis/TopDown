using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem {
    public int mindmg, maxdmg, atk, def, crit;
    public float speed, manaRegen, attackSpeed;
    public bool ranged;
    public static List<List<string>> WeaponTiers = new List<List<string>>() {
        new List<string>() {"Twig", "Sharp Twig", "Plank with a Nail", "Club", "Long Stick", "Log", "Staff", "Crude Bow"},
        new List<string>() {"Rusty Shortsword", "Half a Scissor", "Copper Hatchet", "Mallet", "Flint Spear", "Dog Chain", "Cursed Dagger", "Shortbow"},
        new List<string>() {"Dueling Sword", "Hunting Knife", "Woodcutter's Axe", "Hammer", "Wooden Pike", "Grain Scythe", "Magewood Blade", "Light Crossbow"}
    };
    public Weapon(string n, int qual=-2) {
        itemType = "Weapon";
        name = n;
        sprite = Resources.Load<Sprite>("Weapons/"+name);
        if (sprite == null) {
            Debug.Log("Weapon name \""+name+"\" does not exist.");
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
        atk = 0;
        def = 0;
        crit = 2;
        ranged = false;
        attackSpeed = 1;
        // -- Tier 1 Starter Weapons -- //
        // Tier 1s have an average of 2 dmg and +10% effect
        if (WeaponTiers[0].Contains(name)) {
            tier = 1;
            cost = 1;
            // Tier 0s have no quality, they're all shit
            if (quality != 0) { quality = 0; }
            if (name == "Twig") {
                description = "It's a twig. This is a terrible idea.";
                mindmg = 1;
                maxdmg = 3;
            } else if (name == "Sharp Twig") { // Dagger type
                description = "A particularly spiky twig. Fast and quiet, but you'll have to get close.";
                mindmg = 1;
                maxdmg = 2;
                speed = 1.15f;
                atk = 2;
                def = -3;
                crit = 4;
            } else if (name == "Plank with a Nail") { // Axe type
                description = "This nail should punch through armor at least.";
                mindmg = 1;
                maxdmg = 3;
                atk = 2;
            } else if (name == "Club") { // Mace Type
                description = "It's slow, but it hits hard.";
                mindmg = 1;
                maxdmg = 4;
                speed = 0.9f;
            } else if (name == "Long Stick") { // Spear type
                description = "Now this should keep them back.";
                mindmg = 1;
                maxdmg = 3;
                def = 2;
            } else if (name == "Log") { // Polearm type
                description = "What am dex? Me have big bonk! Pathetic weaklings stay back.";
                mindmg = 2;
                maxdmg = 3;
                atk = 3;
                def = 3;
                speed = 0.80f;
            } else if (name == "Staff") { // Mage weapon type
                description = "A staff made of wood from the enchanted forest.";
                mindmg = 1;
                maxdmg = 2;
                manaRegen = 0.2f;
                speed = 1.05f;
            } else if (name == "Crude Bow") { // ranged weapon type
                description = "Nothing more than a bent piece of wood and twine. It's very hard to pull back.";
                mindmg = 2;
                maxdmg = 3;
                attackSpeed = 0.5f;
                ranged = true;
            }
        // -- Tier 2 Weapons -- //
        // Tier 2s have average 3 dmg  and +25% extra effect
        // Each tier is 10% effect
        } else if (WeaponTiers[1].Contains(name)) {
            tier = 2;
            cost = 5;
            // Tier 1 weapons can only be good or worse
            if (quality > 1) {
                quality = 1;
            }
            if (name == "Rusty Shortsword") {
                description = "Finally, a real weapon! The rust is so thick you can barely see the iron. Probably should avoid hitting anything too hard...";
                mindmg = 2;
                maxdmg = 4;
                speed = 1.1f + 0.05f*quality;
            } else if (name == "Half a Scissor") {
                description = "You know what, it'll work. It's even got a convenient thumb loop!";
                mindmg = 1;
                maxdmg = 3;
                speed = 1.15f + 0.05f*quality;
                atk = 3;
                def = -4;
                crit = 4;
            } else if (name == "Copper Hatchet") {
                description = "This hatchet should bite deep. It has a nice shine to it.";
                mindmg = 2;
                maxdmg = 5;
                atk = 2 + 2*quality;
            } else if (name == "Mallet") {
                description = "I'm gettin me mallet!";
                mindmg = 3 + quality;
                maxdmg = 6;
                speed = 0.9f;
            } else if (name == "Flint Spear") {
                description = "It smells faintly of mammoth blood.";
                mindmg = 2;
                maxdmg = 4;
                def = 2 + 2*quality;
            } else if (name == "Dog Chain") {
                description = "Stake still attached! You'd hate to meet whatever chewed through it...";
                mindmg = 2;
                maxdmg = 6;
                speed = 0.85f + 0.05f*quality;
                atk = 2;
                def = 3;
            } else if (name == "Cursed Dagger") {
                description = "Malice radiates from this dagger like heat from an oven.";
                mindmg = 2;
                maxdmg = 4;
                def = -6 - quality;
                manaRegen = 0.35f + 0.05f*quality;
                speed = 1.10f;
            } else if (name == "Shortbow") { // ranged weapon type
                description = "A small, light bow. Great for travel and rabbit hunting! Not as good for killing.";
                mindmg = 2;
                maxdmg = 6;
                attackSpeed = 0.60f + 0.1f * quality;
                ranged = true;
            }
        // -- Tier 3 Weapons -- //
        // Tier 3 weapons have average 4 dmg and + 50% effect
        } else if (WeaponTiers[2].Contains(name)) {
            tier = 3;
            cost = 20;
            // Tier 2 weapons can only be fine or worse
            if (quality > 2) {
                quality = 2;
            }
            if (name == "Dueling Sword") {
                description = "An antique dueling blade, long and thin. It looks precise.";
                mindmg = 1;
                maxdmg = 7;
                atk = 2 + 1*quality;
                def = 2;
                crit = 3;
                speed = 1.05f + 0.025f*quality;
            } else if (name == "Hunting Knife") {
                description = "A barbed, wicked looking dagger for killing and skinning game.";
                mindmg = 1;
                maxdmg = 9;
                def = -3;
                crit = 4;
                speed = 1.1f + 0.05f*quality;
            } else if (name == "Woodcutter's Axe") {
                description = "A solid iron axe, good for cleaving.";
                mindmg = 2;
                maxdmg = 8;
                atk = 5 + 2*quality;
            } else if (name == "Hammer") {
                description = "Made for smashing.";
                mindmg = 4 + 2*quality;
                maxdmg = 8;
            } else if (name == "Wooden Pike") {
                description = "Was this a signpost?";
                mindmg = 2 + quality;
                maxdmg = 6 + quality;
                def = 10;
            } else if (name == "Grain Scythe") {
                description = "It's big, clunky, and the blade is backwards. But it has a blade!";
                mindmg = 4;
                maxdmg = 6;
                speed = 1.05f + 0.05f*quality;
                def = 3;
            } else if (name == "Magewood Blade") {
                description = "This arming sword has a hilt made from magic wood.";
                mindmg = 2;
                maxdmg = 8;
                manaRegen = 0.25f;
                speed = 1 + 0.05f*quality;
            } else if (name == "Light Crossbow") {
                description = "A crossbow small enough to conceal, and more than powerful enough to get the job done.";
                mindmg = 4;
                maxdmg = 6;
                attackSpeed = 0.5f;
                atk = 2 + 2*quality;
                ranged = true;
            }
        } else {
            Debug.LogError("Weapon name "+name+" not found!");
        }

        // Quality affects cost exponentially
        cost = Mathf.RoundToInt(cost * Mathf.Pow(2, quality));
        if (quality == -1) {
            displayName = "Shitty "+name;
            description = description+" This one is very poorly made.";
        } else if (quality == 0) {
            displayName = name;
        } else if (quality == 1) {
            displayName = "Good "+name;
            description = description+" This weapon is high quality.";
        } else if (quality == 2) {
            displayName = "Fine "+name;
            description = description+" This weapon is of the finest make.";
        } else if (quality == 3) {
            displayName = "Masterwork "+name;
            description = description+" Whoever crafted this weapon was a true master.";
        }
    }
    
    public static bool IsWeapon(string s) {
        foreach (List<string> tier in WeaponTiers) {
            if (tier.Contains(s)) {
                return true;
            }
        }
        return false;
    }

    public override InventoryItem Copy()
    {
        return new Weapon(this.name, this.quality);
    }

    public override string ToString()
    {
        return description;
    }
}