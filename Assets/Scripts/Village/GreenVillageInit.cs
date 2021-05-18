using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GreenVillageInit : MonoBehaviour
{
    private PersistentData data;
    private VillageController villageController;
    private Dictionary<string,Vector3Int> notableCells;
    private PlayerController player; 
    void Awake()
    {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        villageController = gameObject.GetComponent<VillageController>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        notableCells = new Dictionary<string, Vector3Int>();
        notableCells.Add("stairsDown", new Vector3Int(0,-9,0));
        notableCells.Add("dagger", new Vector3Int(1,0,0));
        notableCells.Add("axe", new Vector3Int(1,-1,0));
        notableCells.Add("mace", new Vector3Int(1,-2,0));
        notableCells.Add("spear", new Vector3Int(1,-3,0));
        notableCells.Add("polearm", new Vector3Int(1,-4,0));

        villageController.UpdateNotables(notableCells);
    }

    public Vector3Int GetEntrance() {
        return new Vector3Int(0,0,0);
    }

    public void NotableActions(string key) {
        if (key == "stairsDown") {
            data.depth++;
            data.direction = "down";
            SceneManager.LoadScene("BasicDungeon");
        } else if (key == "dagger" || key == "axe" || key == "mace" || key == "spear" || key == "polearm") {
            data.weapon = key;
            player.EquipWeapon(key);
        }
    }
}
