using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SkillsScreenController : MonoBehaviour
{
    private PersistentData data;
    private GameObject root, eventSystem;
    private PlayerController player;
    private UIController uiController;
    void Awake() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get inactive Root object stored in persistent data
        root = data.root;
        root.SetActive(false);
        player = root.GetComponentInChildren<PlayerController>();
        uiController = root.GetComponentInChildren<UIController>();

        eventSystem = GameObject.Find("EventSystem");
    }

    public void BackToScene() {
        eventSystem.SetActive(false);
        SceneManager.UnloadSceneAsync("SkillsScreen");
        root.SetActive(true);
    }
}
