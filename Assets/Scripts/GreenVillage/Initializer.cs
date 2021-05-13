using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageInitilaizer : MonoBehaviour
{
    public Dictionary<string,Vector3Int> notableCells;

    void Awake()
    {
        notableCells = new Dictionary<string, Vector3Int>();
        notableCells.Add("stairsDown", new Vector3Int(0,-9,0));
    }

    public void notableCollide(Vector3Int cell) {
        foreach (KeyValuePair<string,Vector3Int> n in notableCells) {
            if (cell == n.Value) {
                notableActions(n.Key);
            }
        }
    }
    private void notableActions(string key) {
        if (key == "stairsDown") {
            SceneManager.LoadScene("BasicDungeon");
        }
    }

}
