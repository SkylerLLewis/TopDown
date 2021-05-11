using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 1;
    public bool moving = false;
    public bool attacking = false;
    public bool dying = false;
    public Vector3Int tilePosition;
    public string[] facing;
    private Tilemap dungeonMap, leftWallMap, rightWallMap;
    private Dictionary<string,Tilemap> maps;
    private Initializer mapController;
    private GameObject entities;
    private EntityController entityController;
    private GameObject mainCamera;
    private GameObject canvas;
    private GameObject[] enemyList;
    private Vector3 targetPosition, startPosition, highPoint;
    private Quaternion startAngle, targetAngle;
    private float count = 1.0f;
    
    // Combat Stats
    public int maxhp, hp, attack, defense, mindmg, maxdmg;

    void Start() {
        targetPosition = this.transform.position;
        Grid grid = FindObjectOfType<Grid>();
        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
            } else if (map.name == "LeftWallMap") {
                leftWallMap = map;
            } else if (map.name == "RightWallMap") {
                rightWallMap = map;
            }
        }
        maps = new Dictionary<string, Tilemap>();
        maps.Add("left", leftWallMap);
        maps.Add("right", rightWallMap);
        mapController = dungeonMap.GetComponent<Initializer>();
        canvas = GameObject.FindWithTag("WorldCanvas");
        tilePosition = dungeonMap.WorldToCell(this.transform.position);
        entities = GameObject.FindWithTag("EntityList");
        entityController = entities.GetComponent<EntityController>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        facing = new string[2];
        maxhp = 20;
        hp = maxhp;
        attack = 5;
        defense = 5;
        mindmg = 1;
        maxdmg = 3;
    }


    public Vector3 relativePos;
    public Vector3 pos;
    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (moving) {
                Debug.Log("Can't, I'm moving!");
            }
            if (entityController.enemyTurn) {
                Debug.Log("Can't, enemy's turn!");
            }
        }
        if (Input.GetMouseButtonDown(0) && !moving && !entityController.enemyTurn && !dying) {

            // Determine screen position
            Vector2 pos = new Vector2(
                Input.mousePosition.x/Screen.width,
                Input.mousePosition.y/Screen.height);
            
            // Wait?
            //Vector3Int click = dungeonMap.WorldToCell(Input.mousePosition);
            if (Mathf.Abs(pos.x-0.5f)<0.025f && Mathf.Abs(pos.y-0.5f)<0.05f) {
                EndTurn();
                return;
            }

            // A numerical direction that can rotate 0-3
            int direction = 0;

            // Calculate target pos
            targetPosition = this.transform.position;
            startPosition = this.transform.position;
            if (pos.x < 0.5 && pos.y >= 0.5) { // y++
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
                direction = 0;
            } else if (pos.x >= 0.5 && pos.y >= 0.5) { // x++
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
                direction = 1;
            } else if (pos.x >= 0.5 && pos.y < 0.5) { // y--
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
                direction = 2;
            } else if (pos.x < 0.5 && pos.y < 0.5) { // x--
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
                direction = 3;
            } else {
                Debug.Log("AAAAAAAAAHHHHHHH THIS IS IMPOSSIBLE");
            }

            // Find target tile/wall
            Vector3Int targetCell = dungeonMap.WorldToCell(targetPosition);
            targetCell.z = 0;
            TileBase targetTile = dungeonMap.GetTile(targetCell);
            targetCell = tilePosition;
            TileBase targetWall;
            bool blocked = false;
            string face = "";
            if (direction == 0) {
                face = "left";
            } else if (direction == 1) {
                face = "right";
            } else if (direction == 2) {
                face = "left";
                targetCell.y--;
            } else if (direction == 3) {
                face = "right";
                targetCell.x--;
            }
            targetWall = maps[face].GetTile(targetCell);
            if (targetWall != null) {
                if (targetWall.name.ToLower().IndexOf(face+"door") >= 0 && targetWall.name.ToLower().IndexOf("open") < 0) {
                    blocked = true;
                    // Open door and simulate an attack move on door
                    mapController.OpenDoor(targetCell, direction);
                    attacking = true;
                    highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                    highPoint = targetPosition;
                    targetPosition = startPosition;
                    count = 0.0f;
                } else if (targetWall.name.ToLower().IndexOf(face+"door") >= 0 && targetWall.name.ToLower().IndexOf("open") >= 0) {
                    blocked = false;
                } else if (targetWall.name.ToLower().IndexOf(face) >= 0) {
                    blocked = true;
                }
            }

            // Attack enemy in front?
            targetCell = dungeonMap.WorldToCell(targetPosition);
            EnemyBehavior target = null;
            bool enemyFront = false;
            enemyList = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemyList) {
                target = e.GetComponent<EnemyBehavior>();
                if (target.tilePosition == targetCell) {
                    enemyFront = true;
                    break;
                }
            }

            if (enemyFront && !target.dying) {
                attacking = true;
                highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                highPoint = targetPosition;
                targetPosition = startPosition;
                count = 0.0f;
                Attack(target);
            // Point is valid?
            } else if (!blocked) {//targetTile != null && targetTile.name == "floor") {
                // Init bezier curve
                moving = true;
                tilePosition = dungeonMap.WorldToCell(targetPosition);
                highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                count = 0.0f;
            }
            // Face player in new direction
            if (attacking || moving) {
                if (direction < 2) {
                    facing[0] = "up";
                } else {
                    facing[0] = "down";
                }
                if (direction == 0 || direction == 3) {
                    facing [1] = "left";
                } else {
                    facing[1] = "right";
                }
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
                EndTurn();
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
                EndTurn();
            }
        } else if (dying) {
            if (count < 1.0f) {
                count += 1.0f * 2.5f * Time.deltaTime;
                float t = Mathf.Sin(count * Mathf.PI * 0.5f);
                this.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, t);
            } else {
                Destroy(this.gameObject, 0.5f);
            }
        }
    }

    void Attack(EnemyBehavior target) {
        int roll = Mathf.RoundToInt(Random.Range(1,20));
        roll += attack - target.defense;
        if (roll >= 10) {
            target.Damage(Mathf.RoundToInt(Random.Range(mindmg,maxdmg)));
        } else {
            target.Damage(0);
        }
    }
    
    public void Damage(int dmg) {
        GameObject dmgTextFab = Resources.Load("Prefabs/DamageText") as GameObject;
        GameObject text = Instantiate(dmgTextFab, new Vector3(0,0,0), Quaternion.identity, canvas.transform);
        DmgTextController textCont = text.GetComponent<DmgTextController>();
        textCont.Init(this.transform.position);
        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        if (dmg == 0) {
            textMesh.color = new Color32(255,255,0,255);
            textMesh.text = "miss";
        } else {
            textMesh.text = dmg.ToString();
            hp -= dmg;
            if (hp <= 0) {
                Die();
            }
        }
    }

    private void Die() {
        dying = true;
        count = 0f;
        startAngle = this.transform.rotation;
        targetAngle = startAngle;
        if (facing[1] == "left") {
            targetAngle.z -= 1;
        } else {
            targetAngle.z += 1;
        }
    }

    void EndTurn() {
        entities.BroadcastMessage("turnStart", speed);
    }
}
