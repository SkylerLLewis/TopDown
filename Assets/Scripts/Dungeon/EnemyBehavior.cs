using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class EnemyBehavior : MonoBehaviour
{
    public float moveSpeed, attackSpeed, visualSpeed;
    public float timer;
    public bool moving=false, attacking=false, dying=false, waiting=false;
    public Vector3Int tilePosition;
    public string[] facing;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    private Dictionary<string,Tilemap> maps;
    private GameObject arrow;
    private PlayerController player;
    private EntityController entityController;
    private PathFinder pathFinder;
    private GameObject[] entities;
    private GameObject canvas;
    private Vector3 targetPosition, startPosition, highPoint;
    private Quaternion startAngle, targetAngle;
    private float count = 1.0f;
    
    // Combat Stats
    public int maxhp, hp, attack, defense, mindmg, maxdmg;
    public string enemyType;

    public void SetCoords(Vector3Int cell) {
        tilePosition = cell;
    }
    void Start() {
        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "FloorMap") {
                floorMap = map;
            } else if (map.name == "LeftWallMap") {
                leftWallMap = map;
            } else if (map.name == "RightWallMap") {
                rightWallMap = map;
            } else if (map.name == "BlockMap") {
                blockMap = map;
            }
        }
        maps = new Dictionary<string, Tilemap>();
        maps.Add("left", leftWallMap);
        maps.Add("right", rightWallMap);
        pathFinder = floorMap.GetComponent<PathFinder>();
        canvas = GameObject.FindWithTag("WorldCanvas");
        //tilePosition = floorMap.WorldToCell(this.transform.position);
        //tilePosition.z = 0;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        entityController = GameObject.FindObjectOfType<EntityController>();
        arrow = Resources.Load<GameObject>("Prefabs/Arrow");

        timer = Random.Range(0, moveSpeed);
        facing = new string[2];
        visualSpeed = 8-2*moveSpeed;
    }

    public void MyTurn() {
        targetPosition = this.transform.position;
        startPosition = this.transform.position;
        // A numerical direction that can rotate 0-3
        int direction = 0;
        int [] directions = new int[3];
        Vector3Int targetCell;

        int deltax = player.tilePosition.x - tilePosition.x;
        int deltay = player.tilePosition.y - tilePosition.y;
        // Manhattan distance
        int distance = Mathf.Abs(deltax) + Mathf.Abs(deltay);

        // Ranged? Run away!
        if (enemyType == "Ranged") {
            if (distance < 4) {
                deltax *= -1;
                deltay *= -1;
            }
        }

        // Get uninformed direction
        if (deltay > 0) {
            direction = 0;
        } else if (deltax > 0) {
            direction = 1;
        } else if (deltay < 0) {
            direction = 2;
        } else if (deltax < 0) {
            direction = 3;
        }
        
        if (enemyType == "Melee") {
            if (distance == 1 &&
            pathFinder.DirectionWalkable(tilePosition, direction)) {
                // Player in range, ATTACK!
                
                targetCell = pathFinder.DirectionToCell(direction, tilePosition);attacking = true;
                targetPosition = startPosition;
                highPoint = player.transform.position;
                count = 0.0f;
                Attack(player);
                // Face enemy in new direction
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
                EndTurn(attackSpeed);
                return; // Attack successful, turn over.
            }
        } else if (enemyType == "Ranged" && (distance > 2 || distance == 1)) {
            if (pathFinder.LineOfSight(tilePosition, player.tilePosition)) {
                // Player in line of sight, ATTACK!

                waiting = true;
                count = 1.0f;
                RangedAttack(player);

                // Face enemy in new direction
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
                EndTurn(attackSpeed);
                return; // Attack successful, turn over.
            }
        }

        // Try to move
        // Decide best direction for moving
        if (Mathf.Abs(deltay) >= Mathf.Abs(deltax)) {
            if (deltay >= 0) {
                direction = 0;
            } else {
                direction = 2;
            }
            if (!pathFinder.DirectionWalkable(tilePosition, direction, "enemy")) {
                if (deltax >= 0) {
                    direction = 1;
                } else {
                    direction = 3;
                }
                if (!pathFinder.DirectionWalkable(tilePosition, direction, "enemy")) {
                    direction = (direction + 2) % 4;
                    if (!pathFinder.DirectionWalkable(tilePosition, direction, "enemy")) {
                        direction = -1;
                    }
                }
            }
        } else if (Mathf.Abs(deltax) >= Mathf.Abs(deltay)) {
            if (deltax >= 0) {
                direction = 1;
            } else {
                direction = 3;
            }
            if (!pathFinder.DirectionWalkable(tilePosition, direction, "enemy")) {
                if (deltay >= 0) {
                    direction = 0;
                } else {
                    direction = 2;
                }
                if (!pathFinder.DirectionWalkable(tilePosition, direction, "enemy")) {
                    direction = (direction + 2) % 4;
                    if (!pathFinder.DirectionWalkable(tilePosition, direction, "enemy")) {
                        direction = -1;
                    }
                }
            }
        }
        
        if (direction != -1) {
            // Move!
            targetCell = pathFinder.DirectionToCell(direction, tilePosition);
            targetPosition = floorMap.CellToWorld(targetCell);
            targetPosition.y += 0.25f;
            targetPosition.z = 0;
            
            moving = true;
            highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
            count = 0.0f;
            tilePosition = targetCell;
            // Face enemy in new direction
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
            EndTurn(moveSpeed);
        } else {
            // Not doing jack, report back
            waiting = true;
            count = 0f;
            EndTurn(moveSpeed);
        }
    }

    void Update() {
        if (moving) {
            if (count < 1.0f) {
                count += 1.0f * visualSpeed * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
            } else {
                moving = false;
                entityController.Report();
            }
        } else if (attacking) {
            if (count < 1.0f) {
                count += 1.0f * visualSpeed * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
            } else {
                attacking = false;
                entityController.Report();
            }
        } else if (dying) {
            if (count < 1.0f) {
                count += 1.0f * visualSpeed * Time.deltaTime;
                float t = Mathf.Sin(count * Mathf.PI * 0.5f);
                this.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, t);
            } else {
                Destroy(this.gameObject, 0.5f);
            }
        } else if (waiting) {
            if (count < 1.0f) {
                count += 1.0f * visualSpeed * Time.deltaTime;
            } else {
                waiting = false;
                entityController.Report();
            }
        }
    }
    void Attack(PlayerController target) {
        int roll = Mathf.RoundToInt(Random.Range(1,20+1));
        roll += attack - target.defense;
        if (roll >= 8) {
            target.Damage(Random.Range(mindmg,maxdmg+1));
        } else {
            target.Damage(0);
        }
    }

    void RangedAttack(PlayerController target) {
        int damage;
        int roll = Mathf.RoundToInt(Random.Range(1,20+1));
        roll += attack - target.defense;
        if (roll >= 8) {
            damage = Random.Range(mindmg,maxdmg+1);
        } else {
            damage = 0;
        }
        GameObject clone = Instantiate(
            arrow,
            transform.position,
            Quaternion.identity);
        clone.name = clone.name.Split('(')[0];
        clone.GetComponent<ArrowController>().Shoot(player, damage);
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

    private void EndTurn(float time) {
        timer += time;
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
}
