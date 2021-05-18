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
    }

    public int PathFind(Vector3Int s, Vector3Int e) {
        start = s;
        end = e;
        map.Clear();
        Node first = new Node(start, start, end);
        Search(first);
    }

    private bool Search(Node currentNode) {
        currentNode.state = NodeState.Closed;
        List<Node> nextNodes = GetWalkableNodes(currentNode);
        nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
        foreach (var nextNode in nextNodes) {

        }
    }

    private List<Node> GetWalkableNodes(Node fromNode) {
        List<Node> walkable = new List<Node>();
        IEnumerable<Vector3Int> nextLocations = GetAdjacentLocations(fromNode.cell);

        foreach (var cell in nextLocations) {
            Node node = this.nodes[cell.x, cell.y];
            // Ignore non-walkables or closed nodes
            if (!node.walkable || node.state == NodeState.Closed) {
                continue;
            }
            // ! Finish me!
        }
    }

    // Give four adjacent cells, creating new child nodes if necessary
    private IEnumerable<Vector3Int> GetAdjacentLocations(Vector3Int cell) {
        int x = cell.x, y = cell.y;
        List<Vector3Int> adj = new();
        for (int i=0; i<4; i++) {
            if (i==0)      {y++;}
            else if (i==1) {y -= 2;}
            else if (i==2) {y++; x++;}
            else if (i==3) {x -= 2;}
            adj.Add(new Vector3Int(x, y, 0));

            if (nodes[x, y] == null) {
                nodes[x, y] = new Node(cell, start, end, nodes[cell.x, cell.y]);
            }
        }
        return adj;
    }
}
