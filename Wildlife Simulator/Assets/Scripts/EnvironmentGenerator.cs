using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    [SerializeField] 
    ValueHolder<TileGenerationData> _tileGenDataHolder;
    [SerializeField]
    ValueHolder<bool> _regenerateOnValueChanged;
    [SerializeField]
    private ValueHolder<int> _mapSizeHolder;
    [SerializeField]
    private ValueHolder<float> _scaleHolder;
    [SerializeField]
    private TileGenSpecs[] _myTileGenSpecs;
    private Dictionary<Vector3, Tile> _tileDic;
    private int _currentActiveSize = 0;
    public TileID _tileID;
    public string SaveFileName;

    public int MapSize
    {
        get {  return  UseSO ? _tileGenDataHolder.Value.MapSize : _mapSizeHolder.Value; }
        set
        {
            int regulatedValue = Mathf.Max(value, 0);
            if (UseSO)
            {
                _tileGenDataHolder.Value.MapSize = regulatedValue;
                if(_tileGenDataHolder.ChangedSinceLastCheck())
                    OnValueChanged();
            }
            else
            {
                _mapSizeHolder.Value = regulatedValue;
                if (_mapSizeHolder.ChangedSinceLastCheck())
                    OnValueChanged();
            }
        }
    }
    public bool RegenerateOnValueChanged { 
        get { return _regenerateOnValueChanged.Value; } 
        set 
        { 
            _regenerateOnValueChanged.Value = value;
            if(_regenerateOnValueChanged.ChangedSinceLastCheck() && RegenerateOnValueChanged)
                OnValueChanged();
        } 
    }
    public float Scale
    {
        get { return UseSO ? _tileGenDataHolder.Value.Scale : _scaleHolder.Value; }
        set
        {
            if (UseSO) 
            { 
                _tileGenDataHolder.Value.Scale = value;
                if (_tileGenDataHolder.ChangedSinceLastCheck())
                    OnValueChanged();
                return; }
            _scaleHolder.Value = Mathf.Clamp(value, 0.1f, 1.0f);
            if (_scaleHolder.ChangedSinceLastCheck())
                OnValueChanged();
        }
    }
    public TileGenerationData TileGenData
    {
        get { return _tileGenDataHolder.Value; }
        set {
            //TileGenerationData oldTileGen = _tileGenDataHolder.Value;
            _tileGenDataHolder.Value = value;
            if (_tileGenDataHolder.ChangedSinceLastCheck())
            {
                OnValueChanged();
                //if (oldTileGen != null)
                    //oldTileGen.DeRegister(_tileGenDataHolder.OnValueChanged);
                //if(_tileGenDataHolder.Value != null)
                    //_tileGenDataHolder.Value.Register(_tileGenDataHolder.OnValueChanged);
            }
        }
    }
    public TileGenSpecs[] MyTileGenSpecs
    {
        get { return UseSO? TileGenData.MyTileGenSpecs : _myTileGenSpecs; }
        set
        {
            if (UseSO) TileGenData.MyTileGenSpecs = value;
            else
            {
                var oldSpecs = _myTileGenSpecs;
                _myTileGenSpecs = value;
                if (oldSpecs != MyTileGenSpecs)
                    OnValueChanged();
            }
        }
    }
    public Tile TilePrefab;
    public string FileName;
    public bool UseSO => _tileGenDataHolder != null;
    public bool loaded = false;

    public void GenerateNewEnvironment(int seed = 0)
    {
        print($"Generating New Environment..\n{_currentActiveSize} -> {MapSize}");
        FileInterface.CloseFile();
        if (_tileDic == null || _tileDic.Count == 0) GetTilesInScene();

        seed = Mathf.Max(seed, 0);
        //Make a grid of tiles of size "mapSize"
        for (int x = 0; x < MapSize || x < _currentActiveSize; x++)
        {
            for (int z = 0; z < MapSize || z < _currentActiveSize; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                if(!_tileDic.ContainsKey(pos))
                    _tileDic.Add(pos, 
                        Instantiate(TilePrefab, pos, Quaternion.identity, transform));
                Tile curTile = _tileDic[pos];
                //print($"{pos} : {curTile}");
                if(z >= MapSize || x >= MapSize)
                {
                    curTile.gameObject.SetActive(false);
                    continue;
                }
                curTile.gameObject.SetActive(true);
                float perlinValue = Mathf.PerlinNoise(x * Scale + seed, z * Scale + seed);
                curTile.SetData(GetTileData(perlinValue), 0);
            }
        }
        _currentActiveSize = MapSize;
        TileData GetTileData(float value)
        {
            if (MyTileGenSpecs == null || MyTileGenSpecs.Length == 0) return null;
            int index = 0;;
            for (int i = 0; i < MyTileGenSpecs.Length; i++)
            {
                if (value < MyTileGenSpecs[i].MinSpawnPoint.Value) break;
                index = i;
            }
            int clampedIndex = Mathf.Clamp(index, 0, MyTileGenSpecs.Length - 1);
            return MyTileGenSpecs[clampedIndex].MyTileData.Value;
        }
    }
    public void OnValueChanged()
    {
        if (!_regenerateOnValueChanged.Value) return;
        GenerateNewEnvironment();
    }
    public void ResetTileDic()
    {
        FileInterface.CloseFile();
        GetTilesInScene();
        if(_tileDic != null)
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
    void GetTilesInScene()
    {
        print("getting tiles already in scene");
        if (_tileDic == null) _tileDic = new Dictionary<Vector3, Tile>();
        List<Tile> overlappingTiles = new List<Tile>();
        foreach(Tile t in GetComponentsInChildren<Tile>(true))
        {
            if (_tileDic.ContainsValue(t)) continue;
            Vector3 pos = new Vector3((int)t.transform.position.x,
                (int)t.transform.position.y,
                (int)t.transform.position.z);
            if (_tileDic.ContainsKey(pos)) 
                overlappingTiles.Add(t);
            else
                _tileDic.Add(pos, t);
        }
        while(overlappingTiles.Count > 0)
        {
            Tile curT = overlappingTiles[0];
            overlappingTiles.Remove(curT);
            GameObject.DestroyImmediate(curT.gameObject);
        }
    }
    void DeleteTileFromDic(Vector3 Key)
    {
        Tile T = _tileDic[Key];
        _tileDic.Remove(Key);
        GameObject.DestroyImmediate(T.gameObject);
    }
    public void OnSceneLoaded()
    {
        if (loaded) return;
        loaded = true;
        InitValue();
    }
    void InitValue()
    {
        print("init Values");
        _mapSizeHolder.Value = 0;

        TileGenData?.Register(OnValueChanged);
        _tileGenDataHolder.RegisterEvent(OnValueChanged);
        _regenerateOnValueChanged.RegisterEvent(OnValueChanged);
        _mapSizeHolder.RegisterEvent(OnValueChanged);
        _scaleHolder.RegisterEvent(OnValueChanged);
        GetTilesInScene();
    }
    private void OnEnable()
    {
        OnSceneLoaded();
    }
    private void OnDisable()
    {
        print("Turnign off");
        loaded = false;
        _tileGenDataHolder?.Value.DeRegister(OnValueChanged);
    }
    private void OnValidate()
    {
        Debug.Log("OnValidate");
        //OnValueChanged();
    }
    public void SaveToFile()
    {
        print("Saving...");
        string saveString = "";
        int index = 0;
        foreach(var Key in _tileDic.Keys)
        {
            Tile curTile = _tileDic[Key];
            if (!curTile.gameObject.activeInHierarchy) continue;
            string serializedTile = _tileID.SerializeTile(curTile);
            saveString += serializedTile;
            if (index != _tileDic.Count - 1) saveString += "|";
            print(serializedTile);
            index++;
        }
        FileInterface.LoadFile("Environments", SaveFileName, "txt", false, true);
        print(saveString);
        FileInterface.Write(saveString, true);
        FileInterface.CloseFile();
        print("Finished Saving");
    }
    
}
[System.Serializable]
public class TileSaveFile
{
    public int TildID;
    public int TileStateID;
    public Vector3 TilePosition;
    public TileSaveFile(int tildID, int tileStateID, Vector3 tilePosition)
    {
        TildID = tildID;
        TileStateID = tileStateID;
        TilePosition = tilePosition;
    }
    public static string ToJSON(TileSaveFile TSF)
    {
        return JsonUtility.ToJson(TSF);
    }
    public static TileSaveFile FromJSON(string SaveString)
    {
        return JsonUtility.FromJson(SaveString, typeof(TileSaveFile)) as TileSaveFile;
    }
    public override string ToString()
    {
        return TildID + " " + TileStateID + "\n" + TilePosition;
    }
}


