using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgTextController : MonoBehaviour
{
    private float count;
    Vector3 targetPosition;
    public void Init(Vector3 pos) {
        this.transform.position = new Vector3(pos.x, pos.y+0.75f, pos.z);
        count = 1.0f;
    }

    void Update()
    {
        if (count > 0) {
            targetPosition = this.transform.position;
            targetPosition.y += Mathf.Pow(count/5, 2);
            this.transform.position = targetPosition;
            count -= 0.02f;//1.0f * 5 * Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }
}
