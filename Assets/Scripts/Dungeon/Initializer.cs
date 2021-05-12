using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Initializer : MonoBehaviour
{
    Tile floor, leftWall, rightWall, leftRightWall, leftDoor, leftDoorOpen;
    Tile leftClearWall, rightClearWall, leftRightClearWall;
    Dictionary<string,Tile> tiles, clearTiles;
    private Tilemap floorMap, leftWallMap, rightWallMap;
    Vector3Int cellLoc;
    private GameObject enemies;
    private PlayerController player;
    private Dictionary<string, GameObject> enemyFabs;
    private Dictionary<string, int> enemyWheel;
    private List<Room> rooms;
    
    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        enemies = GameObject.FindWithTag("EntityList");

        // Load tile resources
        /*floor = Resources.Load<Tile>("DungeonMap/floor");
        leftWall = Resources.Load<Tile>("Tiles/DungeonMap/leftWall");
        rightWall = Resources.Load<Tile>("Tiles/DungeonMap/rightWall");
        leftRightWall = Resources.Load<Tile>("Tiles/DungeonMap/leftRightWall");
        leftDoor = Resources.Load<Tile>("Tiles/DungeonMap/leftDoor");
        leftDoorOpen = Resources.Load<Tile>("Tiles/DungeonMap/leftDoorOpen");
        leftClearWall = Resources.Load<Tile>("Tiles/DungeonMap/leftClearWall");
        rightClearWall = Resources.Load<Tile>("Tiles/DungeonMap/rightClearWall");
        leftRightClearWall = Resources.Load<Tile>("Tiles/DungeonMap/leftRightClearWall");
        */
        // Load tile Resources into dictionaries
        tiles = new Dictionary<string,Tile>();
        tiles.Add("floor", Resources.Load<Tile>("Tiles/DungeonMap/floor"));
        tiles.Add("leftWall", Resources.Load<Tile>("Tiles/DungeonMap/leftWall"));
        tiles.Add("rightWall", Resources.Load<Tile>("Tiles/DungeonMap/rightWall"));
        tiles.Add("leftDoor", Resources.Load<Tile>("Tiles/DungeonMap/leftDoor"));
        tiles.Add("leftDoorOpen", Resources.Load<Tile>("Tiles/DungeonMap/leftDoorOpen"));
        tiles.Add("rightDoor", Resources.Load<Tile>("Tiles/DungeonMap/rightDoor"));
        tiles.Add("rightDoorOpen", Resources.Load<Tile>("Tiles/DungeonMap/rightDoorOpen"));

        clearTiles = new Dictionary<string,Tile>();
        clearTiles.Add("leftWall", Resources.Load<Tile>("Tiles/DungeonMap/leftClearWall"));
        clearTiles.Add("rightWall", Resources.Load<Tile>("Tiles/DungeonMap/rightClearWall"));

        // Load Enemy prefabs and likelyhood
        enemyFabs = new Dictionary<string, GameObject>();
        enemyWheel = new Dictionary<string, int>();
        enemyFabs.Add("Goblin", Resources.Load("Prefabs/Goblin") as GameObject);
        enemyWheel.Add("Goblin", 2);
        enemyFabs.Add("Hobgoblin", Resources.Load("Prefabs/Hobgoblin") as GameObject);
        enemyWheel.Add("Hobgoblin", 1);
        enemyFabs.Add("Spider", Resources.Load("Prefabs/Spider") as GameObject);
        enemyWheel.Add("Spider", 1);


        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "FloorMap") {
                floorMap = map;
            } else if (map.name == "LeftWallMap") {
                leftWallMap = map;
            } else if (map.name == "RightWallMap") {
                rightWallMap = map;
            }

        }

        // Create core room
        rooms = new List<Room>();
        Room core = new Room(new Vector3Int(1,1,0), new Vector3Int(-3,-3,0));
        rooms.Add(core);
        // Create 4 Neighbors
        rooms.Add(new Room(new Vector3Int(2,4,0), new Vector3Int(-1,2,0), core, 2));
        core.neighbors[0] = rooms[1];
        rooms.Add(new Room(new Vector3Int(5,1,0), new Vector3Int(2,-2,0), core, 3));
        core.neighbors[1] = rooms[2];
        rooms.Add(new Room(new Vector3Int(-1,-4,0), new Vector3Int(-3,-6,0), core, 0));
        core.neighbors[2] = rooms[3];
        rooms.Add(new Room(new Vector3Int(-4,1,0), new Vector3Int(-8,-4,0), core, 1));
        core.neighbors[3] = rooms[4];
        core.Draw();
        //branch.Draw();
    }

    // Creates a map of rooms 
    void CreateRooms() {
    }

    // Creates a floorspace, rooted at top corner
    public void DrawRoom(Room r) {
        int xLen = r.head.x - r.tail.x+1;
        int yLen = r.head.y - r.tail.y+1;
        Vector3Int placement = new Vector3Int(0,0,0);
        for (int x=0; x>-xLen; x--) {
            for (int y=0; y>-yLen; y--) {
                placement.x = r.head.x + x;
                placement.y = r.head.y + y;
                Tile clone = Instantiate(tiles["floor"]);
                clone.name = clone.name.Split('(')[0];
                floorMap.SetTile(placement, clone);
            }
        }
        CreateWalls(r);
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
            if (currentName.ToLower().IndexOf("clear") < 0 && currentName.ToLower().IndexOf("door") < 0) { // No clear or door tiles
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
        Debug.Log("Placing Door!");
        Vector3Int cell = new Vector3Int();
        if (r1.head.y == r2.tail.y-1) { // New room is above
            Debug.Log("Head: "+r1.head.ToString()+" to Tail: "+r2.tail.ToString());
            // Cycle through possible door placments
            cell.y = r1.head.y;
            int x = Mathf.RoundToInt(Random.Range(0, r1.width));
            for (int i=0; i<r1.width; i++) {
                cell.x = r1.tail.x + x;
                Debug.Log("Trying Cell: "+cell.ToString());
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x,cell.y+1,cell.z))) {
                    leftWallMap.SetTile(cell, tiles["leftDoor"]);
                    break;
                }
                x = (x+1) % r1.width;
            }
        } else if (r1.head.x == r2.tail.x-1) { // New room is right
            Debug.Log("Head: "+r1.head.ToString()+" to Tail: "+r2.tail.ToString());
            // Cycle through possible door placments
            cell.x = r1.head.x;
            int y = Mathf.RoundToInt(Random.Range(0, r1.height));
            for (int i=0; i<r1.height; i++) {
                cell.y = r1.tail.y + y;
                Debug.Log("Trying Cell: "+cell.ToString());
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x+1,cell.y,cell.z))) {
                    rightWallMap.SetTile(cell, tiles["rightDoor"]);
                    break;
                }
                y = (y+1) % r1.height;
            }
        } else if (r1.tail.y == r2.head.y+1) { // New room is below
            Debug.Log("Tail: "+r1.tail.ToString()+" to Head: "+r2.head.ToString());
            // Cycle through possible door placments
            cell.y = r1.tail.y;
            int x = Mathf.RoundToInt(Random.Range(0, r1.width));
            for (int i=0; i<r1.width; i++) {
                cell.x = r1.tail.x + x;
                Debug.Log("Trying Cell: "+cell.ToString());
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x,cell.y-1,cell.z))) {
                    leftWallMap.SetTile(new Vector3Int(cell.x,cell.y-1,cell.z), tiles["leftDoor"]);
                    break;
                }
                x = (x+1) % r1.width;
            }
        } else if (r1.tail.x == r2.head.x+1) { // New room is left
            Debug.Log("Tail: "+r1.tail.ToString()+" to Head: "+r2.head.ToString());
            // Cycle through possible door placments
            cell.x = r1.tail.x;
            int y = Mathf.RoundToInt(Random.Range(0, r1.height));
            for (int i=0; i<r1.height; i++) {
                cell.y = r1.tail.y + y;
                Debug.Log("Trying Cell: "+cell.ToString());
                if (r1.Contains(cell) && r2.Contains(new Vector3Int(cell.x-1,cell.y,cell.z))) {
                    rightWallMap.SetTile(new Vector3Int(cell.x-1,cell.y,cell.z), tiles["rightDoor"]);
                    break;
                }
                y = (y+1) % r1.height;
            }
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
        string o = "Enemies generated: ";
        foreach (string s in r.enemies) {
            o += s + " ";
        }
        Debug.Log(o);
    }

    public void GenEnemies(Room r) {
        // Generate spawn points
        List<Vector3Int> vectors = new List<Vector3Int>();
        vectors.Add(player.tilePosition);
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
        vectors.RemoveAt(0);
        int ei = 0;
        foreach (string e in r.enemies) {
            Vector3 pos = floorMap.CellToWorld(vectors[ei]);
            pos.y += 0.25f;
            GameObject clone = Instantiate(
                enemyFabs[e],
                pos,
                Quaternion.identity,
                enemies.transform);
            clone.name = clone.name.Split('(')[0];
            ei++;
        }
    }
}
