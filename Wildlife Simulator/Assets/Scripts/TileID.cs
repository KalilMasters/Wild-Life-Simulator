using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
[CreateAssetMenu]
public class TileID : ScriptableObject
{
    [SerializeField] private Tile BasicTile;
    [SerializeField] private List<TileData> tiles;
    public TileData GetTileData(int ID)
    {
        return tiles[Mathf.Clamp(ID, 0, tiles.Count)];
    }
    public int GetTileID(TileData TD)
    {
        if(tiles.Contains(TD)) return tiles.IndexOf(TD);
        return -1;
    }
    public string SerializeTile(Tile tile)
    {
        return TileSaveFile.ToJSON(ToSaveFile(tile));
    }
    public TileSaveFile ToSaveFile(Tile tile)
    {
        return new TileSaveFile(
                GetTileID(tile.Data),
                tile.StateID,
                tile.transform.position
                );
    }
    public Tile SpawnTile()
    {
        return Instantiate(BasicTile);
    }
    public Tile SpawnTile(TileSaveFile TSF)
    {
        Tile tile = SpawnTile();
        if (TSF == null)
        {
            Debug.Log("Something went wrong");
            return SpawnTile();
        }
        tile.SetData(GetTileData(TSF.TildID), TSF.TileStateID);
        tile.transform.position = TSF.TilePosition;
        return tile;
    }
}
