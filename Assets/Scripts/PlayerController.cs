using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private string[] walkNames, attackNames;
    public bool moving = false, attacking = false, dying = false, waiting = false, readyToEnd = false;
    public string saySomething = "";
    public int direction;
    public Vector3Int tilePosition;
    public string[] facing;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    private PathFinder pathFinder;
    private List<Vector3Int> rangedTiles;
    private Dictionary<string,Tilemap> maps;
    private PersistentData data;
    public DungeonController dungeonController;
    private VillageController villageController;
    System.Action<Vector3Int> NotableCollide;
    System.Action<Vector3Int, int> OpenDoor;
    System.Action<Vector3Int> OpenChest;
    System.Action<List<Vector3Int>,Color> HighlightTiles;
    System.Func<Vector3Int> FetchPosition;
    private GameObject entities;
    private EntityController entityController;
    private GameObject mainCamera;
    private Camera _mainCamera;
    private GameObject canvas;
    private UIController uiController;
    private GameObject textFab, projectile;
    private Sprite magicMissileSprite;
    private GameObject[] enemyList;
    private Vector3 targetPosition, startPosition, highPoint;
    private Quaternion startAngle, targetAngle;
    private float count = 1.0f, combatCounter = 0f;
    private int combatIsActive = 5;
    public bool inCombat = false;
    private string rangedToExecute = "";
    public Room currentRoom;
    public List<Effect> effects;
    Skill usingSkill;
    
    // Combat Stats
    public Weapon weapon;
    public Armor armor;
    float manaCounter = 0;
    public int maxhp, hp, attack, defense, mindmg, maxdmg, armorDR, maxMana, mana;
    public float speed, food, manaRegen;

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
            OpenChest = dungeonController.OpenChest;
            HighlightTiles = dungeonController.HighlightTiles;
        }
        villageController = floorMap.GetComponent<VillageController>();
        if (villageController != null) {
            NotableCollide = villageController.NotableCollide;
            OpenDoor = villageController.OpenDoor;
            FetchPosition = villageController.FetchPosition;
        }
        pathFinder = floorMap.GetComponent<PathFinder>();
        rangedTiles = new List<Vector3Int>();
        maps = new Dictionary<string, Tilemap>();
        maps.Add("left", leftWallMap);
        maps.Add("right", rightWallMap);
        uiController = GameObject.Find("UICanvas").GetComponent<UIController>();
        canvas = GameObject.FindWithTag("WorldCanvas");

        entities = GameObject.FindWithTag("EntityList");
        entityController = entities.GetComponent<EntityController>();

        // Load Resources
        walkNames = new string[4] {"walkUp", "walkRight", "walkDown", "walkLeft"};
        attackNames = new string[4] {"attackUp", "attackRight", "attackDown", "attackLeft"};
        targetPosition = this.transform.position;
        textFab = Resources.Load("Prefabs/DamageText") as GameObject;
        projectile = Resources.Load("Prefabs/PlayerProjectile") as GameObject;
        
        magicMissileSprite = Resources.Load<Sprite>("Weapons/Bolt");

        // Set Position
        tilePosition = new Vector3Int(0,0,0);
        if (FetchPosition != null) {
            tilePosition = FetchPosition();
            Vector3 pos = PathFinder.TileToWorld(tilePosition);
            pos.y += 0.25f;
            transform.position = pos;
            animator.CrossFade(walkNames[data.direction], 0f);
        }
        if (data.mapType == "Dungeon") {
            currentRoom = dungeonController.GetRooms()[0];
        }
        facing = new string[2];

        // Orient Camera
        mainCamera = GameObject.FindWithTag("MainCamera");
        _mainCamera = mainCamera.GetComponent<Camera>();
        Vector3 camVec = this.transform.position;
        camVec.z = -10;
        mainCamera.transform.position = camVec;

        // Set Attributes
        ApplySkills();
        uiController.UpdateHp();
        uiController.UpdateMana();
        uiController.UpdateFood();
        attack = 5;
        defense = 5;
        mindmg = 0;
        maxdmg = 0;
        armorDR = 0;
        manaRegen = 0;
        EquipWeapon(data.weapon);
        if (data.armor != null) {
            EquipArmor(data.armor);
        }
        // Apply effects
        effects = new List<Effect>();
        if (data.playerEffects != null) {
            effects = data.playerEffects;
            foreach (Effect e in effects) {
                e.Apply(this);
            }
        }
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
            if (pos.x<0.11 || pos.x>0.92) {
                return;
            }

            // Use Ability?
            if (rangedToExecute != "") {
                waiting = true;
                Vector3Int aimCell = PathFinder.WorldToTile(
                    _mainCamera.ScreenToWorldPoint(
                        Input.mousePosition));
                enemyList = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject Eobj in enemyList) {
                    EnemyBehavior e = Eobj.GetComponent<EnemyBehavior>();
                    if (aimCell == e.tilePosition && rangedTiles.Contains(aimCell)) {
                        ExecuteRanged(e);
                        break;
                    }
                }
                return;
            }

            // Use ranged weapon?
            if (weapon.ranged) {
                enemyList = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemyList.Length > 0) {
                    Vector3Int aimCell = PathFinder.WorldToTile(
                        _mainCamera.ScreenToWorldPoint(
                            Input.mousePosition));
                    RangeFind(10);
                    foreach (GameObject Eobj in enemyList) {
                        EnemyBehavior e = Eobj.GetComponent<EnemyBehavior>();
                        if (!e.dying && aimCell == e.tilePosition && rangedTiles.Contains(aimCell)) {
                            waiting = true;
                            ShootArrow(e);
                            EndTurn(1/weapon.attackSpeed);
                            return;
                        }
                    }
                }
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
                        if (data.mapType == "Dungeon") {
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
                            readyToEnd = true;
                        } else if (data.mapType == "Village") {
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

            // Check blockages
            if (!blocked) {
                TileBase targetTile = blockMap.GetTile(targetCell);
                if (targetTile != null) {
                    // Check for important blocks
                    NotableCollide(targetCell);
                    blocked = true;
                }
            }

            // Are there enemies?
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
                    if (!weapon.ranged) {
                        attacking = true;
                        highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                        highPoint = targetPosition;
                        targetPosition = startPosition;
                        count = 0.0f;
                        Attack(target);
                        readyToEnd = true;
                    } else {
                        waiting = true;
                        ShootArrow(target);
                        EndTurn(1/weapon.attackSpeed);
                        return;
                    }

                } else { // No walls, blocks or enemies: move
            
                    // Check if I'm walking onto something important
                    TileBase targetTile = floorMap.GetTile(targetCell);
                    if (targetTile != null && targetTile.name == "chest") {
                        NotableCollide(targetCell);
                        // simulate attack move on chest
                        attacking = true;
                        highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                        highPoint = targetPosition;
                        targetPosition = startPosition;
                        count = 0.0f;
                        readyToEnd = true;
                    } else {
                        // All good to move
                        NotableCollide(targetCell);
                        moving = true;
                        tilePosition = targetCell;
                        highPoint = startPosition +(targetPosition -startPosition)/2 +Vector3.up *0.5f;
                        count = 0.0f;
                        readyToEnd = true;
                    }
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
            // Check if room needs to change
            if (data.mapType == "Dungeon") {
                if (!currentRoom.Contains(tilePosition)) {
                    currentRoom = Room.FindByCell(tilePosition, dungeonController.GetRooms());
                }
            }
        } else if (Input.GetMouseButtonUp(0) && waiting) {
            // Each wait must be an individual click
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
        if (dying) return;
        if (dmg != 0) {
            dmg -= armorDR;
            if (dmg < 1) dmg = 1;
            FloatText(style, dmg.ToString());
            hp -= dmg;
            uiController.UpdateHp();
            if (hp <= 0) {
                Die();
            }
        } else {
            FloatText(style, dmg.ToString());
        }
        if (combat) {
            combatCounter = combatIsActive;
            inCombat = true;
        }
    }

    public void Heal(int heal) {
        hp += heal;
        if (hp > maxhp) hp = maxhp;
        FloatText("heal", heal.ToString());
        uiController.UpdateHp();
    }

    public void GetMana(int m) {
        mana += m;
        if (mana > maxMana) mana = maxMana;
        FloatText("mana", m.ToString());
        uiController.UpdateMana();
    }

    public void Feed(int f) {
        food += f;
        if (food > 1000) {
            food = 1000;
        }
    }

    public void GetXP(int xp) {
        data.xp += xp;
        if (data.xp >= data.nextLevel) {
            data.LevelUp();
            FloatText("msg", "Level up!");
        }
    }

    public void ApplySkills() {
        Armor a = armor;
        Weapon w = weapon;
        UnequipArmor();
        UnequipWeapon();
        maxMana = 10;
        mana = data.mana;
        if (mana == 0) mana = maxMana;
        maxhp = 20;
        hp = data.playerHp;
        if (hp == 0) hp = maxhp;
        food = data.food;
        speed = 1;
        foreach (Skill s in data.skills) {
            if (s.abilityType == "improvement") {
                if (s.stat == "Health") {
                    maxhp += s.amount*s.magnitude;
                } else if (s.stat == "Mana") {
                    maxMana += s.amount*s.magnitude;
                } else if (s.stat == "Speed") {
                    speed = 1f - (0.1f*s.amount*s.magnitude);
                }
            }
        }
        if (a != null) EquipArmor(a);
        if (w != null) EquipWeapon(w);
    }

    public void EquipWeapon(Weapon w) {
        if (weapon != null) {
            attack -= weapon.atk;
            defense -= weapon.def;
            mindmg -= weapon.mindmg;
            maxdmg -= weapon.maxdmg;
            speed /= (1/weapon.speed);
            manaRegen -= weapon.manaRegen;
        }
        weapon = w;
        attack += weapon.atk;
        defense += weapon.def;
        mindmg += weapon.mindmg;
        maxdmg += weapon.maxdmg;
        speed *= (1/weapon.speed);
        manaRegen += weapon.manaRegen;
    }

    public void UnequipWeapon() {
        if (weapon != null) { // Unequip old weapon
            attack -= weapon.atk;
            defense -= weapon.def;
            mindmg -= weapon.mindmg;
            maxdmg -= weapon.maxdmg;
            speed /= (1/weapon.speed);
            weapon = null;
        }
    }

    public void EquipArmor(Armor a) {
        if (armor != null) { // Unequip old armor
            defense -= armor.def;
            attack -= armor.atk;
            speed /= (1/armor.speed);
            armorDR -= armor.armor;
            mindmg -= armor.dmg;
            maxdmg -= armor.dmg;
        }
        armor = a;
        defense += armor.def;
        attack += armor.atk;
        speed *= (1/armor.speed);
        armorDR += armor.armor;
        mindmg += armor.dmg;
        maxdmg += armor.dmg;
    }

    public void UnequipArmor() {
        if (armor != null) { // Unequip old armor
            defense -= armor.def;
            attack -= armor.atk;
            speed /= (1/armor.speed);
            armorDR -= armor.armor;
            mindmg -= armor.dmg;
            maxdmg -= armor.dmg;
            armor = null;
        }
    }

    public void SpeedEffect(float s, float dur) {
        effects.Add(new Effect("Speed", (1/s), dur, this));
        FloatText("msg", "I am speed");
    }

    public void RegenEffect(int regen, float dur) {
        effects.Add(new Effect("Regeneration", regen, dur, this));
    }

    public void FloatText(string style, string msg="") {
        GameObject text = Instantiate(textFab, new Vector3(0,0,0), Quaternion.identity, canvas.transform);
        DmgTextController textCont = text.GetComponent<DmgTextController>();
        textCont.Init(this.transform.position, style, msg);
    }

    public void Ability(Skill s) {
        if (s.abilityType == "magic") {
            if (mana < s.manaCost) return;

            if (s.name == "Magic Missile") {
                usingSkill = s;
                rangedToExecute = s.name;
                RangeFind(range:6);
                HighlightTiles(rangedTiles, new Color(0.5f,0.5f,1,1));
            } else if (s.name == "Lesser Heal") {
                Heal(15);
                mana -= s.manaCost;
                uiController.UpdateMana();
                EndTurn(1);
            }
        }
    }

    public void CancelAbility(string abilityName) {
        rangedToExecute = "";
        HighlightTiles(rangedTiles, new Color(1,1,1,1));
    }

    private void ExecuteRanged(EnemyBehavior e) {
        if (rangedToExecute == "Magic Missile") {
            MagicMissile(e);
        }
        rangedToExecute = "";
        HighlightTiles(rangedTiles, new Color(1,1,1,1));
    }

    void MagicMissile(EnemyBehavior target) {
        int damage;
        int roll = Mathf.RoundToInt(Random.Range(1,20+1))+5;
        string style = "dmg";
        if (roll >= 8) {
            damage = Random.Range(2,5+1);
            if (Random.Range(0, 100) < 10) {
                damage += Random.Range(2,5+1);
                style = "crit";
            }
        } else {
            damage = 0;
            style = "miss";
        }
        GameObject clone = Instantiate(
            projectile,
            transform.position,
            Quaternion.identity);
        clone.name = clone.name.Split('(')[0];
        clone.GetComponent<SpriteRenderer>().sprite = magicMissileSprite;
        clone.GetComponent<ProjectileController>().Shoot(target, damage, style);
        target.FutureDamage(damage);
        EndTurn(2*speed);
        uiController.UsedSkill("Magic Missile");
        mana -= usingSkill.manaCost;
        uiController.UpdateMana();
    }

    private void RangeFind(int range) {
        rangedTiles = pathFinder.GetTilesInSight(tilePosition, 6);
    }

    private void ShootArrow(EnemyBehavior target) {
        combatCounter = combatIsActive;
        inCombat = true;
        int dmg;
        string style;
        int roll = Mathf.RoundToInt(Random.Range(1,20+1));
        roll += attack - target.defense;
        int crit = 5 + attack - target.defense;
        if (roll >= 8) { // 65% baseline chance to hit, missing sucks.
            dmg = Random.Range(mindmg,maxdmg+1);
            if (Random.Range(0, 100) < crit) {
                for (int i=0; i<(weapon.crit-1); i++) {
                    dmg += Random.Range(mindmg,maxdmg+1);
                }
                style = "crit";
            } else {
                style = "dmg";
            }
        } else {
            style = "miss";
            dmg = 0;
        }
        target.FutureDamage(dmg);
        GameObject clone = Instantiate(
            projectile,
            transform.position,
            Quaternion.identity);
        clone.name = clone.name.Split('(')[0];
        clone.GetComponent<ProjectileController>().Shoot(target, dmg, style);
    }

    private void Die() {
        if (dying) return;
        dying = true;
        animator.CrossFade("die", 0f);
        data.depth = 0;
        data.gold -= 20;
        if (data.gold < 0) data.gold = 0;
        data.entrance = 3;
        data.direction = 1;
        hp = 1;
        data.LoadingScreenLoad("GreenVillage", "death");
    }

    private void RegenMana() {
        manaCounter += 1+manaRegen;
        int regenAmount = Mathf.FloorToInt(manaCounter);
        mana += regenAmount;
        if (mana > maxMana) mana = maxMana;
        manaCounter -= regenAmount;
        FloatText("mana", regenAmount.ToString());
        uiController.UpdateMana();
    }

    public void EndTurn(float speedMod=1) {
        if (food > 0) {
            if (combatCounter <= 0) {
                // Resting, quadruple regen & food cost
                if (waiting) {
                    food -= speed*3;
                    if(hp != maxhp && Random.Range(0f,2f) < speed) {
                        hp += 2;
                        if (hp > maxhp) hp = maxhp;
                        FloatText("heal", "2");
                        uiController.UpdateHp();
                    }
                    if (mana != maxMana && Random.Range(0,2.66f) < speed) {
                        RegenMana();
                    }
                } else {// Just walking
                    if(hp != maxhp && Random.Range(0f,4f) < speed) {
                        hp++;
                        FloatText("heal", "1");
                        uiController.UpdateHp();
                    }
                }
            }
            // Mana regens even in combat
            if (mana != maxMana && Random.Range(0,8f) < speed) {
                RegenMana();
            }
            food -= speed;
            uiController.UpdateFood();
        } 
        if (combatCounter > 0) {
            combatCounter -= speed;
            if (combatCounter <= 0) {
                inCombat = false;
            }
        }
        // Cycle through active effects
        for (int i=effects.Count-1; i >= 0; i--) {
            Effect e = effects[i];
            e.duration -= speed;
            e.Ongoing(this);
            if (e.duration <= 0) {
                e.Remove(this);
                effects.RemoveAt(i);
            }
        }
        entities.BroadcastMessage("turnStart", speed*speedMod);
    }

    void OnDestroy() {
        data.playerHp = hp;
        data.mana = mana;
        data.food = food;
        data.playerEffects = effects;
    }

    // A bundle of active effects for easy saving
    public class Effect {
        public string typeName;
        public float effect, duration, effectTimer;
        public Effect(string t, float e, float dur, PlayerController p) {
            typeName = t;
            effect = e;
            duration = dur;
            effectTimer = 0;
            Apply(p);
        }

        public void Apply(PlayerController p) {
            if (typeName == "Speed") {
                Debug.Log("Applying speed "+effect+"!");
                p.speed *= effect;
                Debug.Log("New speed: "+p.speed);
            }
        }

        public void Ongoing(PlayerController p) {
            if (typeName == "Regeneration") {
                effectTimer += p.speed;
                int heal = Mathf.FloorToInt(effectTimer);
                if (p.hp < p.maxhp && heal > 0) {
                    p.Heal(heal);
                    effectTimer -= heal;
                }
            }
        }

        public void Remove(PlayerController p) {
            if (typeName == "Speed") {
                p.speed /= effect;
            }
            p.FloatText("msg", typeName+" wore off");
        }
    }
}
