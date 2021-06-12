using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageController : MonoBehaviour
{
    public Dictionary<string,Vector3Int> notableCells;
    public System.Action<string> NotableActionsRef;
    public System.Action<Vector3Int, int> OpenDoorRef;
    public System.Func<Vector3Int> FetchPositionRef;
    PersistentData data;

    void Awake() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        data.root = GameObject.FindWithTag("Root");
        NotableActionsRef = gameObject.GetComponent<GreenVillageInit>().NotableActions;
        OpenDoorRef = gameObject.GetComponent<GreenVillageInit>().OpenDoor;
        FetchPositionRef = gameObject.GetComponent<GreenVillageInit>().FetchPosition;
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
        OpenDoorRef(cell, dir);
    }

    public Vector3Int FetchPosition() {
        return FetchPositionRef();
    }
}
