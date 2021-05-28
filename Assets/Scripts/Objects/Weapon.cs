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
            description = "It's a twig. This is a terrible idea.";
            mindmg = 1;
            maxdmg = 4;
        } else if (name == "Sharp Twig") { // Dagger type
            description = "A particularly spiky twig. Fast and quiet, but you'll have to get close.";
            mindmg = 1;
            maxdmg = 2;
            speed = 0.8f;
            atk = 3;
            def = -2;
        } else if (name == "Plank with a Nail") { // Axe type
            description = "This nail should punch through armor at least.";
            mindmg = 1;
            maxdmg = 5;
            atk = 2;
        } else if (name == "Club") { // Mace Type
            description = "It's slow, but it hits hard.";
            mindmg = 2;
            maxdmg = 6;
            speed = 1.25f;
        } else if (name == "Long Stick") { // Spear type
            description = "Now this should keep them back.";
            mindmg = 1;
            maxdmg = 4;
            def = 3;
        } else if (name == "Log") { // Polearm type
            description = "What am dex? Me have big bonk! Pathetic weaklings stay back.";
            mindmg = 2;
            maxdmg = 6;
            atk = 3;
            def = 3;
            speed = 2f;
        } else if (name == "Rusty Shortsword") {
            description = "Finally, a real weapon! The rust is so thick you can barely see the iron. Probably should avoid hitting anything too hard...";
            mindmg = 2;
            maxdmg = 6;
            speed = 0.8f;
        } else if (name == "Half a Scissor") {
            description = "You know what, it'll work. It's even got a convenient thumb loop!";
            mindmg = 2;
            maxdmg = 3;
            speed = 0.8f;
            atk = 6;
            def = -2;
        } else if (name == "Copper Hatchet") {
            description = "This hatchet should bite deep. It nice shine to it. At least it's pretty.";
            mindmg = 2;
            maxdmg = 8;
            atk = 4;
        } else if (name == "Mallet") {
            description = "I'm gettin me mallet!";
            mindmg = 4;
            maxdmg = 9;
            speed = 1.25f;
        } else if (name == "Flint Spear") {
            description = "It smells faintly of mammoth blood.";
            mindmg = 2;
            maxdmg = 6;
            def = 6;
        } else if (name == "Grain Scythe") {
            description = "This thing gets stuck on everything! Who thought this was a good weapon!?";
            mindmg = 4;
            maxdmg = 9;
            speed = 2f;
            def = 6;
            atk = 6;
        }
    }

    public override string ToString()
    {
        return description;
    }
}