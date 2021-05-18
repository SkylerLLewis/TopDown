using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    RectTransform hpBar;
    TextMeshProUGUI depthText;
    private PersistentData data;

    void Awake()
    {
        hpBar = GameObject.Find("HP Bar").GetComponent<RectTransform>();
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        depthText = GameObject.Find("Depth Text").GetComponent<TextMeshProUGUI>();
        depthText.text = data.depth.ToString();
    }

    public void UpdateHP(int hp, int maxhp) {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400f*((hp*1.0f)/maxhp));
    }

}
