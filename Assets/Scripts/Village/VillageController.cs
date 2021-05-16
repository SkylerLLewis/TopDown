using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageController : MonoBehaviour
{
    public Dictionary<string,Vector3Int> notableCells;
    public System.Action<string> NotableActionsRef;

    void Awake()
    {
        notableCells = new Dictionary<string, Vector3Int>();
        notableCells.Add("stairsDown", new Vector3Int(0,-9,0));
        NotableActionsRef = gameObject.GetComponent<GreenVillageInit>().NotableActions;
    }

    public void NotableCollide(Vector3Int cell) {
        foreach (KeyValuePair<string,Vector3Int> n in notableCells) {
            if (cell == n.Value) {
                NotableActionsRef(n.Key);
            }
        }
    }

    public void OpenDoor(Vector3Int cell, int dir) {
        
    }
}
