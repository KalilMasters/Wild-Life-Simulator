using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentLoader : MonoBehaviour
{
    public string FileName;
    public TileID TileIDs;
    public
    Dictionary<Vector3, Tile> _tileDic = new Dictionary<Vector3, Tile>();
    private void Awake()
    {
        LoadEnvironment();
    }
    public void ClearEnvironment()
    {
        //throw new System.Exception("Define this method");
        FileInterface.CloseFile();
        if (_tileDic != null)
        {
            print("Clearing old tiles");
            List<Vector3> tileKeys = new List<Vector3>(_tileDic.Keys);
            print("Tiles to delete: " + tileKeys.Count);
            while (tileKeys.Count > 0)
            {
                Vector3 currentKey = tileKeys[0];
                tileKeys.RemoveAt(0);
                DeleteTileFromDic(currentKey);
            }
        }
        _tileDic = new Dictionary<Vector3, Tile>();
    }
    void DeleteTileFromDic(Vector3 Key)
    {
        Tile T = _tileDic[Key];
        _tileDic.Remove(Key);
        GameObject.DestroyImmediate(T.gameObject);
    }

    public void LoadEnvironment()
    {
        if(TileIDs == null)
        {
            print("Please Assign a TileIDHolder");
            return;
        }
        ClearEnvironment();
        FileInterface.LoadFile("Environments", FileName, "txt", true, true);
        string[] tileStringArr = FileInterface.ReadLine(false).Split('|');
        FileInterface.CloseFile();
        StartCoroutine(CreateWorld());
        
        IEnumerator CreateWorld()
        {
            foreach (string s in tileStringArr)
            {
                TileSaveFile TSF = TileSaveFile.FromJSON(s);
                if (TSF == null) continue;
                Tile tile = TileIDs.SpawnTile(TSF);
                _tileDic.Add(TSF.TilePosition, tile);
                yield return null;
            }
        }
    }
    public void SaveEnvironment()
    {
        if (TileIDs == null)
        {
            print("Please Assign a TileIDHolder");
            return;
        }
        if (_tileDic == null || _tileDic.Count == 0)
        {
            print("No Tiles Found");
        }
    }
}
