using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonUtils : MonoBehaviour
{
    public static void PlaceTile(Tilemap map, Vector3Int cell, Tile tile) {
        if (tile == null) {
            map.SetTile(cell, null);
            return;
        }
        Tile clone = Instantiate(tile);
        clone.name = clone.name.Split('(')[0];
        map.SetTile(cell, clone);
        map.SetTileFlags(cell, TileFlags.None);
    }
}
