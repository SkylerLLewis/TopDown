using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public Vector3Int head, tail;
    public int width, height;
    public Room[] neighbors;
    public bool active;
    Tilemap dungeonMap, wallMap;
    Initializer mapController;

    public Room(Vector3Int h, Vector3Int t, Room parent=null, int dir=0) {
        Grid grid = GameObject.FindObjectOfType<Grid>();
        foreach (Tilemap map in GameObject.FindObjectsOfType<Tilemap>()) {
            if (map.name == "DungeonMap") {
                dungeonMap = map;
            } else if (map.name == "WallMap") {
                wallMap = map;
            }
        }
        mapController = dungeonMap.GetComponent<Initializer>();
        active = false;
        head = h;
        tail = t;
        neighbors = new Room[4];
        neighbors[dir] = parent;
    }

    public bool Contains(Vector3Int cell) {
        return (cell.x <= head.x && cell.x >= tail.x &&
                cell.y <= head.y && cell.y >= tail.y);
    }

    // Draws the room into existence
    public void Draw() {
        Debug.Log("Mapcontroller: "+mapController.ToString());
        mapController.DrawRoom(this);
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
