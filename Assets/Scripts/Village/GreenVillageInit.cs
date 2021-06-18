using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GreenVillageInit : MonoBehaviour
{
    private PersistentData data;
    private VillageController villageController;
    private Dictionary<string,Vector3Int> notableCells;
    private PlayerController player; 
    private NPCController npcController;
    private Tilemap floorMap, leftWallMap, rightWallMap, blockMap, roofMap;
    private List<Room> rooms;
    private Dictionary<string,Tile> tiles;
    public Dictionary<Vector3Int, WorldTile> roofTiles;
    private List<Vector3Int> activeRoofCells, activeLeftWallCells, activeRightWallCells;
    private Vector3Int doorCell;
    private string doorFace;
    private bool clarifying = false, declarifying = false;
    private float count = 1.0f;
    void Awake()
    {
        data = GameObject.FindWithTag("Data").GetComponent<PersistentData>();
        villageController = gameObject.GetComponent<VillageController>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        npcController = GameObject.Find("NPCs").GetComponent<NPCController>();

        foreach (Tilemap map in FindObjectsOfType<Tilemap>()) {
            if (map.name == "FloorMap") {
                floorMap = map;
            } else if (map.name == "LeftWallMap") {
                leftWallMap = map;
            } else if (map.name == "RightWallMap") {
                rightWallMap = map;
            } else if (map.name == "BlockMap") {
                blockMap = map;
            } else if (map.name == "RoofMap") {
                roofMap = map;
            }
        }
        rooms = new List<Room>();
        rooms.Add(new Room(new Vector3Int(5,0,0), new Vector3Int(2,-3,0)));
        rooms.Add(new Room(new Vector3Int(4,9,0), new Vector3Int(1,4,0)));

        // -- NEW SHIT! -- //
        GetWorldTiles();
        foreach (KeyValuePair<Vector3Int, WorldTile> kv in roofTiles) {
            //Debug.Log("Tile " + kv.Value.Name + " is at: " + kv.Key);
        }
        /*WorldTile _tile;
        if (roofTiles.TryGetValue(new Vector3Int(4,-2,0), out _tile)) {
            Debug.Log("Tile " + _tile.Name + " costs: " + _tile.Cost);
            _tile.TilemapMember.SetTileFlags(_tile.LocalPlace, TileFlags.None);
            _tile.TilemapMember.SetColor(_tile.LocalPlace, Color.blue);
        }*/
        // -- --------- -- //

        activeRoofCells = new List<Vector3Int>();
        activeLeftWallCells = new List<Vector3Int>();
        activeRightWallCells = new List<Vector3Int>();

        tiles = new Dictionary<string, Tile>();
        tiles.Add("leftWall", Resources.Load<Tile>("Tiles/GreenVillage/leftWall"));
        tiles.Add("rightWall", Resources.Load<Tile>("Tiles/GreenVillage/rightWall"));
        tiles.Add("leftDoor", Resources.Load<Tile>("Tiles/GreenVillage/leftDoor"));
        tiles.Add("leftDoorOpen", Resources.Load<Tile>("Tiles/GreenVillage/leftDoorOpen"));
        tiles.Add("rightDoor", Resources.Load<Tile>("Tiles/GreenVillage/rightDoor"));
        tiles.Add("rightDoorOpen", Resources.Load<Tile>("Tiles/GreenVillage/rightDoorOpen"));

        notableCells = new Dictionary<string, Vector3Int>();
        notableCells.Add("stairsDown", new Vector3Int(0,-9,0));
        notableCells.Add("shopkeeper", new Vector3Int(5,-2,0));
        notableCells.Add("barkeep", new Vector3Int(4,6,0));

        villageController.UpdateNotables(notableCells);
    }

    void Update() {
        if (clarifying) {
            if (count < 1.0f) {
                count += 3 * Time.deltaTime;
                foreach (Vector3Int cell in activeRoofCells) {
                    roofMap.SetColor(cell, new Color(1,1,1,(1-count)));
                }
                foreach (Vector3Int cell in activeLeftWallCells) {
                    leftWallMap.SetColor(cell, new Color(1,1,1,(1-count*0.75f)));
                }
                foreach (Vector3Int cell in activeRightWallCells) {
                    rightWallMap.SetColor(cell, new Color(1,1,1,(1-count*0.75f)));
                }
            } else {
                if (doorFace == "left") {
                    leftWallMap.SetTile(doorCell, tiles["leftDoor"]);
                    leftWallMap.SetTileFlags(doorCell, TileFlags.None);
                    leftWallMap.SetColor(doorCell, new Color(1,1,1,0.25f));
                } else {
                    rightWallMap.SetTile(doorCell, tiles["rightDoor"]);
                    rightWallMap.SetTileFlags(doorCell, TileFlags.None);
                    rightWallMap.SetColor(doorCell, new Color(1,1,1,0.25f));
                }
                clarifying = false;
            }
        } else if (declarifying) {
            if (count < 1.0f) {
                count += 3 * Time.deltaTime;
                foreach (Vector3Int cell in activeRoofCells) {
                    roofMap.SetColor(cell, new Color(1,1,1,(count)));
                }
                foreach (Vector3Int cell in activeLeftWallCells) {
                    leftWallMap.SetColor(cell, new Color(1,1,1,(0.25f+count*0.75f)));
                }
                foreach (Vector3Int cell in activeRightWallCells) {
                    rightWallMap.SetColor(cell, new Color(1,1,1,(0.25f+count*0.75f)));
                }
            } else {
                if (doorFace == "left") {
                    leftWallMap.SetTile(doorCell, tiles["leftDoor"]);
                } else {
                    rightWallMap.SetTile(doorCell, tiles["rightDoor"]);
                }
                declarifying = false;
            }
        }
    }

    public void GetWorldTiles() {
        roofTiles = new Dictionary<Vector3Int, WorldTile>();
        foreach (Vector3Int pos in roofMap.cellBounds.allPositionsWithin) {
            var localPlace = new Vector3Int(pos.x,pos.y,pos.z);

            if (!roofMap.HasTile(localPlace)) continue;
            var tile = new WorldTile {
                LocalPlace = localPlace,
                WorldLocation = roofMap.CellToWorld(localPlace),
                TileBase = roofMap.GetTile(localPlace),
                TilemapMember = roofMap,
                Name = localPlace.x+","+localPlace.y,
                Cost = 1
            };
            roofTiles.Add(tile.LocalPlace, tile);
        }
    }

    public Vector3Int FetchPosition() {
        if (data.entrance == 1) {
            data.direction = 0;
            return new Vector3Int(0,-8,0);
        } else if (data.entrance == 2) {
            Vector3Int cell = new Vector3Int(1,9,0);
            player.tilePosition = new Vector3Int(2,3,0);
            OpenDoor(new Vector3Int(2,3,0), 0);
            return cell;
        }
        return new Vector3Int(0,0,0);
    }

    public void NotableActions(string key) {
        if (key == "stairsDown") {
            data.depth++;
            data.floorDirection = "down";
            SceneManager.LoadScene("BasicDungeon");
        } else if (key == "shopkeeper") {
            player.enabled = false;
            npcController.SpeakToShopkeeper();
        } else if (key == "barkeep") {
            Debug.Log("Talking to barkeep!");
            player.enabled = false;
            npcController.SpeakToBarkeep();
        }
    }

    public void OpenDoor(Vector3Int cell, int dir) {
        if (Room.FindByCell(player.tilePosition, rooms) == null) {
            // Coming in from outside, clarify!
            Vector3Int finderCell = cell;
            if      (dir == 0) { finderCell.y++; }
            else if (dir == 1) { finderCell.x++; }
            Room r = Room.FindByCell(finderCell, rooms);
            activeRoofCells.Clear();
            foreach (Vector3Int c in r.AllCells()) {
                Vector3Int roofCell = new Vector3Int(c.x+1, c.y+1, c.z);
                activeRoofCells.Add(roofCell);
                roofMap.SetTileFlags(roofCell, TileFlags.None);
            }
            SetActiveWalls(r);
            clarifying = true;
            count = 0;
        } else {
            // Leaving, declarifying!
            declarifying = true;
            count = 0;
        }
        doorCell = cell;
        if (dir == 0 || dir == 2) {
            leftWallMap.SetTile(cell, tiles["leftDoorOpen"]);
            doorFace = "left";
        } else {
            rightWallMap.SetTile(cell, tiles["rightDoorOpen"]);
            doorFace = "right";
        }
    }

    public void SetActiveWalls(Room r) {
        activeLeftWallCells.Clear();
        activeRightWallCells.Clear();
        Vector3Int cell = r.tail;
        cell.y--;
        for (int i=0; i<r.width ;i++) {
            activeLeftWallCells.Add(new Vector3Int(cell.x, cell.y, cell.z));
            leftWallMap.SetTileFlags(cell, TileFlags.None);
            cell.x++;
        }
        cell = r.tail;
        cell.x--;
        for (int i=0; i<r.height ;i++) {
            activeRightWallCells.Add(new Vector3Int(cell.x, cell.y, cell.z));
            rightWallMap.SetTileFlags(cell, TileFlags.None);
            cell.y++;
        }
    }
}
