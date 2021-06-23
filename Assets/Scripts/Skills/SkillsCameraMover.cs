using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsCameraMover : MonoBehaviour
{
    Vector3 target, start;
    bool moving = false;
    float time;
    void Start() {
        
    }

    void Update() {
        if (moving) {
            if (time < 1) {
                time += 1f * Time.deltaTime;
                float t = time * time * (3f - 2f * time);
                transform.position = Vector3.Lerp(start, target, t);
            } else {
                moving = false;
            }
        }
    }

    public void MoveHere(Vector3 t) {
        time = 0;
        start = transform.position;
        target = t;
        target.y -= 1;
        target.z = -10;
        moving = true;
    }
}
