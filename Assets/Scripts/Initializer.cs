using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Initializer : MonoBehaviour
{
    Tile floor, leftWall, rightWall, leftRightWall, leftDoor, leftDoorOpen;
    Tile leftClearWall, rightClearWall, leftRightClearWall;
    Dictionary<string,Tile> tiles, clearTiles;
    private Tilemap dungeonMap, leftWallMap, rightWallMap;
    Vector3Int cellLoc;
    private GameObject enemies, player;
    private List<Room> rooms;
    
    void Awake() {
        player = GameObject.FindWithTag("Player");

        // Load tile resources
        /*floor = Resources.Load<Tile>("DungeonMap/floor");
        leftWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftWall");
        rightWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightWall");
        leftRightWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightWall");
        leftDoor = Resources.Load<Tile>("DungeonMap/WallPalette/leftDoor");
        leftDoorOpen = Resources.Load<Tile>("DungeonMap/WallPalette/leftDoorOpen");
        leftClearWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftClearWall");
        rightClearWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightClearWall");
        leftRightClearWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightClearWall");
        */
        // Load tile Resources into dictionaries
        tiles = new Dictionary<string,Tile>();
        tiles.Add("floor", Resources.Load<Tile>("DungeonMap/floor"));
        tiles.Add("leftWall", Resources.Load<Tile>("DungeonMap/WallPalette/leftWall"));
        tiles.Add("rightWall", Resources.Load<Tile>("DungeonMap/WallPalette/rightWall"));
        tiles.Add("leftDoor", Resources.Load<Tile>("DungeonMap/WallPalette/leftDoor"));
        tiles.Add("leftDoorOpen", Resources.Load<Tile>("DungeonMap/WallPalette/leftDoorOpen"));
        Debug.Log("Tiles: "+tiles.ToString());

        clearTiles = new Dictionary<string,Tile>();
        clearTiles.Add("leftWall", Resources.Load<Tile>("DungeonMap/WallPalette/leftClearWall"));
        clearTiles.Add("rightWall", Resources.Load<Tile>("DungeonMap/WallPalette/rightClearWall"));


        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
            } else if (map.name == "LeftWallMap") {
                leftWallMap = map;
            } else if (map.name == "RightWallMap") {
                rightWallMap = map;
            }

        }

        // Create enemy from prefab
        enemies = GameObject.FindWithTag("EntityList");
        GameObject gobFab = Resources.Load("Prefabs/Goblin") as GameObject;
        GameObject gobbo = Instantiate(gobFab, new Vector3(0,-1.75f,0), Quaternion.identity, enemies.transform);
        gobbo.name = gobbo.name.Split('(')[0];

        // Create core room
        rooms = new List<Room>();
        Room core = new Room(new Vector3Int(1,1,0), new Vector3Int(-3,-3,0));
        rooms.Add(core);
        // Create Neighbor
        Room branch = new Room(new Vector3Int(2,4,0), new Vector3Int(-1,2,0), core, 2);
        core.neighbors[0] = branch;
        rooms.Add(branch);
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
                dungeonMap.SetTile(placement, clone);
            }
        }
        CreateWalls(r);
        foreach (Room n in r.neighbors) {
            if (n != null && !n.active) {
                PlaceDoor(r, n);
            }
        }
        r.active = true;
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

    /*void ReplaceWall(Vector3Int cell, Tile wall, Tile clearWall, TileBase currentWall, string name) {
        Tile clone;
        string currentName = "";
        if (currentWall != null) {
            currentName = currentWall.name;
        }
        if (name != currentName) {
            clone = Instantiate(wall);
            wallMap.SetTile(cell, clone);
        } else {
            clone = Instantiate(clearWall);
            wallMap.SetTile(cell, clone);
        }
        clone.name = clone.name.Split('(')[0];
    }
    void PlaceWall(Vector3Int cell, Tile wall) {
        Tile clone = Instantiate(wall);
        wallMap.SetTile(cell, clone);
        clone.name = clone.name.Split('(')[0];
    }*/
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
}
