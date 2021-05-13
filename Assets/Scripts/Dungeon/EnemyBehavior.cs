using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class EnemyBehavior : MonoBehaviour
{
    public float speed;
    public float timer;
    public bool moving = false;
    public bool attacking = false;
    public bool dying = false;
    private bool last = false;
    public Vector3Int tilePosition;
    public string[] facing;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    private Dictionary<string,Tilemap> maps;
    private GameObject plObj;
    private PlayerController player;
    private EntityController entityController;
    private GameObject[] entities;
    private GameObject canvas;
    private Vector3 targetPosition, startPosition, highPoint;
    private Quaternion startAngle, targetAngle;
    private float count = 1.0f;
    
    // Combat Stats
    public int maxhp, hp, attack, defense, mindmg, maxdmg;

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
        canvas = GameObject.FindWithTag("WorldCanvas");
        //tilePosition = floorMap.WorldToCell(this.transform.position);
        //tilePosition.z = 0;
        plObj = GameObject.FindWithTag("Player");
        player = plObj.GetComponent<PlayerController>();
        entityController = GameObject.FindObjectOfType<EntityController>();
        timer = Random.Range(0, speed);
        facing = new string[2];
        Debug.Log("Enemy "+name+" with speed: "+speed+" and timer: "+timer);
        /*maxhp = 2;
        hp = maxhp;
        attack = 1;
        defense = 0;
        mindmg = 1;
        maxdmg = 2;*/
    }

    public void MyTurn(bool last=false) {
        this.last = last;
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
            Vector3Int targetCell = tilePosition;
            if (direction == 0) {
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
                targetCell.y++;
            } else if (direction == 1) {
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
                targetCell.x++;
            } else if (direction == 2) {
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
                targetCell.y--;
            } else if (direction == 3) {
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
                targetCell.x--;
            }

            // Find target tile/wall
            bool blocked = false;
            TileBase targetTile = blockMap.GetTile(targetCell);
            if (targetTile != null) {
                blocked = true;
            }
            TileBase targetWall;
            Vector3Int wallCell = tilePosition;
            string face = "";
            if (direction == 0) {
                face = "left";
            } else if (direction == 1) {
                face = "right";
            } else if (direction == 2) {
                face = "left";
                wallCell.y--;
            } else if (direction == 3) {
                face = "right";
                wallCell.x--;
            }

            targetWall = maps[face].GetTile(wallCell);
            if (targetWall != null) {
                if (targetWall.name.ToLower().IndexOf(face) >= 0) {
                    if (targetWall.name.ToLower().IndexOf("open") >= 0) {
                        blocked = false;
                    } else 
                        blocked = true;
                    }
            }
            
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
            
            if (targetCell == player.tilePosition && !blocked) { // ATTACK!
                attacking = true;
                targetPosition = startPosition;
                highPoint = player.transform.position;
                count = 0.0f;
                Attack(player);
                break;
            } else if (!blocked && !friend) { // Move
                // Init bezier curve
                moving = true;
                highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                count = 0.0f;
                tilePosition = targetCell;
                break;
            } else { // Try again
                if (i == 0) { // First, turn left or right
                    if (Random.Range(-1,1) < 0 ) {
                        direction = (direction+3) % 4;
                    } else {
                        direction = (direction+1) % 4;
                    }
                } else if (i == 1) { // Then, try the other way
                    direction = (direction+2) % 4;
                } else if (i == 2 && last) {
                    //Debug.Log(name+" Designated reporter reporting from uselessness.");
                    entityController.doubleTurn();
                }
                continue;
            }
        }
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
    void Attack(PlayerController target) {
        int roll = Mathf.RoundToInt(Random.Range(1,20+1));
        roll += attack - target.defense;
        if (roll >= 10) {
            target.Damage(Random.Range(mindmg,maxdmg+1));
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
}