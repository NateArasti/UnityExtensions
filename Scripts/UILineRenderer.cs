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
    [SerializeField, Range(1, 100)] private float _resolution = 10;
    [SerializeField] private bool _fillGaps = true;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _selectNew;
    [SerializeField] private GameObject _vertexPrefab;
    [HideInInspector] public List<Transform> _points = new();
    private bool m_VertexPrefabSet;
    private readonly Vector2[] m_ConnectionPoints = new Vector2[4];

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
        if(m_VertexPrefabSet)
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

    public void RemoveLastPoint()
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

    public void SetVerticesPrefabs()
    {
        if (_vertexPrefab == null) return;
        DeleteVerticesPrefabs();
        foreach (var point in _points)
        {
            Instantiate(_vertexPrefab, point.position, Quaternion.identity)
                .transform.SetParent(point);
        }

        m_VertexPrefabSet = true;
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

        m_VertexPrefabSet = false;
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
            CreateLineBetweenPoints(vh,
                _points[0].transform.position,
                _points[1].transform.position);
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
        m_ConnectionPoints[2] = vertex.position;
        vh.AddVert(vertex);
        vertex.position = firstPoint + offsetRight;
        if (_fillGaps) vertex.position += (Vector3) offsetFromCenter;
        m_ConnectionPoints[3] = vertex.position;
        vh.AddVert(vertex);

        if (_fillGaps && !corner)
        {
            FillGaps(vh);
        }

        vertex.position = secondPoint + offsetLeft;
        if (_fillGaps) vertex.position -= (Vector3) offsetFromCenter;
        m_ConnectionPoints[0] = vertex.position;
        vh.AddVert(vertex);
        vertex.position = secondPoint + offsetRight;
        if (_fillGaps) vertex.position -= (Vector3) offsetFromCenter;
        m_ConnectionPoints[1] = vertex.position;
        vh.AddVert(vertex);

        var currentLastIndex = vh.currentVertCount - 1;
        vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
        vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
    }

    private void FillGaps(VertexHelper vh)
    {
        var a1 = m_ConnectionPoints[1].y - m_ConnectionPoints[0].y;
        var b1 = m_ConnectionPoints[0].x - m_ConnectionPoints[1].x;
        var c1 = -m_ConnectionPoints[0].x * m_ConnectionPoints[1].y + m_ConnectionPoints[0].y * m_ConnectionPoints[1].x;

        var a2 = m_ConnectionPoints[3].y - m_ConnectionPoints[2].y;
        var b2 = m_ConnectionPoints[2].x - m_ConnectionPoints[3].x;
        var c2 = -m_ConnectionPoints[2].x * m_ConnectionPoints[3].y + m_ConnectionPoints[2].y * m_ConnectionPoints[3].x;

        if (Mathf.Approximately(a1 * b2 - a2 * b1, 0))
        {
            var currentLastIndex = vh.currentVertCount - 1;
            vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
            vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
        }
        else
        {
            var center = new Vector2((b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1), (a2 * c1 - a1 * c2) / (a1 * b2 - a2 * b1));
            var firstPoints = EvaluateSlerpPoints(m_ConnectionPoints[0], m_ConnectionPoints[2], center);
            var secondPoints = EvaluateSlerpPoints(m_ConnectionPoints[1], m_ConnectionPoints[3], center);

            var vertex = UIVertex.simpleVert;
            vertex.color = color;
            for (var i = 0; i < firstPoints.Count - 1; i += 1)
            {
                vertex.position = firstPoints[i];
                vh.AddVert(vertex);
                vertex.position = secondPoints[i];
                vh.AddVert(vertex);
                vertex.position = firstPoints[i + 1];
                vh.AddVert(vertex);
                vertex.position = secondPoints[i + 1];
                vh.AddVert(vertex);

                var currentLastIndex = vh.currentVertCount - 1;
                vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
                vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
            }
        }
    }

    private List<Vector3> EvaluateSlerpPoints(Vector3 start, Vector3 end, Vector3 center)
    {
        var result = new List<Vector3>();
        var startRelativeCenter = start - center;
        var endRelativeCenter = end - center;

        var f = 1f / _resolution;

        for (var i = 0f; i < 1 + f; i += f)
        {
            result.Add(Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + center);
        }

        return result;
    }
}
