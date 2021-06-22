using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockController : MonoBehaviour
{
    SkillsScreenController skillsController;
    // Start is called before the first frame update
    void Start() {
        skillsController = transform.parent.parent.GetComponent<SkillsScreenController>();
        
    }

    void OnMouseUp() {
        Debug.Log("CLEEKED!");
    }
}
