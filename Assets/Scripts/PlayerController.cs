using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private string[] walkNames, attackNames;
    public float speed = 1;
    public bool moving = false;
    public bool attacking = false;
    public bool dying = false;
    private string saySomething;
    public int direction;
    public Vector3Int tilePosition;
    public string[] facing;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    private Dictionary<string,Tilemap> maps;
    private PersistentData data;
    private DungeonController dungeonController;
    private VillageController villageController;
    System.Action<Vector3Int> NotableCollide;
    System.Action<Vector3Int, int> OpenDoor;
    private GameObject entities;
    private EntityController entityController;
    private GameObject mainCamera;
    private GameObject canvas;
    private UIController uiController;
    private GameObject textFab;
    private GameObject[] enemyList;
    private Vector3 targetPosition, startPosition, highPoint;
    private Quaternion startAngle, targetAngle;
    private float count = 1.0f;
    
    // Combat Stats
    public Weapon weapon;
    public int maxhp, hp, attack, defense, mindmg, maxdmg;

    void Start() {
        // Load Controllers
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        animator = gameObject.GetComponent<Animator>();
        // Load Map Controllers
        Grid grid = FindObjectOfType<Grid>();
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
        dungeonController = floorMap.GetComponent<DungeonController>();
        if (dungeonController != null) {
            NotableCollide = dungeonController.NotableCollide;
            OpenDoor = dungeonController.OpenDoor;
        }
        villageController = floorMap.GetComponent<VillageController>();
        if (villageController != null) {
            NotableCollide = villageController.NotableCollide;
            OpenDoor = villageController.OpenDoor;
        }
        maps = new Dictionary<string, Tilemap>();
        maps.Add("left", leftWallMap);
        maps.Add("right", rightWallMap);
        uiController = GameObject.Find("UICanvas").GetComponent<UIController>();
        canvas = GameObject.FindWithTag("WorldCanvas");

        // Load Resources
        walkNames = new string[4] {"walkUp", "walkRight", "walkDown", "walkLeft"};
        attackNames = new string[4] {"attackUp", "attackRight", "attackDown", "attackLeft"};
        targetPosition = this.transform.position;
        textFab = Resources.Load("Prefabs/DamageText") as GameObject;

        // Set Attributes
        tilePosition = new Vector3Int(0,0,0);
        entities = GameObject.FindWithTag("EntityList");
        entityController = entities.GetComponent<EntityController>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        Vector3 camVec = this.transform.position;
        camVec.z = -10;
        mainCamera.transform.position = camVec;
        facing = new string[2];
        maxhp = 20;
        hp = data.playerHp;
        if (hp == 0) {
            hp = maxhp;
        }
        uiController.UpdateHP(hp, maxhp);
        EquipWeapon(data.weapon);
        saySomething = "";
    }


    public Vector3 relativePos;
    public Vector3 pos;
    // Update is called once per frame
    void Update() {
        /*if (Input.GetMouseButtonDown(0)) {
            if (moving) {
                Debug.Log("Can't, I'm moving!");
            }
            if (entityController.enemyTurn) {
                Debug.Log("Can't, enemy's turn!");
            }
        }*/
        if (Input.GetMouseButtonDown(0) && !moving && !attacking && !entityController.enemyTurn && !dying) {

            // Determine screen position
            Vector2 pos = new Vector2(
                Input.mousePosition.x/Screen.width,
                Input.mousePosition.y/Screen.height);
            
            // Using interface?
            if (pos.x<0.2 || pos.x>0.8) {
                return;
            }

            // Wait?
            if (Mathf.Abs(pos.x-0.5f)<0.025f && Mathf.Abs(pos.y-0.55f)<0.05f) {
                FloatText("wait");
                EndTurn();
                return;
            }

            // A numerical direction that can rotate 0-3
            direction = 0;

            Vector3Int targetCell = tilePosition;
            // Calculate target pos
            targetPosition = this.transform.position;
            startPosition = this.transform.position;
            if (pos.x < 0.5 && pos.y >= 0.5) { // y++
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
                targetCell.y++;
                direction = 0;
            } else if (pos.x >= 0.5 && pos.y >= 0.5) { // x++
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y + 0.5f;
                targetCell.x++;
                direction = 1;
            } else if (pos.x >= 0.5 && pos.y < 0.5) { // y--
                targetPosition.x = this.transform.position.x + 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
                targetCell.y--;
                direction = 2;
            } else if (pos.x < 0.5 && pos.y < 0.5) { // x--
                targetPosition.x = this.transform.position.x - 1f;
                targetPosition.y = this.transform.position.y - 0.5f;
                targetCell.x--;
                direction = 3;
            }

            // Find target tile/wall
            bool blocked = false;
            Vector3Int wallCell = tilePosition;
            TileBase targetWall = null;
            string face = "";
            if (!blocked) {
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
            }
            if (targetWall != null) {
                if (targetWall.name.ToLower().IndexOf(face+"door") >= 0) { 
                    if (targetWall.name.ToLower().IndexOf("open") < 0) {
                        blocked = true;
                        // Open door and simulate an attack move on door
                        OpenDoor(wallCell, direction);
                        attacking = true;
                        highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                        highPoint = targetPosition;
                        targetPosition = startPosition;
                        count = 0.0f;
                    } else {
                        blocked = false;
                    }
                } else if (targetWall.name.ToLower().IndexOf(face) >= 0) {
                    blocked = true;
                }
            }
            if (!blocked) {
                NotableCollide(targetCell);
            }
            TileBase targetTile = blockMap.GetTile(targetCell);
            if (targetTile != null) {
                blocked = true;
            }

            if (!blocked) {
                // Attack enemy in front?
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
                } else {//targetTile != null && targetTile.name == "floor") {
                    // Init bezier curve
                    moving = true;
                    tilePosition = targetCell; //floorMap.WorldToCell(targetPosition);
                    highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                    count = 0.0f;
                }
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
                if (moving) {
                    animator.CrossFade(walkNames[direction], 0f);
                } else {
                    animator.CrossFade(attackNames[direction], 0f);
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
                if (saySomething != "") {
                    FloatText("msg", saySomething);
                    saySomething = "";
                }
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
                animator.CrossFade(walkNames[direction], 0f);
                // Turn over, activate entities
                attacking = false;
                EndTurn();
            }
        } else if (dying) {
            /*if (count < 1.0f) {
                count += 1.0f * 2.5f * Time.deltaTime;
                float t = Mathf.Sin(count * Mathf.PI * 0.5f);
                this.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, t);
            } else {
                Destroy(this.gameObject, 10f);
            }*/
        }
    }

    void Attack(EnemyBehavior target) {
        int roll = Mathf.RoundToInt(Random.Range(1,20+1));
        roll += attack - target.defense;
        if (roll >= 10) {
            target.Damage(Random.Range(mindmg,maxdmg+1));
        } else {
            target.Damage(0);
        }
    }
    
    public void Damage(int dmg) {
        if (dmg != 0) {
            FloatText("dmg", dmg.ToString());
            hp -= dmg;
            if (hp <= 0) {
                Die();
            }
        } else {
            FloatText("miss");
        }
        uiController.UpdateHP(hp, maxhp);
    }

    public void EquipWeapon(Weapon w) {
        weapon = w;
        attack = 5+weapon.atk;
        defense = 5+weapon.def;
        mindmg = weapon.mindmg;
        maxdmg = weapon.maxdmg;
        speed = weapon.speed;
    }

    private void FloatText(string type, string msg="") {
        GameObject text = Instantiate(textFab, new Vector3(0,0,0), Quaternion.identity, canvas.transform);
        DmgTextController textCont = text.GetComponent<DmgTextController>();
        textCont.Init(this.transform.position);
        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        if (type == "dmg") {
            textMesh.text = msg;
        } else if (type == "miss") {
            textMesh.color = new Color32(255,255,0,255);
            textMesh.text = "miss";
        } else if (type == "heal") {
            textMesh.text = msg;
            textMesh.color = new Color32(0,255,0,255);
        } else if (type == "wait") {
            textMesh.color = new Color32(255,255,255,255);
            textMesh.text = "wait";
        } else if (type == "msg") {
            textMesh.color = new Color32(0,0,0,255);
            textMesh.text = msg;
        } else {
            textMesh.text = "AHHH";
            textMesh.color = new Color32(0,0,255,255);
        }
    }

    public void Ability(string abilityName) {
        Debug.Log("Ability "+abilityName+" activated!");
    }

    private void Die() {
        dying = true;
        animator.CrossFade("die", 0f);
    }

    void EndTurn() {
        if (hp != maxhp && Random.Range(0f,4f) < speed) {
            hp++;
            FloatText("heal", "1");
            uiController.UpdateHP(hp, maxhp);
        }
        entities.BroadcastMessage("turnStart", speed);
    }

    void OnDestroy() {
        data.playerHp = hp;
    }
}
