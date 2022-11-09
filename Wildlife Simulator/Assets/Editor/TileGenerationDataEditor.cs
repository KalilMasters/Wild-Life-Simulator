using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TileGenerationData))]
public class TileGenerationDataEditor : Editor
{
    TileGenerationData TGD;
    private void OnEnable()
    {
        if(TGD == null) TGD = target as TileGenerationData;
        TGD.RegisterValues();
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        TGD.MapSize = EditorGUILayout.IntField("Map Size", TGD.MapSize);
        TGD.Scale = EditorGUILayout.FloatField("Scale", TGD.Scale);
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("myTileGenSpecsHolder"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
