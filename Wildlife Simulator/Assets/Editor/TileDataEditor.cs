using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TileData))]
public class TileDataEditor : Editor
{
    TileData TD;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(TD == null) TD = target as TileData;
        //TD.TileMat = EditorGUILayout.ObjectField("Material", TD.TileMat, typeof(Material), false) as Material;
        //TD.TileName = EditorGUILayout.TextField("Name", TD.TileName);
    }
}
