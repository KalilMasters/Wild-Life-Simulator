using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Unity.VisualScripting.FullSerializer;

[CustomEditor(typeof(EnvironmentGenerator))]
public class EnvironmentGeneratorEditor : Editor
{
    EnvironmentGenerator EG;
    private ReorderableList list;
    private SerializedProperty TileGenSpecs;
    private void OnEnable()
    {
        TileGenSpecs = serializedObject.FindProperty("_myTileGenSpecs");

        list = new ReorderableList(serializedObject, TileGenSpecs, true, true, true, true);
        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
        if (EG == null)
            EG = target as EnvironmentGenerator;
        EG.OnSceneLoaded();
    }
    public override void OnInspectorGUI()
    {
        EG._tileID = EditorGUILayout.ObjectField("IDs", EG._tileID, typeof(TileID), false) as TileID;
        EG.RegenerateOnValueChanged = EditorGUILayout.Toggle("Regenerate On Value Changed", EG.RegenerateOnValueChanged);
        EG.TileGenData = EditorGUILayout.ObjectField(EG.TileGenData, typeof(TileGenerationData), false) as TileGenerationData;
        EG.MapSize = EditorGUILayout.IntField("Map Size", EG.MapSize);
        EG.Scale = EditorGUILayout.FloatField("Scale", EG.Scale);
        EG.TilePrefab = EditorGUILayout.ObjectField("Tile Prefab", EG.TilePrefab, typeof(Tile), false) as Tile;
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_myTileGenSpecs"), true);
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Generate"))
            EG.GenerateNewEnvironment();
        if (GUILayout.Button("Clear Tiles"))
            EG.ResetTileDic();
        EG.SaveFileName = EditorGUILayout.TextField("FileName: ", EG.SaveFileName);
        if (GUILayout.Button("Save Layout"))
            EG.SaveToFile();
    }
    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), "TileSpecs");
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("TilePrefab"),
            GUIContent.none);
    }
    void DrawHeader(Rect rect)
    {
        string name = "Tile Gen Specs";
        EditorGUI.LabelField(rect, name);
    }
    private void OnValidate()
    {
        Debug.Log("OnValidate Editor");
        //EG.OnValueChanged();
    }
}
