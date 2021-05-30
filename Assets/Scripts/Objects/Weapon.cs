using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem {
    public int mindmg, maxdmg, atk, def, quality;
    public float speed;
    public static List<List<string>> WeaponTiers = new List<List<string>>() {
        new List<string>() {"Twig", "Sharp Twig", "Plank with a Nail", "Club", "Long Stick", "Log"},
        new List<string>() {"Rusty Shortsword", "Half a Scissor", "Copper Hatchet", "Mallet", "Flint Spear", "Grain Scythe"},
        new List<string>() {"Woodcutter's Axe"}
    };
    public Weapon(string n) {
        itemType = "Weapon";
        name = n;
        sprite = Resources.Load<Sprite>("Weapons/"+name);
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
        // Note - speed value is multiplicitive (Lower is better)
        speed = 1f;
        atk = 0;
        def = 0;
        // -- Tier 0 Starter Weapons -- //
        if (WeaponTiers[0].Contains(name)) {
            // Tier 0s have no quality, they're all shit
            if (quality != 0) { quality = 0; }
            if (name == "Twig") {
                description = "It's a twig. This is a terrible idea.";
                mindmg = 1;
                maxdmg = 2;
            } else if (name == "Sharp Twig") { // Dagger type
                description = "A particularly spiky twig. Fast and quiet, but you'll have to get close.";
                mindmg = 1;
                maxdmg = 1;
                speed = 0.8f;
                atk = 2;
                def = -2;
            } else if (name == "Plank with a Nail") { // Axe type
                description = "This nail should punch through armor at least.";
                mindmg = 1;
                maxdmg = 3;
                atk = 1;
            } else if (name == "Club") { // Mace Type
                description = "It's slow, but it hits hard.";
                mindmg = 2;
                maxdmg = 3;
                speed = 1.2f;
            } else if (name == "Long Stick") { // Spear type
                description = "Now this should keep them back.";
                mindmg = 1;
                maxdmg = 2;
                def = 2;
            } else if (name == "Log") { // Polearm type
                description = "What am dex? Me have big bonk! Pathetic weaklings stay back.";
                mindmg = 2;
                maxdmg = 3;
                atk = 2;
                def = 2;
                speed = 1.5f;
            }
        // -- Tier 1 Weapons -- //
        } else if (WeaponTiers[1].Contains(name)) {
            // Tier 1 weapons can only be good or worse
             if (quality > 1) {
                quality = 1;
            }
            if (name == "Rusty Shortsword") {
                description = "Finally, a real weapon! The rust is so thick you can barely see the iron. Probably should avoid hitting anything too hard...";
                mindmg = 2;
                maxdmg = 4;
                speed = 0.9f - 0.1f*quality;
            } else if (name == "Half a Scissor") {
                description = "You know what, it'll work. It's even got a convenient thumb loop!";
                mindmg = 1;
                maxdmg = 3;
                speed = 0.8f - 0.1f*quality;
                atk = 3;
                def = -2;
            } else if (name == "Copper Hatchet") {
                description = "This hatchet should bite deep. It has a nice shine to it. At least it's pretty.";
                mindmg = 2;
                maxdmg = 5;
                atk = 3 + 2*quality;
            } else if (name == "Mallet") {
                description = "I'm gettin me mallet!";
                mindmg = 4 + 2*quality;
                maxdmg = 6;
                speed = 1.20f;
            } else if (name == "Flint Spear") {
                description = "It smells faintly of mammoth blood.";
                mindmg = 2;
                maxdmg = 4;
                def = 2 + 2*quality;
            } else if (name == "Grain Scythe") {
                description = "It's big, clunky, and the blade is backwards. But it has a blade!";
                mindmg = 4;
                maxdmg = 6;
                speed = 1.5f - 0.2f*quality;
                def = 3;
                atk = 3;
            }
        // -- Tier 2 Weapons -- //
        } else if (WeaponTiers[2].Contains(name)) {
            // Tier 2 weapons can only be fine or worse
             if (quality > 2) {
                quality = 2;
            }
            if (name == "Woodcutter's Axe") {
                description = "A solid iron axe, good for cleaving.";
                mindmg = 3;
                maxdmg = 8;
                atk = 4 + 2*quality;
            }
        }
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

    public override string ToString()
    {
        return description;
    }
}