using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Utilities : MonoBehaviour
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

    public static T Choice<T>(T [] opt) {
        return opt[Random.Range(0, opt.Length)];
    }
    public static T Choice<T>(List<T> opt) {
        return opt[Random.Range(0, opt.Count)];
    }
    public static T Choice<T>(T a, T b) {
        if (Random.Range(0,2) == 1) {
            return a;
        } else {
            return b;
        }
    }

    public static List<T> Sample<T>(int count, in List<T> list) {
        List<T> newList = new List<T>();
        for (int i=0; i<list.Count; i++) {
            if (Random.value < (float)count/(list.Count-i)) {
                newList.Add(list[i]);
                count--;
                if (count == 0) break;
            }
        }
        return newList;
    }
}
