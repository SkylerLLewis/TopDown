using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Initializer : MonoBehaviour
{
    Tile floor, leftWall, rightWall, leftRightWall, leftLowerWall, rightLowerWall, leftRightLowerWall, leftDoor, leftDoorOpen;
    Tile leftClearWall, rightClearWall, leftRightClearWall;
    private Tilemap dungeonMap, wallMap;
    Vector3Int cellLoc;
    private GameObject enemies, player;
    private Room[] rooms;
    
    void Awake() {
        player = GameObject.FindWithTag("Player");

        // Load tile resources
        floor = Resources.Load<Tile>("DungeonMap/floor");
        leftWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftWall");
        rightWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightWall");
        leftRightWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightWall");
        leftLowerWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftLowerWall");
        rightLowerWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightLowerWall");
        leftRightLowerWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightLowerWall");
        leftDoor = Resources.Load<Tile>("DungeonMap/WallPalette/leftDoor");
        leftDoorOpen = Resources.Load<Tile>("DungeonMap/WallPalette/leftDoorOpen");
        leftClearWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftClearWall");
        rightClearWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightClearWall");
        leftRightClearWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightClearWall");


        cellLoc = new Vector3Int(-1, 1, 0);
        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
            } else if (map.name == "WallMap") {
                wallMap = map;
            }

        }
        dungeonMap.SetTile(cellLoc, floor);
        Debug.Log(string.Format("Success! {0}", cellLoc.ToString()));

        // Create enemy from prefab
        enemies = GameObject.FindWithTag("EntityList");
        GameObject gobFab = Resources.Load("Prefabs/Goblin") as GameObject;
        GameObject gobbo = Instantiate(gobFab, new Vector3(0,-1.75f,0), Quaternion.identity, enemies.transform);
        gobbo.name = gobbo.name.Split('(')[0];

        // Create core room
        Room core = new Room(new Vector3Int(1,1,0), new Vector3Int(-3,-3,0));
        // Create Neighbor
        Room branch = new Room(new Vector3Int(2,4,0), new Vector3Int(-1,2,0), core, 2);
        core.neighbors[0] = branch;
        core.Draw();
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
                Tile clone = Instantiate(floor);
                clone.name = clone.name.Split('(')[0];
                dungeonMap.SetTile(placement, clone);
            }
        }
        CreateWalls(r);
        /*foreach (Room n in r.neighbors) {
            if (n != null && !n.active) {
                PlaceDoor(r, n);
            }
        }*/
        r.active = true;
    }

    // A function for placing proper walls around a floor tile
    void CreateWalls(Room r) {
        Vector3Int cell;
        // Add four corners
        ReplaceWall(r.head, leftRightWall, leftRightClearWall, wallMap.GetTile(r.head), "leftRightWall");

        cell = new Vector3Int(r.head.x, r.tail.y, 0);
        ReplaceWall(cell, rightWall, rightClearWall, wallMap.GetTile(cell), "rightWall");
        cell = new Vector3Int(r.head.x, r.tail.y-1, 0);
        PlaceWall(cell, leftClearWall);

        cell = new Vector3Int(r.tail.x, r.tail.y-1, 0);
        PlaceWall(cell, leftClearWall);
        cell = new Vector3Int(r.tail.x-1, r.tail.y, 0);
        PlaceWall(cell, rightClearWall);

        cell = new Vector3Int(r.tail.x, r.head.y, 0);
        ReplaceWall(cell, leftWall, leftClearWall, wallMap.GetTile(cell), "leftWall");
        cell = new Vector3Int(r.tail.x-1, r.head.y, 0);
        PlaceWall(cell, rightClearWall);

        for (int x=r.head.x-1; x > r.tail.x; x--) {
            cell = new Vector3Int(x, r.head.y, 0);
            ReplaceWall(cell, leftWall, leftClearWall, wallMap.GetTile(cell), "leftWall");
            cell = new Vector3Int(x, r.tail.y-1, 0);
            PlaceWall(cell, leftClearWall);
        }
        for (int y=r.head.y-1; y > r.tail.y; y--) {
            cell = new Vector3Int(r.head.x, y, 0);
            ReplaceWall(cell, rightWall, rightClearWall, wallMap.GetTile(cell), "rightWall");
            cell = new Vector3Int(r.tail.x-1, y, 0);
            PlaceWall(cell, rightClearWall);
        }


        /* TileBase adj = dungeonMap.GetTile(new Vector3Int(cell.x, cell.y+1, cell.z));
        TileBase wall;
        
        bool top = (adj != null && adj.name == "floor");
        adj = dungeonMap.GetTile(new Vector3Int(cell.x, cell.y-1, cell.z));
        bool bottom = (adj != null && adj.name == "floor");
        adj = dungeonMap.GetTile(new Vector3Int(cell.x+1, cell.y, cell.z));
        bool right = (adj != null && adj.name == "floor");
        adj = dungeonMap.GetTile(new Vector3Int(cell.x-1, cell.y, cell.z));
        bool left = (adj != null && adj.name == "floor");
        //Debug.Log("Top: "+top+" Bottom: "+bottom+" Right: "+right+" Left: "+left);

        Tile clone = null;
        // Upper wall placement
        wall = wallMap.GetTile(cell);
        if (!top && !right) {
            clone = Instantiate(leftRightWall);
            wallMap.SetTile(cell, clone);
        } else if (!top) {
            clone = Instantiate(leftWall);
            wallMap.SetTile(cell, clone);
        } else if (!right) {
            clone = Instantiate(rightWall);
            wallMap.SetTile(cell, clone);
        }
        // Lower wall placement
        if (!bottom && !left) {
            clone = Instantiate(leftRightLowerWall);
            wallMap.SetTile(cell, clone);
        } else if (!left) {
            clone = Instantiate(leftLowerWall);
            wallMap.SetTile(cell, clone);
        } else if (!bottom) {
            clone = Instantiate(rightLowerWall);
            wallMap.SetTile(cell, clone);
        }
        if (clone != null) {
            clone.name = clone.name.Split('(')[0];
        }*/
    }
    void ReplaceWall(Vector3Int cell, Tile wall, Tile clearWall, TileBase currentWall, string name) {
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
    }

    void PlaceDoor(Room r1, Room r2) {

    }

    public void OpenDoor(Vector3Int cell) {
        TileBase door = wallMap.GetTile(cell);
        if (door.name.ToLower().IndexOf("left") >= 0) {
            wallMap.SetTile(cell, Instantiate(leftDoorOpen));
        }
    }
}
