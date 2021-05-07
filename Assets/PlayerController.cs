using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed = 1;
    public bool moving = false;
    public bool attacking = false;
    public Vector3Int tilePosition;
    private Tilemap dungeonMap;
    private Tilemap wallMap;
    private GameObject entities;
    private EntityController entityController;
    private GameObject mainCamera;
    private GameObject[] enemyList;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Vector3 highPoint;
    private float count = 1.0f;

    void Start() {
        targetPosition = this.transform.position;
        Grid grid = FindObjectOfType<Grid>();
        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
                break;
            }
        }
        tilePosition = dungeonMap.WorldToCell(this.transform.position);
        entities = GameObject.FindWithTag("EntityList");
        entityController = entities.GetComponent<EntityController>();
        mainCamera = GameObject.FindWithTag("MainCamera");
    }


    public Vector3 relativePos;
    public Vector3 pos;
    // Update is called once per frame
    void Update() {
        /*/ Start new code
        pos = new Vector3 (transform.position.x, transform.position.y, 0);
        Vector3Int tilepos = dungeonMap.WorldToCell(pos);
        Vector3 tileworldpos = dungeonMap.CellToWorld(tilepos);
        relativePos = pos - tileworldpos;
        if (relativePos.y < .25)
            transform.position = new Vector3(transform.position.x, transform.position.y, 1);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, 2);
        */// End new code
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Click!");
            if (moving) {
                Debug.Log("Can't, I'm moving!");
            }
            if (entityController.enemyTurn) {
                Debug.Log("Can't, enemy's turn!");
            }
        }
        if (Input.GetMouseButtonDown(0) && !moving && !entityController.enemyTurn) {

            // Determine screen position
            Vector2 pos = new Vector2(
                Input.mousePosition.x/Screen.width,
                Input.mousePosition.y/Screen.height);
            
            // Wait?
            //Vector3Int click = dungeonMap.WorldToCell(Input.mousePosition);
            if (Mathf.Abs(pos.x-0.5f)<0.025f && Mathf.Abs(pos.y-0.5f)<0.05f) {
                endTurn();
                return;
            }

            // Calculate target pos
            targetPosition = this.transform.position;
            startPosition = this.transform.position;
            if (pos.x < 0.5 && pos.y >= 0.5) { // y++
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
            } else if (pos.x >= 0.5 && pos.y >= 0.5) { // x++
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
            } else if (pos.x >= 0.5 && pos.y < 0.5) { // y--
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
            } else if (pos.x < 0.5 && pos.y < 0.5) { // x--
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
            } else {
                Debug.Log("AAAAAAAAAHHHHHHH THIS IS IMPOSSIBLE");
            }

            // Find target tile
            Vector3Int targetCell = dungeonMap.WorldToCell(targetPosition);
            targetCell.z = 0;
            TileBase targetTile =  dungeonMap.GetTile(targetCell);

            // Attack enemy in front?
            bool enemyFront = false;
            enemyList = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemyList) {
                EnemyBehavior es = e.GetComponent<EnemyBehavior>();
                if (es.tilePosition == targetCell) {
                    enemyFront = true;
                    break;
                }
            }

            if (enemyFront) {
                Debug.Log("//---PLAYER ATTACK---//");moving = true;
                highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                highPoint = targetPosition;
                targetPosition = startPosition;
                count = 0.0f;
            // Point is valid?
            } else if (targetTile != null && targetTile.name == "floor") {
                // Init bezier curve
                moving = true;
                tilePosition = dungeonMap.WorldToCell(targetPosition);
                highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                count = 0.0f;
            }
        }

        // Continue Bezier curve
        if (moving) {
            if (count < 1.0f) {
                count += 1.0f * 5 * Time.deltaTime;
                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
                Vector3 camVec = Vector3.Lerp(startPosition, targetPosition, count);
                camVec.z = -10;
                mainCamera.transform.position = camVec;
            } else {
                // Turn over, activate entities
                moving = false;
                endTurn();
            }
        } else if (attacking) {
            if (count < 1.0f) {
                count += 1.0f * 5 * Time.deltaTime;
                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
            } else {
                // Turn over, activate entities
                attacking = false;
                endTurn();
            }
        }
    }

    void endTurn() {
        entities.BroadcastMessage("turnStart", speed);
    }
}
