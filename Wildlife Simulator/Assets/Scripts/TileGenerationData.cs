using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class TileGenerationData : ScriptableObject
{
    public UnityEvent OnValueChanged;
    //public event System.Action OnValueChanged;
    [SerializeField]
    private ValueHolder<int> mapSize;
    [SerializeField]
    private ValueHolder<float> scale;
    [SerializeField]
    private ValueHolder<TileGenSpecs[]> myTileGenSpecsHolder;

    public int MapSize
    {
        get { return mapSize.Value; }
        set{ mapSize.Value = value; }
    }
    public float Scale
    {
        get { return scale.Value; }
        set
        {
            float regulatedValue = Mathf.Clamp(value, 0.1f, 1.0f);
            scale.Value = regulatedValue;
        }
    }
    public TileGenSpecs[] MyTileGenSpecs
    {
        get { return myTileGenSpecsHolder.Value; }
        set { myTileGenSpecsHolder.Value = value; }
    }
    void ValueChanged()
    {
        Debug.Log("Value Changed");
        OnValueChanged.Invoke();
    }
    public List<UnityAction> activeCalls = new List<UnityAction>();
    public void Register(UnityAction call)
    {
        if (activeCalls.Contains(call))
        {
            Debug.Log("Already contains " + call.ToString());
            DeRegister(call);
        }
        Debug.Log("Registered " + call + " to " + name);
        activeCalls.Add(call);
        OnValueChanged.AddListener(call);
    }
    public void DeRegister(UnityAction call)
    {
        if (activeCalls.Contains(call))
        {
            Debug.Log("Deregistered User to " + name);
            OnValueChanged.RemoveListener(call);
            activeCalls.Remove(call);
            return;
        }
    }
    public void RegisterValues()
    {
        mapSize.RegisterEvent(ValueChanged);
        scale.RegisterEvent(ValueChanged);
        foreach(TileGenSpecs TGS in myTileGenSpecsHolder.Value)
        {
            TGS.Register(myTileGenSpecsHolder.OnValueChanged);
        }
        myTileGenSpecsHolder.RegisterEvent(ValueChanged);
    }
}
[System.Serializable]
public struct TileGenSpecs
{
    public ValueHolder<float> MinSpawnPoint;
    public ValueHolder<TileData> MyTileData;
    public ValueHolder<bool> IsFlatTerrain;
    public void Register(UnityAction reciever)
    {
        MinSpawnPoint.RegisterEvent(reciever);
        MyTileData.RegisterEvent(reciever);
        IsFlatTerrain.RegisterEvent(reciever);
    }
}
public delegate void OnValueChanged();
