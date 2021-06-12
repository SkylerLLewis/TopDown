using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private string[] walkNames, attackNames;
    public float speed, food;
    public bool moving = false, attacking = false, dying = false, waiting = false, readyToEnd = false;
    public string saySomething;
    public int direction;
    public Vector3Int tilePosition;
    public string[] facing;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    private string mapType;
    private Dictionary<string,Tilemap> maps;
    private PersistentData data;
    private DungeonController dungeonController;
    private VillageController villageController;
    System.Action<Vector3Int> NotableCollide;
    System.Action<Vector3Int, int> OpenDoor;
    System.Action<Vector3Int> OpenChest;
    System.Func<Vector3Int> FetchPosition;
    private GameObject entities;
    private EntityController entityController;
    private GameObject mainCamera;
    private GameObject canvas;
    private UIController uiController;
    private GameObject textFab;
    private GameObject[] enemyList;
    private Vector3 targetPosition, startPosition, highPoint;
    private Quaternion startAngle, targetAngle;
    private float count = 1.0f, combatCounter = 0f;
    private int combatIsActive = 5;
    public bool inCombat = false;
    
    // Combat Stats
    public Weapon weapon;
    public Armor armor;
    public int maxhp, hp, attack, defense, mindmg, maxdmg, armorDR;

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
            mapType = "Dungeon";
            NotableCollide = dungeonController.NotableCollide;
            OpenDoor = dungeonController.OpenDoor;
            OpenChest = dungeonController.OpenChest;
        }
        villageController = floorMap.GetComponent<VillageController>();
        if (villageController != null) {
            mapType = "Village";
            NotableCollide = villageController.NotableCollide;
            OpenDoor = villageController.OpenDoor;
            FetchPosition = villageController.FetchPosition;
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
        if (FetchPosition != null) {
            tilePosition = FetchPosition();
            Vector3 pos = PathFinder.TileToWorld(tilePosition);
            pos.y += 0.25f;
            transform.position = pos;
            animator.CrossFade(walkNames[data.direction], 0f);
        }
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
        food = data.food;
        uiController.UpdateBars();
        attack = 5;
        defense = 5;
        speed = 1;
        mindmg = 0;
        maxdmg = 0;
        armorDR = 0;
        EquipWeapon(data.weapon);
        if (data.armor != null) {
            EquipArmor(data.armor);
        }
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
        if (Input.GetMouseButton(0) && !moving && !waiting && !attacking && !entityController.enemyTurn && !dying) {

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
                if (inCombat) {
                    FloatText("wait");
                } else {
                    FloatText("wait", "rest");
                }
                waiting = true;
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
                        if (mapType == "Dungeon") {
                            // Open door and simulate an attack move on door
                            blocked = true;
                            OpenDoor(wallCell, direction);
                            attacking = true;
                            highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                            highPoint = targetPosition;
                            targetPosition = startPosition;
                            count = 0.0f;
                            // You're in combat now
                            if (entities.transform.childCount > 0) {
                                combatCounter = combatIsActive;
                                inCombat = true;
                            }
                        } else if (mapType == "Village") {
                            // It's a village, just walk on in.
                            OpenDoor(wallCell, direction);
                            blocked = false;
                        }
                    } else {
                        blocked = false;
                    }
                } else if (targetWall.name.ToLower().IndexOf(face) >= 0) {
                    blocked = true;
                }
            }

            TileBase targetTile = blockMap.GetTile(targetCell);
            if (targetTile != null && !blocked) {
                // Check for important blocks
                if (targetTile.name == "chest") {
                    // simulate attack move on chest
                    attacking = true;
                    highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                    highPoint = targetPosition;
                    targetPosition = startPosition;
                    count = 0.0f;
                }
                NotableCollide(targetCell);
                blocked = true;
            }

            if (!blocked) {
                // Attack enemy in front?
                EnemyBehavior target = null;
                bool enemyFront = false;
                enemyList = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var e in enemyList) {
                    target = e.GetComponent<EnemyBehavior>();
                    if (target.tilePosition == targetCell && !target.dying) {
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
                    //EndTurn();
                    readyToEnd = true;
                // Point is valid?
                } else {
                    // Check if I'm walking onto something important
                    NotableCollide(targetCell);
                    // Init bezier curve
                    moving = true;
                    tilePosition = targetCell;
                    highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                    count = 0.0f;
                    //EndTurn();
                    readyToEnd = true;
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
        } else if (Input.GetMouseButtonUp(0) && waiting) {
            waiting = false;
        }
        // End turn after brief delay
        if (readyToEnd && count > 0.2f) {
            readyToEnd = false;
            EndTurn();
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
            } else if (!dying) {
                if (saySomething != "") {
                    FloatText("msg", saySomething);
                    saySomething = "";
                }
                moving = false;
            }
        } else if (attacking) {
            if (count < 1.0f) {
                count += 1.0f * 5 * Time.deltaTime;
                Vector3 m1 = Vector3.Lerp(startPosition, highPoint, count);
                Vector3 m2 = Vector3.Lerp(highPoint, targetPosition, count);
                this.transform.position = Vector3.Lerp(m1, m2, count);
            } else if (!dying) {
                animator.CrossFade(walkNames[direction], 0f);
                // Turn over, activate entities
                attacking = false;
            }
        } else if (dying) {
            
        }
    }

    void Attack(EnemyBehavior target) {
        combatCounter = combatIsActive;
        inCombat = true;
        int roll = Mathf.RoundToInt(Random.Range(1,20+1));
        roll += attack - target.defense;
        int crit = 5 + attack - target.defense;
        if (roll >= 8) { // 65% baseline chance to hit, missing sucks.
            int dmg = Random.Range(mindmg,maxdmg+1);
            if (Random.Range(0, 100) < crit) {
                for (int i=0; i<(weapon.crit-1); i++) {
                    dmg += Random.Range(mindmg,maxdmg+1);
                }
                target.Damage(dmg, "crit");
            } else {
                target.Damage(dmg, "dmg");
            }
        } else {
            target.Damage(0, "miss");
        }
    }
    
    public void Damage(int dmg, string style, bool combat=true) {
        FloatText(style, dmg.ToString());
        if (dmg != 0) {
            dmg -= armorDR;
            if (dmg < 1) dmg = 1;
            hp -= dmg;
            if (hp <= 0) {
                Die();
            }
            if (combat) {
                combatCounter = combatIsActive;
                inCombat = true;
            }
        }
        uiController.UpdateBars();
    }

    public void Heal(int heal) {
        hp += heal;
        if (hp > maxhp) {
            hp = maxhp;
        }
        FloatText("heal", heal.ToString());
    }

    public void Feed(int f) {
        food += f;
        if (food > 1000) {
            food = 1000;
        }
    }

    public void EquipWeapon(Weapon w) {
        if (weapon != null) { // Unequip old weapon
            attack -= weapon.atk;
            defense -= weapon.def;
            mindmg -= weapon.mindmg;
            maxdmg -= weapon.maxdmg;
            speed /= (1/weapon.speed);
        }
        weapon = w;
        attack += weapon.atk;
        defense += weapon.def;
        mindmg += weapon.mindmg;
        maxdmg += weapon.maxdmg;
        speed *= (1/weapon.speed);
    }

    public void EquipArmor(Armor a) {
        if (armor != null) { // Unequip old armor
            defense -= armor.def;
            speed /= (1/armor.speed);
            armorDR -= armor.armor;
            mindmg -= armor.dmg;
            maxdmg -= armor.dmg;
        }
        armor = a;
        defense += armor.def;
        speed *= (1/armor.speed);
        armorDR += armor.armor;
        mindmg += armor.dmg;
        maxdmg += armor.dmg;
    }

    public void FloatText(string style, string msg="") {
        GameObject text = Instantiate(textFab, new Vector3(0,0,0), Quaternion.identity, canvas.transform);
        DmgTextController textCont = text.GetComponent<DmgTextController>();
        textCont.Init(this.transform.position, style, msg);
    }

    public void Ability(string abilityName) {
        Debug.Log("Ability "+abilityName+" activated!");
    }

    private void Die() {
        dying = true;
        animator.CrossFade("die", 0f);
    }

    public void EndTurn() {
        if (food > 0) {
            if (combatCounter <= 0) {
                if (hp != maxhp && Random.Range(0f,3f) < speed) {
                    if (waiting) { // Resting, triple regen & food cost
                        hp += 3;
                        if (hp > maxhp) hp = maxhp;
                        FloatText("heal", "3");
                        food -= speed*2;
                    } else { // Just walking
                        hp++;
                        FloatText("heal", "1");
                    }
                }
            } else if (combatCounter > 0) {
                combatCounter -= speed;
                if (combatCounter <= 0) {
                    inCombat = false;
                }
            }
            food -= speed;
        }
        uiController.UpdateBars();
        entities.BroadcastMessage("turnStart", speed);
    }

    void OnDestroy() {
        data.playerHp = hp;
        data.food = food;
    }
}
