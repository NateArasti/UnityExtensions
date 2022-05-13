using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
public class UILineRenderer : Graphic
{
    [Space(20f)]
    [SerializeField] private float _thickness = 10;
    [SerializeField] private bool _fillGaps = true;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _selectNew;
    [SerializeField] private GameObject _vertexPrefab;
    [HideInInspector] public List<Transform> _points = new();

    public Transform AddPoint()
    {
        var newPoint = new GameObject(
            $"Point {_points.Count + 1}",
            typeof(RectTransform)).transform;
        if(_points.Count > 0)
            newPoint.Translate((Vector2)_points[^1].position + Vector2.one * 5 * _thickness);
        else
            newPoint.position = Vector3.zero;
        newPoint.SetParent(transform);
        newPoint.SetAsLastSibling();
        _points.Add(newPoint);
        OnValidate();
        SetVerticesPrefabs();
        if(_selectNew)
            Selection.activeTransform = newPoint;
        return newPoint;
    }

    public Transform AddPoint(Vector3 position)
    {
        var newPoint = new GameObject(
            $"{_points.Count + 1}", 
            typeof(RectTransform)).transform;
        newPoint.position = position;
        newPoint.SetParent(transform);
        newPoint.SetAsLastSibling();
        _points.Add(newPoint);
        OnValidate();
        SetVerticesPrefabs();
        if(_selectNew)
            Selection.activeTransform = newPoint;
        return newPoint;
    }

    public void RemovePoint()
    {
#if UNITY_EDITOR
        DestroyImmediate(_points[^1].gameObject);
#else
        Destroy(_points[^1].gameObject);
#endif
        _points.RemoveAt(_points.Count - 1);
        OnValidate();
    }

    public void RemovePoint(int index)
    {
#if UNITY_EDITOR
        DestroyImmediate(_points[index].gameObject);
#else
        Destroy(_points[index].gameObject);
#endif
        _points.RemoveAt(index);
        for (var i = index; i < _points.Count; ++i)
        {
            _points[i].name = $"Point {i + 1}";
        }
        OnValidate();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        if (_points.Count < 2) return;

        for (var i = 0; i < _points.Count - 1; ++i)
        {
            CreateLineBetweenPoints(vh, 
                _points[i].transform.position, 
                _points[i + 1].transform.position, i == 0);
        }

        if(_loop)
        {
            CreateLineBetweenPoints(vh,
                _points[^1].transform.position,
                _points[0].transform.position);

            var currentLastIndex = vh.currentVertCount - 1;
            vh.AddTriangle(currentLastIndex, currentLastIndex - 1, 0);
            vh.AddTriangle(0, 1, currentLastIndex);
        }
    }

    public void SetVerticesPrefabs()
    {
        if (_vertexPrefab == null) return;
        DeleteVerticesPrefabs();
        foreach (var point in _points)
        {
            Instantiate(_vertexPrefab, point.position, Quaternion.identity)
                .transform.SetParent(point);
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
    protected override void Start()
    {
        base.Start();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        AddPoint();
        AddPoint();
    }

    private void Update()
    {
        OnValidate();
    }

    private static float GetAngle(Vector3 v) => Mathf.Rad2Deg * Mathf.Atan2(v.y, v.x);

    private void CreateLineBetweenPoints(VertexHelper vh, Vector2 firstPoint, Vector2 secondPoint, bool corner = false)
    {
        var vertex = UIVertex.simpleVert;
        vertex.color = color;

        var direction = secondPoint - firstPoint;
        var angle = GetAngle(direction);
        var d = 0.5f * _thickness;

        var offsetLeft = new Vector2(
            d * Mathf.Cos(Mathf.Deg2Rad * (90 + angle)),
            d * Mathf.Sin(Mathf.Deg2Rad * (90 + angle)));
        var offsetRight = new Vector2(
            d * Mathf.Cos(Mathf.Deg2Rad * (-90 + angle)),
            d * Mathf.Sin(Mathf.Deg2Rad * (-90 + angle)));

        var offsetFromCenter = 
            direction.normalized * Mathf.Clamp(direction.magnitude, 0, 0.5f * _thickness);

        vertex.position = firstPoint + offsetLeft;
        if (_fillGaps) vertex.position += (Vector3) offsetFromCenter;
        vh.AddVert(vertex);
        vertex.position = firstPoint + offsetRight;
        if (_fillGaps) vertex.position += (Vector3) offsetFromCenter;
        vh.AddVert(vertex);

        var currentLastIndex = vh.currentVertCount - 1;

        if (_fillGaps && !corner)
        {
            vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
            vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
        }

        vertex.position = secondPoint + offsetLeft;
        if (_fillGaps) vertex.position -= (Vector3) offsetFromCenter;
        vh.AddVert(vertex);
        vertex.position = secondPoint + offsetRight;
        if (_fillGaps) vertex.position -= (Vector3) offsetFromCenter;
        vh.AddVert(vertex);

        currentLastIndex = vh.currentVertCount - 1;
        vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
        vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
    }
}
