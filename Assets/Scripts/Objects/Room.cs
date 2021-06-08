using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public Vector3Int head, tail, center;
    public int width, height, loot;
    public Room[] neighbors;
    public bool active;
    Tilemap floorMap;
    Initializer mapController;
    public List<string> enemies;

    public Room(Vector3Int h, Vector3Int t, Room parent=null, List<Room> rooms=null) {
        Grid grid = GameObject.FindObjectOfType<Grid>();
        foreach (Tilemap map in GameObject.FindObjectsOfType<Tilemap>()) {
            if (map.name == "FloorMap") {
                floorMap = map;
                break;
            }
        }
        mapController = floorMap.GetComponent<Initializer>();
        active = false;
        head = h;
        tail = t;
        width = head.x - tail.x + 1;
        height = head.y - tail.y + 1;
        center = (head + tail) / 2;
        neighbors = new Room[4];
        if (parent != null) {
            SetNeighbors(parent, rooms);
        }
        enemies = new List<string>();
        mapController.RetrieveEnemies(this);
        loot = 0;
    }

    // Draws the room into existence
    public void Draw() {
        active = true;
        mapController.DrawRoom(this);
    }

    public void SetNeighbors(Room parent, List<Room> rooms) {
        int dir = Neighboring(parent);
        neighbors[dir] = parent;
        parent.neighbors[(dir+2)%4] = this;
        foreach (Room r in rooms) {
            dir = Neighboring(r);
            if (dir != -1 && neighbors[dir] == null && r.neighbors[(dir+2)%4] == null) {
                Debug.Log("Extra neighbor added at "+ToString()+" "+dir);
                neighbors[dir] = r;
                r.neighbors[(dir+2)%4] = this;
            }
        }
    }

    public bool Contains(Vector3Int cell) {
        return (cell.x <= head.x && cell.x >= tail.x &&
                cell.y <= head.y && cell.y >= tail.y);
    }

    public int Neighboring(in Room r) {
        if (this.head.y == r.tail.y-1 && 
        (Mathf.Abs(this.head.x-r.head.x) <= this.width || Mathf.Abs(this.head.x-r.head.x) <= r.width-1)) {
            return 0;
        } else if (this.head.x == r.tail.x-1 && 
        (Mathf.Abs(this.head.y-r.head.y) <= this.height || Mathf.Abs(this.head.y-r.head.y) <= r.height-1)) {
            return 1;
        } else if (this.tail.y == r.head.y+1 && 
        (Mathf.Abs(this.head.x-r.head.x) <= this.width || Mathf.Abs(this.head.x-r.head.x) <= r.width-1)) {
            return 2;
        } else if (this.tail.x == r.head.x+1 && 
        (Mathf.Abs(this.head.y-r.head.y) <= this.height || Mathf.Abs(this.head.y-r.head.y) <= r.height-1)) {
            return 3;
        }
        return -1;
    }

    public bool Collides(in Room r) {
        bool collide = false;
        if (r.head.x > this.head.x && r.head.y > this.head.y) { // top right
            if (r.tail.x <= this.head.x && r.tail.y <= this.head.y) {
                collide = true;
            } else {
                collide = false;
            }
        } else if (r.head.x > this.head.x && r.tail.y < this.tail.y) { // bottom right
            if (r.tail.x <= this.head.x && r.head.y >= this.tail.y) {
                collide = true;
            } else {
                collide = false;
            }
        } else if (r.tail.x < this.tail.x && r.tail.y < this.tail.y) { // bottom left
            if (r.head.x >= this.tail.x && r.head.y >= this.tail.y) {
                collide = true;
            } else {
                collide = false;
            }
        } else if (r.tail.x < this.tail.x && r.head.y > this.head.y) { // top left
            if (r.head.x >= this.tail.x && r.tail.y <= this.head.y) {
                collide = true;
            } else {
                collide = false;
            }
        } else { // Contains or parallel with
            if (r.tail.x > this.head.x || r.tail.y > this.head.y || r.head.y < this.tail.y || r.head.x < this.tail.x) {
                collide = false;
            } else {
                collide = true;
            }
        }
        return collide;
    }

    public static Room FindByCell(Vector3Int cell, List<Room> rooms) {
        foreach (Room r in rooms) {
            if (r.Contains(cell)) {
                return r;
            }
        }
        Debug.LogWarning("Room not found containing cell: "+cell);
        return null;
    }

    
    public override string ToString() {
        string s = "Room: ";
        s += head.ToString() + ", ";
        s += tail.ToString();
        return s;
    }
}
