using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillController : MonoBehaviour
{
    UIController uiController;
    public bool active, disabled;
    public string skillName;
    Image image;
    Dictionary<string, Sprite> sprites;
    public Skill skill;

    void Awake(){
        uiController = transform.parent.GetComponent<UIController>();
        image = GetComponent<Image>();
        active = false;
        disabled = false;
    }

    void OnMouseUp() {
        if (disabled) return;
        if (!active) {
            uiController.AbilityActivate(skill);
            if (skill.activationType != "instant") {
                image.sprite = sprites["on"];
                active = true;
            }
        } else {
            image.sprite = sprites["off"];
            active = false;
            uiController.CancelAbility(skillName);
        }
    }

    public void SetName(string n) {
        skillName = n;
        sprites = new Dictionary<string, Sprite>();
        sprites.Add("off", Resources.Load<Sprite>("Buttons/Skills/"+n));
        sprites.Add("on", Resources.Load<Sprite>("Buttons/Skills/"+n+" Selected"));
        sprites.Add("disabled", Resources.Load<Sprite>("Buttons/Skills/"+n+" Disabled"));
        GetComponent<Image>().sprite = sprites["off"];
        skill = new Skill(n);
    }

    public void Disable() {
        image.sprite = sprites["disabled"];
        disabled = true;
    }

    public void Undisable() {
        image.sprite = sprites["off"];
        disabled = false;
    }

    public void Used() {
        image.sprite = sprites["off"];
        active = false;
    }
}
