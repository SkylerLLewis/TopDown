using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public int depth;
    public int entrance;
    public string direction;
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        depth = 0;
        entrance = 0;
        direction = "down";
        SceneManager.LoadScene("GreenVillage");
    }
}
