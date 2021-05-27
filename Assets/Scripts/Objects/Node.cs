using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3Int cell;
    public bool walkable;
    public int G, H, F;
    public NodeState state;
    public Node ParentNode;
    private PathFinder pathFinder;

    public Node(Vector3Int c, Vector3Int start, Vector3Int end, PathFinder finder, Node parent=null) {
        pathFinder = finder;
        cell = c;
        state = NodeState.Untested;
        ParentNode = parent;
        H = GetTraversalCost(cell, end);
        if (parent == null) {
            Debug.Log("Parent is null.");
            G = 0;
            F = H;
        } else {
            //G = parent.G + GetTraversalCost(parent.cell, cell);
            G = parent.G+1;
            UpdateParent(parent);
        }
    }

    public void UpdateParent(Node p) {
        ParentNode = p;
        G = p.G + 1;
        F = G + H;
        walkable = pathFinder.IsWalkable(ParentNode.cell, cell);
        Debug.Log("Node "+cell.ToString()+" walkable? "+walkable);
    }

    // Calculates Manhattan distance
    public static int GetTraversalCost(Vector3Int start, Vector3Int end) {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }
}

public enum NodeState {Untested, Open, Closed}
