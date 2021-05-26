using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    RectTransform hpBar;
    TextMeshProUGUI depthText;
    private PersistentData data;
    private PlayerController player;
    void Awake()
    {
        hpBar = GameObject.Find("HP Bar").GetComponent<RectTransform>();
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        depthText = GameObject.Find("Depth Text").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Debug.Log("PLayer: "+player.ToString());
        depthText.text = data.depth.ToString();
    }

    public void UpdateHP(int hp, int maxhp) {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 190f*((hp*1.0f)/maxhp));
    }

    public void OpenInventory() {
        Scene scene = SceneManager.GetSceneByName("Inventory");
        Debug.Log("Inventory is: "+scene.ToString());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Inventory"));
    }

    public void AbilityActivate(string name) {
        Debug.Log("This: "+this.GetComponent<UIController>().ToString());
        this.GetComponent<UIController>().player.Ability(name);
    }

}
