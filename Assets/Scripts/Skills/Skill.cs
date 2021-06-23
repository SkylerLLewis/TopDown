using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

    public static Dictionary<string, List<string>> SkillTypes = 
    new Dictionary<string,List<string>>() {
        {"basic", new List<string>() {"Health", "Mana", "Speed"}},
        {"magic", new List<string>() {"Magic Missile", "Lesser Heal"}}
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
                manaCost = 4;
                activationType = "ranged";
            } else if (name == "Lesser Heal") {
                manaCost = 10;
                activationType = "instant";
            }
        }
    }
}