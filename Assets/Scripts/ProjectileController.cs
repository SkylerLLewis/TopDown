using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    Vector3 startPosition, targetPosition;
    float count;
    public float speed;
    string style;
    EnemyBehavior target;
    int damage;
    bool shot = false;
    public void Shoot(EnemyBehavior p, int dmg, string s) {
        target = p;
        damage = dmg;
        style = s;
        startPosition = transform.position;
        startPosition.y += 0.75f;
        targetPosition = PathFinder.TileToWorld(p.tilePosition);
        targetPosition.y += 0.75f;
        count = 0;
        speed = 15/Vector3.Distance(startPosition, targetPosition);
        // Set rotation of Projectile
        var angles = transform.rotation.eulerAngles;
        angles.z = Mathf.Rad2Deg * Mathf.Atan(/*y*/ (targetPosition.y - startPosition.y) / /*x*/(targetPosition.x - startPosition.x));
        if (targetPosition.x < startPosition.x) {
            angles.z += 180;
        }
        transform.rotation = Quaternion.Euler(angles);
        shot = true;
    }

    void Update() {
        if (shot) {
            if (count < 1.0f) {
                count += 1.0f * speed * Time.deltaTime;
                this.transform.position = Vector3.Lerp(startPosition, targetPosition, count);
            } else {
                target.Damage(damage, style);
                Destroy(this.gameObject);
            }
        }
    }
}