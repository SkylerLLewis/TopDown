using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    public Dictionary<string,Vector3Int> notableCells;
    Tile floor, leftWall, rightWall, leftRightWall, leftDoor, leftDoorOpen;
    Tile leftClearWall, rightClearWall, leftRightClearWall;
    Dictionary<string,Tile> tiles, clearTiles;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    Vector3Int cellLoc;
    private GameObject enemies;
    private PlayerController player;
    private DungeonController dungeonController;
    private Dictionary<string, GameObject> enemyFabs;
    private Dictionary<string, int> enemyWheel;
    private List<Room> rooms;
    private PersistentData data;
    
    void Awake() {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        enemies = GameObject.FindWithTag("EntityList");
        dungeonController = gameObject.GetComponent<DungeonController>();

        notableCells = new Dictionary<string, Vector3Int>();

        // Load tile Resources into dictionaries
        tiles = new Dictionary<string,Tile>();
        tiles.Add("floor", Resources.Load<Tile>("Tiles/DungeonMap/floor"));
        tiles.Add("floor1", Resources.Load<Tile>("Tiles/DungeonMap/floor1"));
        tiles.Add("floor2", Resources.Load<Tile>("Tiles/DungeonMap/floor2"));
        tiles.Add("floor3", Resources.Load<Tile>("Tiles/DungeonMap/floor3"));
        tiles.Add("leftWall", Resources.Load<Tile>("Tiles/DungeonMap/leftWall"));
        tiles.Add("rightWall", Resources.Load<Tile>("Tiles/DungeonMap/rightWall"));
        tiles.Add("leftDoor", Resources.Load<Tile>("Tiles/DungeonMap/leftDoor"));
        tiles.Add("leftDoorOpen", Resources.Load<Tile>("Tiles/DungeonMap/leftDoorOpen"));
        tiles.Add("rightDoor", Resources.Load<Tile>("Tiles/DungeonMap/rightDoor"));
        tiles.Add("rightDoorOpen", Resources.Load<Tile>("Tiles/DungeonMap/rightDoorOpen"));
        tiles.Add("stairsUp", Resources.Load<Tile>("Tiles/DungeonMap/stairsUp"));
        tiles.Add("stairsDown", Resources.Load<Tile>("Tiles/DungeonMap/stairsDown"));

        clearTiles = new Dictionary<string,Tile>();
        clearTiles.Add("leftWall", Resources.Load<Tile>("Tiles/DungeonMap/leftClearWall"));
        clearTiles.Add("rightWall", Resources.Load<Tile>("Tiles/DungeonMap/rightClearWall"));
        clearTiles.Add("leftDoor", Resources.Load<Tile>("Tiles/DungeonMap/leftDoorClear"));
        clearTiles.Add("leftDoorOpen", Resources.Load<Tile>("Tiles/DungeonMap/leftDoorOpenClear"));
        clearTiles.Add("rightDoor", Resources.Load<Tile>("Tiles/DungeonMap/rightDoorClear"));
        clearTiles.Add("rightDoorOpen", Resources.Load<Tile>("Tiles/DungeonMap/rightDoorOpenClear"));

        // Load Enemy prefabs and likelyhood
        enemyFabs = new Dictionary<string, GameObject>();
        enemyWheel = new Dictionary<string, int>();
        enemyFabs.Add("Skeleton", Resources.Load("Prefabs/Skeleton") as GameObject);
        enemyWheel.Add("Skeleton", 10);
        enemyFabs.Add("Goblin", Resources.Load("Prefabs/Goblin") as GameObject);
        enemyWheel.Add("Goblin", 1);
        enemyFabs.Add("Hobgoblin", Resources.Load("Prefabs/Hobgoblin") as GameObject);
        enemyWheel.Add("Hobgoblin", 1+data.depth);
        enemyFabs.Add("Spider", Resources.Load("Prefabs/Spider") as GameObject);
        enemyWheel.Add("Spider", 1+data.depth);
        if (data.depth > 1 && data.depth < 4) {
            enemyWheel["Skeleton"] = 1;
            enemyWheel["Goblin"] = 15;
        }


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

        // Room collision testing
        Room test1 = new Room(new Vector3Int(-2,-4,0), new Vector3Int(-5,-9,0));
        Room test2 = new Room(new Vector3Int(-3,-3,0), new Vector3Int(-8,-6,0));
        if (test1.Collides(test2)) {
            Debug.Log("They Collide!!");
        } else {
            Debug.Log("They don't collide.");
        }

        // Gen Dungeon
        GenerateDungeon();

        dungeonController.UpdateNotables(notableCells);
    }

    // Actions on collisions
    public void NotableActions(string key) {
        if (key == "stairsUp") {
            data.depth--;
            data.direction = "up";
            if (data.depth == 0) {
                SceneManager.LoadScene("GreenVillage");
            } else {
                SceneManager.LoadScene("BasicDungeon");
            }
        } else if (key == "stairsDown") {
            data.depth++;
            data.direction = "down";
            SceneManager.LoadScene("BasicDungeon");
        }
    }

    // Creates a map of rooms 
    void GenerateDungeon() {
        // Create core room
        rooms = new List<Room>();
        Vector3Int head = new Vector3Int();
        Vector3Int tail = new Vector3Int();
        head.x = Random.Range(1, 4);
        head.y = Random.Range(1, 4);
        tail.x = Random.Range(-3, 0);
        tail.y = Random.Range(-3, 0);
        Room core = new Room(head, tail);
        rooms.Add(core);
        int rand = Random.Range(0,2);
        if (rand == 0) {// String shape Dungeon
            // Create 4 Neighbors
            for (int i=0; i<4; i++) {
                GenerateRoom(core, i);
            }
            // Create string of rooms
            int direction = Random.Range(0, 4);
            Room branch = core.neighbors[direction];
            for (int i=0; i<4; i++) {
                GenerateRoom(branch, direction);
                branch = branch.neighbors[direction];
            }
            // Add Random Rooms
            for (int i=0; i<10; i++) {
                branch = rooms[Random.Range(0, rooms.Count)];
                direction = Random.Range(0, 4);
                int j = 0;
                while (branch.neighbors[direction] != null) {
                    direction = (direction+1)%4;
                    j++;
                    if (j >= 4) { break; }
                }
                if (branch.neighbors[direction] == null) {
                    GenerateRoom(branch, direction);
                }
            }
        } else if (rand == 1) { // Loop shape Dungeon
            Room branch = core;
            int direction = Random.Range(0, 4);
            // Create loop of rooms
            for (int i=0; i<4; i++) {
                int dir = (direction+i)%4;
                for (int j=0; j<3; j++) {
                    if (!GenerateRoom(branch, dir)) {
                        break; // Break if room creation fails
                    }
                    branch = branch.neighbors[dir];
                }
            }
            GenerateRoom(branch, (direction+3)%4);
            // Add Random Rooms
            for (int i=0; i<6; i++) {
                branch = rooms[Random.Range(0, rooms.Count)];
                direction = Random.Range(0, 4);
                int j = 0;
                while (branch.neighbors[direction] != null) {
                    direction = (direction+1)%4;
                    j++;
                    if (j >= 4) { break; }
                }
                if (branch.neighbors[direction] == null) {
                    GenerateRoom(branch, direction);
                }
            }
        }
        GenExits();
        core.Draw();
    }

    private bool GenerateRoom(Room parent, int direction) {
        int width, height;
        Vector3Int head = new Vector3Int();
        Vector3Int tail = new Vector3Int();
        bool valid;
        Room newRoom;
        int tries = 0;
        do {
            width = Mathf.RoundToInt(Mathf.Pow(Random.Range(1.4f, 2.6f), 2));
            height = Mathf.RoundToInt(Mathf.Pow(Random.Range(1.4f, 2.6f), 2));
            tries++;
            // Generate room to test
            if (direction == 0) {
                tail.x = Random.Range(parent.tail.x-width+1, parent.head.x+1);
                tail.y = parent.head.y+1;
                head.x = tail.x + width-1;
                head.y = tail.y + height-1;
            } else if (direction == 1) {
                tail.x = parent.head.x+1;
                tail.y = Random.Range(parent.tail.y-height+1, parent.head.y+1);
                head.x = tail.x + width-1;
                head.y = tail.y + height-1;
            } else if (direction == 2) {
                head.x = Random.Range(parent.tail.x, parent.head.x+width);
                head.y = parent.tail.y-1;
                tail.x = head.x - width+1;
                tail.y = head.y - height+1;
            } else if (direction == 3) {
                head.x = parent.tail.x-1;
                head.y = Random.Range(parent.tail.y, parent.head.y+height);
                tail.x = head.x - width+1;
                tail.y = head.y - height+1;
            }
            newRoom = new Room(head, tail);
            valid = true;
            foreach (Room r in rooms) {
                if (r.Collides(newRoom)) {
                    valid = false;
                    break;
                }
            }
            if (tries > 20) {
                Debug.Log("Room ("+newRoom.ToString()+") with width:"+width+" and height:"+height+" impossible to make");
                return false;
            }
        } while (!valid);
        newRoom.SetNeighbors(parent, rooms);
        rooms.Add(newRoom);
        parent.neighbors[direction] = rooms[rooms.Count-1];
        return true;
    }

    // Creates a floorspace, rooted at top corner
    public void DrawRoom(Room r) {
        int xLen = r.head.x - r.tail.x+1;
        int yLen = r.head.y - r.tail.y+1;
        Vector3Int placement = new Vector3Int(0,0,0);
        // Place floor
        for (int x=0; x>-xLen; x--) {
            for (int y=0; y>-yLen; y--) {
                placement.x = r.head.x + x;
                placement.y = r.head.y + y;
                Tile clone = null;
                clone = Instantiate(tiles["floor"]);
                int rand = Random.Range(0, 20);
                if (rand < 10) {
                    clone = Instantiate(tiles["floor"]);
                } else if (rand < 18) {
                    clone = Instantiate(tiles["floor1"]);
                } else if (rand == 18) {
                    clone = Instantiate(tiles["floor2"]);
                } else if (rand == 19) {
                    clone = Instantiate(tiles["floor3"]);
                }
                clone.name = clone.name.Split('(')[0];
                floorMap.SetTile(placement, clone);
            }
        }
        // Place Walls
        CreateWalls(r);
        // Create exits, if applicable
        foreach (KeyValuePair<string,Vector3Int> exit in notableCells) {
            if (r.Contains(exit.Value)) {
                floorMap.SetTile(exit.Value, null);
                blockMap.SetTile(exit.Value, tiles[exit.Key]);
            }
        }
        // Place Doors
        foreach (Room n in r.neighbors) {
            if (n != null && !n.active) {
                PlaceDoor(r, n);
            }
        }
        r.active = true;
        GenEnemies(r);
    }

    // A function for placing proper walls around a floor tile
    void CreateWalls(Room r) {
        Vector3Int cell;
        // Add four corners
        PlaceWall(r.head, "leftWall");
        PlaceWall(r.head, "rightWall");

        cell = new Vector3Int(r.head.x, r.tail.y, 0);
        PlaceWall(cell, "rightWall");
        cell = new Vector3Int(r.head.x, r.tail.y-1, 0);
        PlaceWall(cell, "leftWall", true);

        cell = new Vector3Int(r.tail.x, r.tail.y-1, 0);
        PlaceWall(cell, "leftWall", true);
        cell = new Vector3Int(r.tail.x-1, r.tail.y, 0);
        PlaceWall(cell, "rightWall", true);

        cell = new Vector3Int(r.tail.x, r.head.y, 0);
        PlaceWall(cell, "leftWall");
        cell = new Vector3Int(r.tail.x-1, r.head.y, 0);
        PlaceWall(cell, "rightWall", true);
        
        // Fill walls
        for (int x=r.head.x-1; x > r.tail.x; x--) {
            cell = new Vector3Int(x, r.head.y, 0);
            PlaceWall(cell, "leftWall");
            cell = new Vector3Int(x, r.tail.y-1, 0);
            PlaceWall(cell, "leftWall", true);
        }
        for (int y=r.head.y-1; y > r.tail.y; y--) {
            cell = new Vector3Int(r.head.x, y, 0);
            PlaceWall(cell, "rightWall");
            cell = new Vector3Int(r.tail.x-1, y, 0);
            PlaceWall(cell, "rightWall", true);
        }
    }

    void PlaceWall(Vector3Int cell, string name, bool clear=false) {
        // Determine Map
        Tilemap map;
        if (name.ToLower().IndexOf("left") >= 0) {
            map = leftWallMap;
        } else {
            map = rightWallMap;
        }
        TileBase currentWall = map.GetTile(cell);
        string currentName = "";
        if (currentWall != null) { // Wall in-place, clarify it
            currentName = currentWall.name;
            if (currentName.ToLower().IndexOf("clear") < 0) {
                Tile clone = Instantiate(clearTiles[currentName]);
                map.SetTile(cell, clone);
                clone.name = clone.name.Split('(')[0];
            }
        } else { // Place wall normally
            Tile clone;
            if (!clear) {
                clone = Instantiate(tiles[name]);
                map.SetTile(cell, clone);
            } else {
                clone = Instantiate(clearTiles[name]);
                map.SetTile(cell, clone);
            }
            clone.name = clone.name.Split('(')[0];
        }
    }

    void PlaceDoor(Room r1, Room r2) {
        Vector3Int cell = new Vector3Int();
        if (r1.head.y == r2.tail.y-1) { // New room is above
            // Cycle through possible door placments
            cell.y = r1.head.y;
            int x = Mathf.RoundToInt(Random.Range(0, r1.width));
            for (int i=0; i<r1.width; i++) {
                cell.x = r1.tail.x + x;
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x,cell.y+1,cell.z))
                && !notableCells.ContainsValue(cell)) {
                    leftWallMap.SetTile(cell, tiles["leftDoor"]);
                    break;
                }
                x = (x+1) % r1.width;
            }
        } else if (r1.head.x == r2.tail.x-1) { // New room is right
            // Cycle through possible door placments
            cell.x = r1.head.x;
            int y = Mathf.RoundToInt(Random.Range(0, r1.height));
            for (int i=0; i<r1.height; i++) {
                cell.y = r1.tail.y + y;
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x+1,cell.y,cell.z))
                && !notableCells.ContainsValue(cell)) {
                    rightWallMap.SetTile(cell, tiles["rightDoor"]);
                    break;
                }
                y = (y+1) % r1.height;
            }
        } else if (r1.tail.y == r2.head.y+1) { // New room is below
            // Cycle through possible door placments
            cell.y = r1.tail.y;
            int x = Mathf.RoundToInt(Random.Range(0, r1.width));
            for (int i=0; i<r1.width; i++) {
                cell.x = r1.tail.x + x;
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x,cell.y-1,cell.z))
                && !notableCells.ContainsValue(cell)) {
                    leftWallMap.SetTile(new Vector3Int(cell.x,cell.y-1,cell.z), clearTiles["leftDoor"]);
                    break;
                }
                x = (x+1) % r1.width;
            }
        } else if (r1.tail.x == r2.head.x+1) { // New room is left
            // Cycle through possible door placments
            cell.x = r1.tail.x;
            int y = Mathf.RoundToInt(Random.Range(0, r1.height));
            for (int i=0; i<r1.height; i++) {
                cell.y = r1.tail.y + y;
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x-1,cell.y,cell.z))
                && !notableCells.ContainsValue(cell)) {
                    rightWallMap.SetTile(new Vector3Int(cell.x-1,cell.y,cell.z), clearTiles["rightDoor"]);
                    break;
                }
                y = (y+1) % r1.height;
            }
        }
    }

    void GenExits() {
        string entrance = "stairsUp", exit = "stairsDown";
        if (data.direction == "up") {
            entrance = "stairsDown";
            exit = "stairsUp";
        }
        notableCells.Add(entrance, new Vector3Int(0,1,0));
        int rand = Random.Range(1, rooms.Count);
        int i = 0;
        foreach(Room r in rooms) {
            if (i == rand) {
                Vector3Int cell = new Vector3Int(
                    r.tail.x+Random.Range(1,r.width-1),
                    r.tail.y+Random.Range(1,r.height-1),
                    0);
                notableCells.Add(exit, cell);
                break;
            }
            i++;
        }
    }

    public void OpenDoor(Vector3Int cell, int dir) {
        Tile clone;
        if (dir == 0 || dir == 2) {
            clone = Instantiate(tiles["leftDoorOpen"]);
            leftWallMap.SetTile(cell, clone);
            if (dir == 0) { cell.y++; }
        } else {
            clone = Instantiate(tiles["rightDoorOpen"]);
            rightWallMap.SetTile(cell, clone);
            if (dir == 1) { cell.x++; }
        }
        clone.name = clone.name.Split('(')[0];
        foreach (Room r in rooms) {
            if (r.Contains(cell) && !r.active) {
                r.Draw();
            }
        }
    }

    // Give a room its list of enemies
    public void RetrieveEnemies(Room r) {
        int numEnemies = Mathf.RoundToInt(Mathf.Pow(Random.Range(1f,2f), 2));
        float wheelTotal = 0f;
        foreach (KeyValuePair<string,int> e in enemyWheel) {
            wheelTotal += e.Value;
        }
        for (int _=0; _<numEnemies; _++) {
            int total = 0;
            float roll = Random.Range(0, wheelTotal);
            foreach (KeyValuePair<string,int> e in enemyWheel) {
                total += e.Value;
                if (roll <= total) {
                    r.enemies.Add(e.Key);
                    break;
                }
            }
        }
    }

    public void GenEnemies(Room r) {
        // Generate spawn points
        List<Vector3Int> vectors = new List<Vector3Int>();
        // Add player and notables as occupied
        vectors.Add(player.tilePosition);
        foreach (KeyValuePair<string,Vector3Int> v in notableCells) {
            vectors.Add(v.Value);
        }
        for (int i=0; i<r.enemies.Count; i++) {
            Vector3Int v;
            bool valid;
            int sentinel = 0;
            do {
                valid = true;
                v = new Vector3Int(
                    Random.Range(r.tail.x, r.head.x+1),
                    Random.Range(r.tail.y, r.head.y+1),
                    0);
                foreach (Vector3Int v2 in vectors) {
                    if (v == v2) {
                        valid = false;
                        break;
                    }
                }
                sentinel++;
                if (sentinel > 100) {break;}
            } while (!valid);
            vectors.Add(v);
        }
        // Remove player and notables from spawn options
        vectors.RemoveRange(0, notableCells.Count+1);
        int ei = 0;
        foreach (string e in r.enemies) {
            Vector3 pos = floorMap.CellToWorld(vectors[ei]);
            pos.y += 0.25f;
            pos.z = 0;
            GameObject clone = Instantiate(
                enemyFabs[e],
                pos,
                Quaternion.identity,
                enemies.transform);
            clone.name = clone.name.Split('(')[0];
            clone.GetComponent<EnemyBehavior>().SetCoords(vectors[ei]);
            ei++;
        }
    }
}
