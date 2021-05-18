using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgTextController : MonoBehaviour
{
    private float count;
    private float yvar, xvar;
    Vector3 targetPosition;
    public void Init(Vector3 pos) {
        this.transform.position = new Vector3(pos.x, pos.y+0.75f, pos.z);
        count = 1.0f;
        yvar = Random.Range(4.5f, 5.5f);
        xvar = Random.Range(-0.01f,0.01f);
    }

    void Update()
    {
        if (count > 0) {
            targetPosition = this.transform.position;
            targetPosition.y += Mathf.Pow(count/yvar, 2);
            targetPosition.x += xvar;
            this.transform.position = targetPosition;
            count -= 0.02f;//1.0f * 5 * Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }
}
