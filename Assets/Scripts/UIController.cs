using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    RectTransform hpBar;
    void Start()
    {
        hpBar = GameObject.Find("HP Bar").GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }

    public void UpdateHP(int hp, int maxhp) {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400f*((hp*1.0f)/maxhp));
    }
}
