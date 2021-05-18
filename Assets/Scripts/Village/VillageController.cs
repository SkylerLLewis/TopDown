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
        NotableActionsRef = gameObject.GetComponent<GreenVillageInit>().NotableActions;
    }

    public void NotableCollide(Vector3Int cell) {
        foreach (KeyValuePair<string,Vector3Int> n in notableCells) {
            if (cell == n.Value) {
                NotableActionsRef(n.Key);
            }
        }
    }

    public void UpdateNotables(Dictionary<string,Vector3Int> n) {
        notableCells = n;
    }

    public void OpenDoor(Vector3Int cell, int dir) {
        
    }
}
