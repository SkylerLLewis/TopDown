using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem {
    public int mindmg, maxdmg, atk, def;
    public float speed;

    public Weapon(string n) {
        itemType = "Weapon";
        name = n;
        sprite = Resources.Load<Sprite>("Weapons/"+name);
        Classify();
    }

    private void Classify() {
        // Note - speed value is multiplicitive (Lower is better)
        speed = 1f;
        atk = 0;
        def = 0;
        // -- Starter Weapons -- //
        if (name == "Twig") {
            mindmg = 1;
            maxdmg = 4;
            description = "It's a twig. This is a terrible idea.";
        } else if (name == "Sharp Twig") { // Dagger type
            mindmg = 1;
            maxdmg = 2;
            speed = 0.8f;
            atk = 3;
            def = -2;
            description = "A particularly spiky twig. Fast and quiet, but you'll have to get close.";
        } else if (name == "Plank with a Nail") { // Axe type
            mindmg = 1;
            maxdmg = 5;
            atk = 2;
            description = "This nail should punch through armor at least.";
        } else if (name == "Club") { // Mace Type
            mindmg = 2;
            maxdmg = 6;
            speed = 1.25f;
            description = "It's slow, but it hits hard.";
        } else if (name == "Long Stick") { // Spear type
            mindmg = 1;
            maxdmg = 4;
            def = 3;
            description = "Now this should keep them back.";
        } else if (name == "Log") { // Polearm type
            mindmg = 2;
            maxdmg = 6;
            atk = 3;
            def = 3;
            speed = 2f;
            description = "What am dex? Me have big bonk! Pathetic weaklings stay back.";
        }
    }

    public override string ToString()
    {
        return description;
    }
}