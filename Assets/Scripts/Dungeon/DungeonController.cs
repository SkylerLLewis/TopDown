using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonController : MonoBehaviour
{
    public Dictionary<string,Vector3Int> notableCells;
    public System.Action<Vector3Int,int> OpenDoorRef;
    public System.Action<Vector3Int> OpenChestRef;
    public System.Action<string> NotableActionsRef;
    private PersistentData data;

    void Awake()
    {
        OpenDoorRef = gameObject.GetComponent<Initializer>().OpenDoor;
        OpenChestRef = gameObject.GetComponent<Initializer>().OpenChest;
        NotableActionsRef = gameObject.GetComponent<Initializer>().NotableActions;
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        data.root = GameObject.FindWithTag("Root");
    }

    public void NotableCollide(Vector3Int cell) {
        foreach (KeyValuePair<string,Vector3Int> n in notableCells) {
            if (cell == n.Value) {
                NotableActionsRef(n.Key);
                break;
            }
        }
    }

    public void UpdateNotables(Dictionary<string,Vector3Int> notable) {
        notableCells = notable;
    }

    public void OpenDoor(Vector3Int cell, int dir) {
        OpenDoorRef(cell, dir);
    }

    public void OpenChest(Vector3Int cell) {
        OpenChestRef(cell);
    }
}
