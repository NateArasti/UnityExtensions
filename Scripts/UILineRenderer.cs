using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    [Space(20f)]
    [SerializeField] private float _thickness;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _selectNew;
    [SerializeField] private GameObject _vertexPrefab;
    [HideInInspector, SerializeField] private List<Transform> _points;

    public void AddPoint()
    {
        var newPoint = new GameObject($"{_points.Count + 1}", typeof(RectTransform)).transform;
        newPoint.Translate((Vector2)_points[_points.Count - 1].position + Vector2.one * _thickness);
        newPoint.SetParent(transform);
        newPoint.SetAsLastSibling();
        _points.Add(newPoint);
        OnValidate();
        SetVerticesPrefabs();
        if(_selectNew)
            Selection.activeTransform = newPoint;
    }

    public void RemovePoint()
    {
#if UNITY_EDITOR
        DestroyImmediate(_points[_points.Count - 1].gameObject);
#else
        Destroy(_points[_points.Count - 1].gameObject);
#endif
        _points.RemoveAt(_points.Count - 1);
        OnValidate();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if(_points.Count < 2) return;

        for (var i = 0; i < _points.Count - 1; ++i)
        {
            CreateLineBetweenPoints(vh, 
                _points[i].transform.position, 
                _points[i + 1].transform.position);
        }

        if(_loop)
        {
            CreateLineBetweenPoints(vh,
                _points[_points.Count - 1].transform.position,
                _points[0].transform.position);
        }
    }

    public void SetVerticesPrefabs()
    {
        if (_vertexPrefab != null)
        {
            DeleteVerticesPrefabs();
            foreach (var point in _points)
            {
                Instantiate(_vertexPrefab, point.position, Quaternion.identity)
                    .transform.SetParent(point);
            }
        }
    }

    public void DeleteVerticesPrefabs()
    {
        foreach (var point in _points)
        {
            foreach (Transform child in point)
            {
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }
    }

    private void Update()
    {
        OnValidate();
    }

    private static float GetAngle(Vector3 v) => Mathf.Rad2Deg * Mathf.Atan2(v.y, v.x);

    private void CreateLineBetweenPoints(VertexHelper vh, Vector2 firstPoint, Vector2 secondPoint)
    {
        var vertex = UIVertex.simpleVert;
        vertex.color = color;

        var direction = firstPoint - secondPoint;
        var angle = GetAngle(direction);
        var d = 0.5f * _thickness;

        var offsetLeft = new Vector2(
            d * Mathf.Cos(Mathf.Deg2Rad * (90 + angle)),
            d * Mathf.Sin(Mathf.Deg2Rad * (90 + angle)));
        var offsetRight = new Vector2(
            d * Mathf.Cos(Mathf.Deg2Rad * (-90 + angle)),
            d * Mathf.Sin(Mathf.Deg2Rad * (-90 + angle)));

        vertex.position = firstPoint + offsetLeft;
        vh.AddVert(vertex);
        vertex.position = firstPoint + offsetRight;
        vh.AddVert(vertex);

        vertex.position = secondPoint + offsetLeft;
        vh.AddVert(vertex);
        vertex.position = secondPoint + offsetRight;
        vh.AddVert(vertex);

        var currentLastIndex = vh.currentVertCount - 1;
        vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
        vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
    }
}
