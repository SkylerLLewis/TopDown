using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

    public static Dictionary<string, List<string>> SkillTypes = 
    new Dictionary<string,List<string>>() {
        {"basic", new List<string>() {"Health", "Mana", "Speed"}},
        {"magic", new List<string>() {"Magic Missile", "Lesser Heal"}},
        {"melee", new List<string>() {"Lunge", "Power Attack"}}
    };
    public string name, abilityType, activationType, stat,
                  displayName, description;
    public int magnitude = 1, amount, manaCost;
    public Skill(string n) {
        name = n;
        Classify();
    }

    private void Classify() {
        if (SkillTypes["basic"].Contains(name)) {
            abilityType = "improvement";
            stat = name;
            if (name == "Health") {
                displayName = "Increase Health";
                description = "Instantly double the amount of blood in your body! (very safe and reasonable)";
                amount = 5;
            } else if (name == "Mana") {
                displayName = "Increase Max mana";
                description = "Enwrinkle your brain!";
                amount = 4;
            } else if (name == "Speed") {
                displayName = "Increase Speed";
                description = "Hasten your tippy tappies.";
                amount = 1;
            }
        } else if (SkillTypes["magic"].Contains(name)) {
            abilityType = "magic";
            displayName = name;
            if (name == "Magic Missile") {
                description = "Fire a magic missile at your enemies from afar that has a higher chance to both hit and crit. This skill takes two turns to cast.";
                manaCost = 4;
                activationType = "ranged";
            } else if (name == "Lesser Heal") {
                description = "Slow bleeding and heal bruises. (+15hp)";
                manaCost = 10;
                activationType = "instant";
            }
        } else if (SkillTypes["melee"].Contains(name)) {
            abilityType = "melee";
            displayName = name;
            if (name == "Lunge") {
                description = "Lunge forward suddenly to strike an enemy before they can close with you, staying just out of their reach.";
                activationType = "ranged";
            } else if (name == "Power Attack") {
                description = "Bring your weapon down for a crushing blow.";
                activationType = "ranged";
            }
        }
    }
}