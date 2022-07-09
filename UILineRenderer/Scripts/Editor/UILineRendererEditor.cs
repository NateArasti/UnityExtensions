using UnityEditor;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable once CheckNamespace

[CustomEditor(typeof(UILineRenderer))]
public class UILineRendererEditor : Editor
{
    private SerializedProperty _pointsProperty;
    private bool _showPoints;

    private void OnEnable()
    {
        _pointsProperty = serializedObject.FindProperty("_points");
    }

    public override void OnInspectorGUI()
    {
        var lineRenderer = target as UILineRenderer;

        DrawDefaultInspector();
        if (GUILayout.Button("Set Vertices Prefabs")) lineRenderer.SetVerticesPrefabs();
        if (GUILayout.Button("Delete Vertices Prefabs")) lineRenderer.DeleteVerticesPrefabs();

        GUILayout.Space(20);
        _showPoints = EditorGUILayout.Foldout(_showPoints, "Points");
        if (_showPoints)
        {
            var length = _pointsProperty.arraySize;
            EditorGUILayout.IntField("Size", _pointsProperty.arraySize);
            for (var i = 0; i < length; i++)
            {
                EditorGUILayout.PropertyField(_pointsProperty.GetArrayElementAtIndex(i));
            }
        }

        if (GUILayout.Button("+")) lineRenderer.AddPoint();
        if (GUILayout.Button("-")) lineRenderer.RemoveLastPoint();

        serializedObject.ApplyModifiedProperties();
    }
}