using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public Vector3Int head, tail;
    public int width, height, loot;
    public Vector3 center;
    public Room[] neighbors;
    public Vector3Int[] doors;
    public bool[] doorsOpen;
    public bool active;
    Tilemap floorMap;
    Initializer mapController;
    public List<string> enemies;
    public string prefab;

    public Room(Vector3Int h, Vector3Int t, Room parent=null, List<Room> rooms=null, string _prefab="") {
        Grid grid = GameObject.FindObjectOfType<Grid>();
        foreach (Tilemap map in GameObject.FindObjectsOfType<Tilemap>()) {
            if (map.name == "FloorMap") {
                floorMap = map;
                break;
            }
        }
        // Section for dungeon rooms only
        mapController = floorMap.GetComponent<Initializer>();
        if (mapController != null) {
            enemies = mapController.RetrieveEnemies();
        }
        active = false;
        head = h;
        tail = t;
        width = head.x - tail.x + 1;
        height = head.y - tail.y + 1;
        center = (head + tail) / 2;
        doors = new Vector3Int[4];
        doorsOpen = new bool[4] {false,false,false,false};
        neighbors = new Room[4];
        loot = 0;
        // Is this room a special prefab?
        if (_prefab != "") {
            prefab = _prefab;
            if (prefab == "Bonemass") {
                Bonemass(parent);
            }
            return;
        } else {
            prefab = "";
        }
        if (parent != null) {
            SetNeighbors(parent, rooms);
        }
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

    public List<Vector3Int> AllCells() {
        List<Vector3Int> cells = new List<Vector3Int>();
        int x,y;
        for (int i=0; i<width; i++) {
            x = tail.x+i;
            for (int j=0; j<height; j++) {
                y = tail.y+j;
                cells.Add(new Vector3Int(x, y, 0));
            }
        }
        return cells;
    }

    public static Room FindByCell(Vector3Int cell, List<Room> rooms) {
        foreach (Room r in rooms) {
            if (r.Contains(cell)) {
                return r;
            }
        }
        return null;
    }

    public Vector3Int GetDoorCell(Room target) {
        int dir;
        Vector3Int delta = new Vector3Int(
            Mathf.RoundToInt(target.center.x - center.x),
            Mathf.RoundToInt(target.center.y - center.y),
            0);
        // Get naiive direction
        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x)) {
            if (delta.y >= 0) {
                dir = 0;
            } else {
                dir = 2;
            }
        } else {
            if (delta.x >= 0) {
                dir = 1;
            } else {
                dir = 3;
            }
        }
        // Basic direction won't work
        if (doors[dir] == null || !(doors[dir] == null || doorsOpen[dir])) {
            // Super strange logic expression, a V ~(a V b)
            // if door is null, always override, but also if door is not null and door is not open...
            int prev = dir;
            // rotate left or right
            if (dir == 0 || dir == 2) {
                // Was going up/down, go left/right
                if (delta.x >= 0) {
                    dir = 1;
                } else {
                    dir = 3;
                }
            } else {
                // Was going left/right, go up/down
                if (delta.y >= 0) {
                    dir = 0;
                } else {
                    dir = 2;
                }
            }
            if (doors[dir] == null || !(doors[dir] == null || doorsOpen[dir])) {
                // try other way
                dir = (dir+2)%4;
                if (doors[dir] == null || !(doors[dir] == null || doorsOpen[dir])) {
                    // just turn around
                    dir = (prev+2)%2;
                }
            }
        }
        Vector3Int cell = doors[dir];
        if (cell.x == 0 && cell.y == -1) {
            string s = "AHHH BAD DOOR CELL";
            foreach(Vector3Int d in doors) {
                s += d+"\n";
            }
            Debug.Log(s);
        }
        if (dir == 0) {
            cell.y++;
        } else if (dir == 1) {
            cell.x++;
        } else if (dir == 2) {
            cell.y--;
        } else if (dir == 3) {
            cell.x--;
        }
        return cell;
    }
    
    public override string ToString() {
        string s = "Room: ";
        s += head.ToString() + ", ";
        s += tail.ToString();
        return s;
    }

    private void Bonemass(Room parent) {
        enemies.AddRange(mapController.RetrieveEnemies());
        enemies.Add("Bonemass");
    }
}
