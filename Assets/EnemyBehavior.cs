using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBehavior : MonoBehaviour
{
    public float speed = 0.25f;
    public float timer;
    public bool moving = false;
    public bool attacking = false;
    private bool last = false;
    public Vector3Int tilePosition;
    private Tilemap dungeonMap;
    private GameObject plObj;
    private PlayerController player;
    private EntityController entityController;
    private GameObject[] entities;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Vector3 highPoint;
    private float count = 1.0f;
    void Start() {
        speed = 10f;
        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
                break;
            }
        }
        tilePosition = dungeonMap.WorldToCell(this.transform.position);
        plObj = GameObject.FindWithTag("Player");
        player = plObj.GetComponent<PlayerController>();
        entityController = GameObject.FindObjectOfType<EntityController>();
        timer = Random.Range(0, speed);
        Debug.Log("Enemy "+name+" with speed: "+speed+" and timer: "+timer);
    }

    public void myTurn(bool last=false) {
        this.last = last;
        Debug.Log(name+" taking turn!");
        targetPosition = this.transform.position;
        startPosition = this.transform.position;
        // A numerical direction that can rotate 0-3
        int direction = 0;

        // What direction is best?
        if (player.tilePosition.y > tilePosition.y) { // y++
            direction = 0;
        } else if (player.tilePosition.x > tilePosition.x) { // x++
            direction = 1;
        } else if (player.tilePosition.y < tilePosition.y) { // y--
            direction = 2;
        } else if (player.tilePosition.x < tilePosition.x) { // x--
            direction = 3;
        }
        
        for (int i=0; i<3; i++) { // Broken when action executed
            if (direction == 0) {
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
            } else if (direction == 1) {
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
            } else if (direction == 2) {
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
            } else if (direction == 3) {
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
            }

            // Find target tile
            Vector3Int targetCell = dungeonMap.WorldToCell(targetPosition);
            TileBase targetTile =  dungeonMap.GetTile(targetCell);
            
            // Tile clear of friends?
            bool friend = false;
            entities = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in entities) {
                EnemyBehavior es = e.GetComponent<EnemyBehavior>();
                if (es.tilePosition == targetCell) {
                    friend = true;
                    break;
                }
            }

            if (targetCell == player.tilePosition) { // ATTACK!
                Debug.Log("//---Attack time!---//");// Init bezier curve
                attacking = true;
                targetPosition = startPosition;
                highPoint = player.transform.position;
                count = 0.0f;
                if (last) {
                    //Debug.Log(name+"Designated reporter going to report from attack.");
                }
                break;
            } else if (targetTile != null && targetTile.name == "floor" && !friend) { // Move
                // Init bezier curve
                moving = true;
                highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                count = 0.0f;
                tilePosition = dungeonMap.WorldToCell(targetPosition);
                if (last) {
                    //Debug.Log(name+"Designated reporter going to report from move.");
                }
                break;
            } else { // Try again
                if (i == 0) { // First, turn left or right
                    if (Random.Range(-1,1) < 0 ) {
                        direction = (direction-1) % 4;
                    } else {
                        direction = (direction+1) % 4;
                    }
                } else if (i == 1) { // Then, try the other way
                    direction = (direction+2) % 4;
                } else if (i == 2 && last) {
                    Debug.Log(name+" Designated reporter reporting from uselessness.");
                    entityController.doubleTurn();
                }
                continue;
            }
        }
    }

    void Update() {
        if (moving) {
            if (count < 1.0f) {
                count += 1.0f * 5 * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
            } else {
                moving = false;
                if (last) {
                    //Debug.Log(name+" Designated reporter reporting from move.");
                }
                if (last) { entityController.doubleTurn(); }
            }
        } else if (attacking) {
            if (count < 1.0f) {
                count += 1.0f * 5 * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
            } else {
                attacking = false;
                if (last) {
                    //Debug.Log(name+" Designated reporter reporting from attack.");
                }
                if (last) { entityController.doubleTurn(); }
            }
        }
    }
}
