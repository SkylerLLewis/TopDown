using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

    public static Dictionary<string, List<string>> SkillTypes = 
    new Dictionary<string,List<string>>() {
        {"magic", new List<string>() {"Magic Missile", "Lesser Heal"}}
    };
    public string name, abilityType, activationType;
    public int manaCost;
    public Skill(string n) {
        name = n;
        Classify();
    }

    private void Classify() {
        if (SkillTypes["magic"].Contains(name)) {
            abilityType = "magic";
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