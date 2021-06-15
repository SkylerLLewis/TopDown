using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    public class MapArray<T> {
        private T [,] array;
        private int offset;
        public MapArray() {
            array = new T[100,100];
            offset = 50;
        }
        public T this[int i, int j] {
            get { return array[i+offset, j+offset]; }
            set { array[i+offset, j+offset] = value; }
        }
        public void Clear() {
            array = new T[100,100];
        }
    }

    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap;
    public Vector3Int start, end;
    // A 2d map of Nodes
    public MapArray<Node> nodes;
    public Dictionary<string, Tilemap> maps;

    void Start()
    {
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
        maps = new Dictionary<string, Tilemap>();
        maps.Add("left", leftWallMap);
        maps.Add("right", rightWallMap);
        nodes = new MapArray<Node>();
    }

    public static Vector3 TileToWorld(Vector3Int p) {
        return new Vector3(p.x-p.y, (p.x+p.y)/2f, 0);
    }

    public static Vector3Int WorldToTile(Vector3 p) {
        Debug.Log("World Vec: "+p+"\n  x:"+p.x+" - "+p.y+"\n  y:"+p.y+" - "+p.x);
        return new Vector3Int(
            Mathf.RoundToInt(p.x+p.y/2),
            Mathf.RoundToInt(p.y-p.x),
            0);
    }

    public int PathFind(Vector3Int s, Vector3Int e) {
        start = s;
        end = e;
        nodes.Clear();
        Node first = new Node(start, start, end, this);
        nodes[start.x,start.y] = first;

        if (Search(first)) {
            // Trace back the path and return the next direction
            Node me = nodes[start.x, start.y];
            Node next = nodes[end.x, end.y];
            Debug.Log("Backtracing:");
            while (next.ParentNode != me) {
                Debug.Log("     "+next.cell.ToString());
                next = next.ParentNode;
            }
            if (next.cell.y > me.cell.y) {
                return 0;
            } else if (next.cell.x > me.cell.x) {
                return 1;
            } else if (next.cell.y < me.cell.y) {
                return 2;
            } else if (next.cell.x < me.cell.x) {
                return 3;
            }
        }
        return -1;
    }

    private bool Search(Node currentNode) {
        Debug.Log("Searching from "+currentNode.cell.ToString());
        currentNode.state = NodeState.Closed;
        List<Node> nextNodes = GetWalkableNodes(currentNode);
        nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
        Debug.Log("Nodes are:");
        foreach (var nextNode in nextNodes) {
            Debug.Log("    "+nextNode.cell.ToString());
            if (nextNode.cell == end) {// End found
                return true;
            } else {
                if (Search(nextNode)) {
                    return true;
                }
            }
        }
        return false;
    }

    private List<Node> GetWalkableNodes(Node fromNode) {
        List<Node> walkable = new List<Node>();
        IEnumerable<Vector3Int> nextLocations = GetAdjacentLocations(fromNode.cell);

        Debug.Log("Adjacent Locations: ");
        foreach (var cell in nextLocations) {
            // Stay on the floor
            if (!floorMap.HasTile(cell)) {
                continue;
            }
            Debug.Log("    "+cell.ToString());
            Node node = this.nodes[cell.x, cell.y];
            // Ignore non-walkables or closed nodes
            if (!node.walkable || node.state == NodeState.Closed) {
                if (! node.walkable) {
                    Debug.Log("Unwalkable!");
                } else {
                    Debug.Log("Closed!");
                }
                continue;
            }
            // Add open nodes to the list if their G-value is
            // lower going this route
            if (node.state == NodeState.Open) {
                float traversalCost = Node.GetTraversalCost(node.cell, node.ParentNode.cell);
                float gTemp = fromNode.G + traversalCost;
                if (gTemp < node.G && IsWalkable(fromNode.cell, node.cell)) {
                    node.UpdateParent(fromNode);
                    walkable.Add(node);
                }
            } else { // Set parent of untested nodes and flag
                node.UpdateParent(fromNode);
                node.state = NodeState.Open;
                walkable.Add(node);
            }
        }
        return walkable;
    }

    // Give four adjacent cells, creating new child nodes if necessary
    private IEnumerable<Vector3Int> GetAdjacentLocations(Vector3Int cell) {
        int x = cell.x, y = cell.y;
        List<Vector3Int> adj = new List<Vector3Int>();
        for (int i=0; i<4; i++) {
            if (i==0)      {y++;}
            else if (i==1) {y -= 2;}
            else if (i==2) {y++; x++;}
            else if (i==3) {x -= 2;}
            adj.Add(new Vector3Int(x, y, 0));

            if (nodes[x, y] == null) {
                Debug.Log("Creating new node!");
                nodes[x, y] = new Node(new Vector3Int(x, y, 0), start, end, this, nodes[cell.x, cell.y]);
            }
        }
        return adj;
    }

    public bool IsWalkable(Vector3Int from, Vector3Int to, string obstructors="") {
        // Check for blocks
        if (blockMap.HasTile(to)) {
            return false;
        }
        // Check for walls
        string face = "";
        Vector3Int wallCell = from;
        if (to.y > from.y) {
            face = "left";
        } else if (to.x > from.x) {
            face = "right";
        } else if (to.y < from.y) {
            face = "left";
            wallCell.y--;
        } else if (to.x < from.x) {
            face = "right";
            wallCell.x--;
        }
        if (maps[face].HasTile(wallCell)) {
            string targetWall = maps[face].GetTile(wallCell).name.ToLower();
            if (targetWall.IndexOf(face) >= 0 && targetWall.IndexOf("open") < 0) {
                return false;
            }
        }
        if (obstructors == "enemy") {
            GameObject [] entities = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in entities) {
                EnemyBehavior es = e.GetComponent<EnemyBehavior>();
                if (es.tilePosition == to && !es.dying) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool LineOfSight(Vector3Int from, Vector3Int to) {
        string s = "PATHFINDING: Line of sight";
        int deltax = to.x - from.x;
        int deltay = to.y - from.y;
        int xdir, ydir;
        if (deltax >= 0) {
            xdir = 1;
        } else {
            xdir = 3;
        }
        if (deltay >= 0) {
            ydir = 0;
        } else {
            ydir = 2;
        }
        float xcost, ycost;
        
        s += "\n    Starting at: "+from;
        Vector3Int xtest = new Vector3Int(), ytest = new Vector3Int(), walk = from, targetCell;
        while (walk != to) {
            if (deltax == 0) { // vertical target
                targetCell = DirectionToCell(ydir, walk);
            } else if (deltay == 0) { // horizontal target
                targetCell = DirectionToCell(xdir, walk);
            } else {
                // Try x
                xtest = DirectionToCell(xdir, walk);
                xcost = Vector3Int.Distance(xtest, to);
                s += "\n        x: "+xcost;
                // Try y 
                ytest = DirectionToCell(ydir, walk);
                ycost = Vector3Int.Distance(ytest, to);
                s += "  y: "+ycost;
                // Closest to target?
                if (xcost < ycost) {
                    targetCell = xtest;
                } else if (xcost > ycost) {
                    targetCell = ytest;
                } else {// Cross over when unsure
                    if (Mathf.Abs(deltax) < Mathf.Abs(deltay)) {
                        targetCell = xtest;
                    } else {
                        targetCell = ytest;
                    }
                }
            }

            s += "\n    Walking to "+targetCell;
            if (IsWalkable(walk, targetCell)) {
                walk = targetCell;
            } else {
                //Debug.Log(s+"\nFailed.");
                return false;
            }
        }
        //Debug.Log(s+"\nSuccess!");
        return true;
    }

    private int GCD(int a, int b) {
        int greater = a;
        if (b > a) { greater = b; }
        for (int i = greater; i>0; i--) {
            if (a%i == 0 && b%i == 0) {
                return i;
            }
        }
        return 1;
    }

    public Vector3Int DirectionToCell(int direction, Vector3Int currentCell) {
        Vector3Int cell = currentCell;
        if (direction == 0) {
            cell.y++;
        } else if (direction == 1) {
            cell.x++;
        } else if (direction == 2) {
            cell.y--;
        } else if (direction == 3) {
            cell.x--;
        }
        return cell;
    }

    public bool DirectionWalkable(Vector3Int currentCell, int direction, string obstructors="") {
        return IsWalkable(currentCell, DirectionToCell(direction, currentCell), obstructors);
    }

    public List<Vector3Int> GetTilesInSight(Vector3Int center, int range) {
        List<Vector3Int> cells = new List<Vector3Int>();
        Vector3Int cell = new Vector3Int();
        int width = center.x+range;
        int height = center.y+range;
        for (int x=center.x-range; x <= width; x++) {
            for (int y=center.y-range; y <= height; y++) {
                cell.x = x;
                cell.y = y;
                if (floorMap.GetTile(cell) != null) {
                    if (Vector3Int.Distance(center, cell) <= 6 &&
                    LineOfSight(center, cell)) {
                        Vector3Int copy = cell;
                        cells.Add(cell);
                    }
                }
            }
        }
        return cells;
    }
}
