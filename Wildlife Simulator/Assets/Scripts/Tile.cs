using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData Data;
    Renderer _visual;
    [SerializeField] private int _stateID;
    public void SetData(TileData newData, int stateID)
    {
        if (Data == newData) return;
        Data = newData;
        if(_visual == null) _visual = GetComponent<Renderer>();
        _visual.material = newData.TileMat;
        gameObject.name = newData.TileName;
        _stateID = stateID;     
    }
    public int StateID => _stateID;
    public override string ToString()
    {
        return name + " " + StateID + "\n" + transform.position;
    }
}
