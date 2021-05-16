using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GreenVillageInit : MonoBehaviour
{
    private PersistentData data;
    void Start()
    {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
    }

    public Vector3Int GetEntrance() {
        return new Vector3Int(0,0,0);
    }

    public void NotableActions(string key) {
        if (key == "stairsDown") {
            data.depth++;
            data.direction = "down";
            SceneManager.LoadScene("BasicDungeon");
        }
    }
}
