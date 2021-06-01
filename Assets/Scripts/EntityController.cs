using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    private List<EnemyBehavior> entities;
    private List<EnemyBehavior> next;
    public bool enemyTurn;
    private float slowest;
    private int waiting;
    void Start() {
        entities = new List<EnemyBehavior>();
        next = new List<EnemyBehavior>();
        enemyTurn = false;
    }

    void turnStart(float time) {
        entities.Clear();
        next.Clear();
        enemyTurn = true;
        slowest = 0f;

        // Give them time to take turns (<0 means your turn!)
        foreach (Transform child in transform) {
            EnemyBehavior e = child.GetComponent<EnemyBehavior>();
            e.timer -= time;
            // Only add living/fast enough
            if (e.timer < 0 && !e.dying) {
                entities.Add(e);
                if (e.moveSpeed > slowest) {
                    slowest = e.moveSpeed;
                }
            }
        }
        // Sort enemies by time
        entities.Sort((x, y) => {
            var ret = x.timer.CompareTo(y.timer);
            return ret;
        });
        // Take actions
        if (entities.Count > 0) {
            //slowest += 0.5f;
            //Invoke("TakeTurns", slowest);
            TakeTurns();
        } else {
            enemyTurn = false;
        }
        /*for (int i=0; i<entities.Count; i++) {
            EnemyBehavior e = entities[i];
            e.timer += e.moveSpeed;
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
        }*/
    }

    public void TakeTurns() {
        string s = "Enemies taking turns:";
        foreach (EnemyBehavior e in entities) {
            if (e.timer < 0) {
                s += "\n    "+e.gameObject.name;
                waiting++;
                e.MyTurn();
            }
        }
        Debug.Log(s);
        if (waiting == 0) {
            enemyTurn = false;
        }
    }

    public void Report() {
        waiting--;
        if (waiting == 0) {
            TakeTurns();
        }
    }

    /*public void doubleTurn() {
        if (next.Count != 0) {
            for (int i=next.Count-1; i>=0; i--) {
                EnemyBehavior e = next[i];
                    e.timer += e.moveSpeed;
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
    }*/
}
