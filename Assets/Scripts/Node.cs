using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3Int cell;
    public bool walkable;
    public float G, H, F;
    public NodeState state;
    public Node ParentNode;

    public Node(Vector3Int c, Vector3Int start, Vector3Int end, Node parent=null) {
        cell = c;
    }
}

public enum NodeState {Untested, Open, Closed}
