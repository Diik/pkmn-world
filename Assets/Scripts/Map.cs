using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

    private Dictionary<Vector2, GameObject> TileSet = new Dictionary<Vector2, GameObject>();
    private Vector2? FirstPosition;
    private int lastTileX = 0;
    private int lastTileY = 0;

    /*
     * This will load the tiles dynamically
     * according to the player's geolocation
     * in the real world.
     */
    void FixedUpdate()
    {
        UpdateTiles();
    }

    void UpdateTiles()
    {
        // make sure location is initialized
        if (LocationController.GetStatus() != LocationServiceStatus.Running) return;

        var lastLoc = Input.location.lastData;
        int[] tilePos = WorldToTileCoords(lastLoc.latitude, lastLoc.longitude);

        SetCurrentTile(tilePos[0], tilePos[1]);
    }

    /*
     * Set the current tile and calculate which tiles need
     * to be removed/added.
     */
    void SetCurrentTile(int x, int y) {
        // Don't regenerate tiles if it's the same tile position as before
        if (lastTileX == x && lastTileY == y) return;

        if (FirstPosition == null)
            FirstPosition = new Vector2(x, y);

        // Make a list of all the required tile positions
        List<Vector2> tiles = new List<Vector2>();
        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                tiles.Add(new Vector2(x + i, y + j));
            }
        }

        // Remove old tiles
        foreach (var item in TileSet) {
            if (tiles.IndexOf(item.Key) == -1) {

                // Destroy the tile
                Destroy(item.Value);

                // Remove it from the active TileSet
                TileSet.Remove(item.Key);
            }
        }

        // Add new tiles
        foreach (Vector2 tile in tiles) {
            if (!TileSet.ContainsKey(tile)) {

                // Create the tile object
                GameObject obj = new GameObject("Tile");
                obj.transform.parent = transform;

                /*
                 * Add the tile behaviour and set both the position (x & y)
                 * and the world position.
                 */
                Tile t = obj.AddComponent<Tile>();
                t.Position = new Vector2(tile.x, tile.y);
                t.WorldPosition = new Vector3(
                    ((tile.x - FirstPosition.Value.x) * 100),
                    0,
                    ((tile.y - FirstPosition.Value.y) * 100)
                );

                // Add the tile to the tileset dictionary
                TileSet.Add(tile, obj);
            }
        }

        // Refresh last tile position
        lastTileX = x;
        lastTileY = y;
    }

    /*
     * Transform real world coordinates (lat/long)
     * to the tile (x/y) coordinates.
     * This is the opposite of TileToWorldCoords.
     */
    public static int[] WorldToTileCoords(double lat, double lon, int zoom = 15) {
        return new int[] {
            (int)((lon + 180.0) / 360.0 * (1 << zoom)),
            (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom))
        };
    }

    /*
     * Transform tile (x/y) coordinates to the real world
     * coordinates (lat/long). This will always return the
     * top-left coordinates of a tile.
     */
    public static float[] TileToWorldCoords(int x, int y, int zoom = 15) {
        double n = Math.PI - ((2.0 * Math.PI * y) / Math.Pow(2.0, zoom));

        return new float[] {
            (float)((x / Math.Pow(2.0, zoom) * 360.0) - 180.0),
            (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)))
        };
    }

}
