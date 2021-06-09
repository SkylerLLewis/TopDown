using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    private PersistentData data;
    private PlayerController player;
    private GameObject root, itemArray, eventSystem;
    TextMeshProUGUI title, description, button, stats, gold;
    void Start()
    {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        // Get inactive Root object stored in persistent data
        root = data.root;
        root.SetActive(false);
        player = root.GetComponentInChildren<PlayerController>();
        
    }
}
