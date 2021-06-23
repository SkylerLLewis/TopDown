using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockController : MonoBehaviour
{
    SkillsScreenController skillsController;
    SkillsCameraMover _camera;
    void Start() {
        skillsController = GameObject.Find("Canvas").GetComponent<SkillsScreenController>();
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<SkillsCameraMover>();
    }

    void OnMouseUp() {
        skillsController.DisplayUnlock(this.name);
        _camera.MoveHere(transform.position);
    }
}
