using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    Vector3 startPosition, targetPosition;
    float count;
    public float speed;
    PlayerController target;
    int damage;
    bool shot = false;
    public void Shoot(PlayerController p, int dmg) {
        target = p;
        damage = dmg;
        startPosition = transform.position;
        startPosition.y += 0.5f;
        targetPosition = p.transform.position;
        targetPosition.y += 0.5f;
        count = 0;
        speed = 10/Vector3.Distance(startPosition, targetPosition);
        shot = true;
    }

    void Update() {
        if (shot) {
            if (count < 1.0f) {
                count += 1.0f * speed * Time.deltaTime;
                this.transform.position = Vector3.Lerp(startPosition, targetPosition, count);
            } else {
                target.Damage(damage);
                Destroy(this.gameObject);
            }
        }
    }
}
