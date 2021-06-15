using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonController : MonoBehaviour
{
    public Dictionary<string,Vector3Int> notableCells;
    public System.Action<Vector3Int,int> OpenDoorRef;
    public System.Action<Vector3Int> OpenChestRef;
    public System.Action<List<Vector3Int>> HighlightTilesRef;
    public System.Action<string> NotableActionsRef;
    public System.Func<List<Room>> GetRoomsRef;
    private PersistentData data;

    void Awake()
    {
        OpenDoorRef = gameObject.GetComponent<Initializer>().OpenDoor;
        OpenChestRef = gameObject.GetComponent<Initializer>().OpenChest;
        NotableActionsRef = gameObject.GetComponent<Initializer>().NotableActions;
        GetRoomsRef = gameObject.GetComponent<Initializer>().GetRooms;
        HighlightTilesRef = gameObject.GetComponent<Initializer>().HighlightTiles;
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        data.root = GameObject.FindWithTag("Root");
        data.mapType = "Dungeon";
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

    public List<Room> GetRooms() {
        return GetRoomsRef();
    }

    public void HighlightTiles(List<Vector3Int> tiles) {
        HighlightTilesRef(tiles);
    }
}
