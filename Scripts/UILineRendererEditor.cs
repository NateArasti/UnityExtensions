using UnityEditor;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable once CheckNamespace

[CustomEditor(typeof(UILineRenderer))]
public class UILineRendererEditor : Editor
{
    private SerializedProperty m_PointsProperty;
    private bool m_ShowPoints;

    private void OnEnable()
    {
        m_PointsProperty = serializedObject.FindProperty("_points");
    }

    public override void OnInspectorGUI()
    {
        var lineRenderer = target as UILineRenderer;

        DrawDefaultInspector();
        if (GUILayout.Button("Set Vertices Prefabs")) lineRenderer.SetVerticesPrefabs();
        if (GUILayout.Button("Delete Vertices Prefabs")) lineRenderer.DeleteVerticesPrefabs();

        GUILayout.Space(20);
        m_ShowPoints = EditorGUILayout.Foldout(m_ShowPoints, "Points");
        if (m_ShowPoints)
        {
            var length = m_PointsProperty.arraySize;
            EditorGUILayout.IntField("Size", m_PointsProperty.arraySize);
            for (var i = 0; i < length; i++)
            {
                EditorGUILayout.PropertyField(m_PointsProperty.GetArrayElementAtIndex(i));
            }
        }
        if (GUILayout.Button("+")) lineRenderer.AddPoint();
        if (GUILayout.Button("-")) lineRenderer.RemoveLastPoint();

        serializedObject.ApplyModifiedProperties();
    }
}