using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryController : MonoBehaviour
{
    void Start() {
        
    }

    public void BackToScene() {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GreenVillage"));
    }

}
