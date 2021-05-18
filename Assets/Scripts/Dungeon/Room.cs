using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public Vector3Int head, tail, center;
    public int width, height;
    public Room[] neighbors;
    public bool active;
    Tilemap floorMap;
    Initializer mapController;
    public List<string> enemies;

    public Room(Vector3Int h, Vector3Int t, Room parent=null, int dir=0) {
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
        neighbors[dir] = parent;
        enemies = new List<string>();
        mapController.RetrieveEnemies(this);
    }

    public bool Contains(Vector3Int cell) {
        return (cell.x <= head.x && cell.x >= tail.x &&
                cell.y <= head.y && cell.y >= tail.y);
    }

    // Draws the room into existence
    public void Draw() {
        mapController.DrawRoom(this);
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
            if (r.head.x <= this.tail.x && r.tail.y <= this.head.y) {
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

    
    public override string ToString() {
        string s = "Room: ";
        s += head.ToString() + ", ";
        s += tail.ToString();
        return s;
    }
}
