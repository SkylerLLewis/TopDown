using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Initializer : MonoBehaviour
{
    Tile floor;
    Tile leftWall;
    Tile rightWall;
    Tile leftRightWall;
    Tile leftLowerWall;
    Tile rightLowerWall;
    Tile leftRightLowerWall;
    private Tilemap dungeonMap;
    private Tilemap wallMap;
    private Tilemap lowerWallMap;
    Vector3Int cellLoc;
    private GameObject enemies;

    private class Room {
        private Vector3Int head, tail;
        private int width, height;
        Room() {

        }

        public bool Contains(Vector3Int cell) {
            return (cell.x <= head.x && cell.x >= tail.x &&
                    cell.y <= head.y && cell.y >= tail.y);
        }

        /*public bool Collides(in Room r) {
            bool collide;
            if (r.head.x > this.head.x && r.head.y > this.head.y) {
                collide = Abs(r.head.x-this.head.x) < r.width || Abs(r.head.y-this.head.y) < r.height;
            } else if (r.head.x < this.head.x) {
                wide = Abs(r.head.x-this.head.x) < this.width;
            } else if (r.tail.x > this.head.x) {
                return (Abs(r.head.x-this.head.x) < this.width);
            }
            return collide;
        }*/
    }
    
    void Awake() {
        // Load tile resources
        floor = Resources.Load<Tile>("DungeonMap/floor");
        leftWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftWall");
        rightWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightWall");
        leftRightWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightWall");
        leftLowerWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftLowerWall");
        rightLowerWall = Resources.Load<Tile>("DungeonMap/WallPalette/rightLowerWall");
        leftRightLowerWall = Resources.Load<Tile>("DungeonMap/WallPalette/leftRightLowerWall");


        cellLoc = new Vector3Int(-1, 1, 0);
        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
            } else if (map.name == "WallMap") {
                wallMap = map;
            } else if (map.name == "LowerWallMap") {
                lowerWallMap = map;
            }

        }
        dungeonMap.SetTile(cellLoc, floor);
        Debug.Log(string.Format("Success! {0}", cellLoc.ToString()));

        // Create 4x4 floorspace rooted at (-1, -6)
        createRoom(new Vector3Int(-1,-6,0), new Vector3Int(-4,-9,0));
        /*Vector3Int roomBase = new Vector3Int(-1,-6,0);
        Vector3Int placement = new Vector3Int(0,0,0);
        for (int x=0; x>-4; x--) {
            for (int y=0; y>-4; y--) {
                placement.x = roomBase.x + x;
                placement.y = roomBase.y + y;
                dungeonMap.SetTile(placement, Instantiate(floor));
                if (Random.value>0.5) {
                    TileBase t = dungeonMap.GetTile(placement);
                }
            }
        }*/

        // Create Enemy
        /*GameObject Goblin = new GameObject("Goblin", typeof(Rigidbody2D), typeof(SpriteRenderer));
        Goblin.transform.position = new Vector3(0, -0.25f, 0);
        Rigidbody2D GobBody = Goblin.GetComponent<Rigidbody2D>();
        GobBody.gravityScale = 0;
        SpriteRenderer GobSprite = Goblin.GetComponent<SpriteRenderer>();
        GobSprite.sprite = Resources.Load<Sprite>("Goblin");
        */
        enemies = GameObject.FindWithTag("EntityList");
        Object gobFab = Resources.Load("Prefabs/Goblin");
        Instantiate(gobFab, new Vector3(0,-1.25f,0), Quaternion.identity, enemies.transform);
    }

    // Creates a floorspace, rooted at top corner
    void createRoom(Vector3Int head, Vector3Int tail) {
        int xLen = head.x - tail.x+1;
        int yLen = head.y - tail.y+1;
        Vector3Int placement = new Vector3Int(0,0,0);
        for (int x=0; x>-xLen; x--) {
            for (int y=0; y>-yLen; y--) {
                placement.x = head.x + x;
                placement.y = head.y + y;
                Tile clone = Instantiate(floor);
                clone.name = "floor";
                dungeonMap.SetTile(placement, clone);
            }
        }
        createWalls(tail);
        for (int x=0; x>-xLen; x--) {
            for (int y=0; y>-yLen; y--) {
                placement.x = head.x + x;
                placement.y = head.y + y;
                createWalls(placement);
            }
        }
    }

    // A function for placing proper walls around a floor tile
    void createWalls(Vector3Int cell) {
        TileBase adj = dungeonMap.GetTile(new Vector3Int(cell.x, cell.y+1, cell.z));
        
        bool top = (adj != null && adj.name == "floor");
        adj = dungeonMap.GetTile(new Vector3Int(cell.x, cell.y-1, cell.z));
        bool bottom = (adj != null && adj.name == "floor");
        adj = dungeonMap.GetTile(new Vector3Int(cell.x+1, cell.y, cell.z));
        bool right = (adj != null && adj.name == "floor");
        adj = dungeonMap.GetTile(new Vector3Int(cell.x-1, cell.y, cell.z));
        bool left = (adj != null && adj.name == "floor");
        //Debug.Log("Top: "+top+" Bottom: "+bottom+" Right: "+right+" Left: "+left);

        // Upper wall placement
        if (!top && !right) {
            wallMap.SetTile(cell, Instantiate(leftRightWall));
        } else if (!top) {
            wallMap.SetTile(cell, Instantiate(leftWall));
        } else if (!right) {
            wallMap.SetTile(cell, Instantiate(rightWall));
        }
        // Lower wall placement
        if (!bottom && !left) {
            lowerWallMap.SetTile(cell, Instantiate(leftRightLowerWall));
        } else if (!left) {
            lowerWallMap.SetTile(cell, Instantiate(leftLowerWall));
        } else if (!bottom) {
            lowerWallMap.SetTile(cell, Instantiate(rightLowerWall));
        }
    }
}
