using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DmgTextController : MonoBehaviour
{
    private float count, speed;
    private float yvar, xvar;
    Vector3 starPosition, targetPosition;
    TextMeshProUGUI textMesh;

    public void Init(Vector3 pos, string style, string msg="") {
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        if (style != "gold" && style != "cost") {
            pos.y += 0.75f;
        }
        if (style == "skillCost") {
            pos.y -= 0.75f;
            pos.x -= 0.25f;
        }
        this.transform.position = pos;
        starPosition = this.transform.position;
        count = 1.0f;
        speed = 2;
        // Lower yvar is higher
        yvar = Random.Range(5f, 6f);
        xvar = Random.Range(-0.001f,0.001f);

        if (style == "dmg") {
            textMesh.text = msg;
        } else if (style == "crit") {
            textMesh.text = "<b>"+msg+"</b>";
            textMesh.color = new Color32(150,0,0,255);
            textMesh.fontSize = 24;
            yvar *= 0.95f;
            speed *= 0.75f;
        } else if (style == "miss") {
            textMesh.color = new Color32(255,255,0,255);
            textMesh.text = "miss";
            yvar *= -1.5f;
            Vector3 p = this.transform.position;
            p.y += 0.25f;
            this.transform.position = p;
        } else if (style == "heal") {
            textMesh.text = msg;
            textMesh.color = new Color32(0,255,0,255);
            yvar += 1;
        } else if (style == "mana") {
            textMesh.text = msg;
            textMesh.color = new Color32(0,150,255,255);
            yvar += 1;
        } else if (style == "wait") {
            textMesh.color = new Color32(255,255,255,255);
            if (msg == "") {
                textMesh.text = "wait";
            } else {
                textMesh.text = msg;
            }
            xvar /= 2;
            yvar *= 0.9f;
        } else if (style == "msg") {
            textMesh.color = new Color32(200,200,200,255);
            textMesh.text = msg;
            speed /= 2;
            xvar = 0;
            yvar *= 1.1f;
        } else if (style == "cost") {
            textMesh.color = new Color32(255,150,0,255);
            textMesh.text = msg;
            textMesh.fontSize = 48;
            speed /= 2;
            xvar *= 0.5f;
            yvar *= -1.2f;
        } else if (style == "gold") {
            textMesh.color = new Color32(100,255,0,255);
            textMesh.text = msg;
            textMesh.fontSize = 48;
            speed /= 2;
            xvar *= 0.5f;
            yvar *= 1.3f; 
        } else if (style == "skillCost") {
            textMesh.color = new Color32(0,0,0,255);
            textMesh.text = msg;
            textMesh.fontSize = 48;
            speed /= 2;
            xvar *= 0.5f;
            yvar *= -1.2f;
        } else {
            
            textMesh.text = "AHHH";
            textMesh.color = new Color32(255,0,0,255);
        }
    }

    void Update()
    {
        if (count > 0) {
            targetPosition = this.transform.position;
            targetPosition.y += Mathf.Pow(count/yvar, 3);
            targetPosition.x += xvar;
            this.transform.position = targetPosition;
            count -= 1.0f * speed * Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }
}
