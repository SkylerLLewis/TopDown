using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    private List<EnemyBehavior> entities;
    private List<EnemyBehavior> next;
    public bool enemyTurn;
    void Start() {
        entities = new List<EnemyBehavior>();
        next = new List<EnemyBehavior>();
        enemyTurn = false;
    }

    void turnStart(float time) {
        entities.Clear();
        next.Clear();
        enemyTurn = true;

        // Sort enemies by timer, take turns
        foreach (Transform child in transform) {
            EnemyBehavior e = child.GetComponent<EnemyBehavior>();
            e.timer -= time;
            entities.Add(e);
        }
        entities.Sort((x, y) => {
            var ret = x.timer.CompareTo(y.timer);
            return ret;
        });
        // Remove dead/slow ones
        for (int i=entities.Count-1; i>=0; i--) {
            if (entities[i].timer >= 0) {
                entities.RemoveAt(i);
            } else if (entities[i].dying) {
                entities.RemoveAt(i);
            }
        }
        // Take actions
        for (int i=0; i<entities.Count; i++) {
            EnemyBehavior e = entities[i];
            e.timer += e.speed;
            if (e.timer < 0) {
                next.Add(e);
            }
            if (i == entities.Count-1) {
                e.MyTurn(true);
            } else {
                e.MyTurn();
            }
        }
        if (entities.Count == 0) {
            enemyTurn = false;
        }
    }

    public void doubleTurn() {
        if (next.Count != 0) {
            for (int i=next.Count-1; i>=0; i--) {
                EnemyBehavior e = next[i];
                    e.timer += e.speed;
                if (next[i].timer >= 0) {
                    next.RemoveAt(i);
                }
                if (i == 0) {
                    e.BroadcastMessage("MyTurn", true);
                } else {
                    e.BroadcastMessage("MyTurn", true);
                }
            }
        } else {
            enemyTurn = false;
        }
    }
}
