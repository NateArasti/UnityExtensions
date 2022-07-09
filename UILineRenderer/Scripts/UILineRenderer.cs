using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
public class UILineRenderer : Graphic
{
    /// <summary>
    /// Use this list to get access to points to move them
    /// Don't change this list manually, use AddPoint and RemovePoint methods instead
    /// </summary>
    [HideInInspector] public List<Transform> Points = new();
    private readonly List<UILineRendererPointHandler> _pointHandlers = new();

    [Space(20f)]
    [SerializeField] private float _thickness = 10;
    [SerializeField, Range(1, 100)] private float _resolution = 10;
    [SerializeField] private bool _fillGaps = true;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _selectNew;
    [SerializeField] private GameObject _vertexPrefab;
    private bool _vertexPrefabSet;
    private readonly Vector2[] _connectionPoints = new Vector2[4];

    [MenuItem("GameObject/UI/Extensions/UI Line Renderer")]
    public static void CreateUILineRenderer()
    {
        var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        new GameObject("UILineRenderer", typeof(UILineRenderer)).transform.SetParent(canvas.transform);
    }

    /// <summary>
    /// Adds point at the end of the list and returns the transform if it
    /// </summary>
    /// <returns></returns>
    public Transform AddPoint()
    {
        var newPosition = Points.Count > 0
            ? Points[^1].position + new Vector3(5 * _thickness, 5 * _thickness, 0)
            : Vector3.zero;
        return AddPoint(newPosition);
    }

    /// <summary>
    /// Adds point at the end of the list with given position and returns the transform if it
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Transform AddPoint(Vector3 position)
    {
        var newPoint = new GameObject(
            $"Point {Points.Count + 1}", 
            typeof(RectTransform)).transform;
        newPoint.position = position;
        newPoint.SetParent(transform);
        newPoint.SetAsLastSibling();
        Points.Add(newPoint);
        _pointHandlers.Add(newPoint.gameObject.AddComponent<UILineRendererPointHandler>());
        _pointHandlers[^1].Init(Points.Count - 1, RemovePointFromList);
        OnValidate();
        if (_vertexPrefabSet)
            SetVerticesPrefabs();
        if (_selectNew)
            Selection.activeTransform = newPoint;
        return newPoint;
    }
    
    /// <summary>
    /// Removes last point
    /// </summary>
    public void RemoveLastPoint()
    {
        RemovePoint(Points.Count - 1);
    }

    /// <summary>
    /// Removes point with the given index
    /// </summary>
    public void RemovePoint(int index)
    {
#if UNITY_EDITOR
        DestroyImmediate(Points[index].gameObject);
#else
        Destroy(_points[index].gameObject);
#endif
    }

    private void RemovePointFromList(int index)
    {
        Points.RemoveAt(index);
        _pointHandlers.RemoveAt(index);
        for (var i = index; i < Points.Count; ++i)
        {
            Points[i].name = $"Point {i + 1}";
            _pointHandlers[i].Index = i;
        }
        OnValidate();
    }

    public void SetVerticesPrefabs()
    {
        if (_vertexPrefab == null) return;
        DeleteVerticesPrefabs();
        foreach (var point in Points)
        {
            Instantiate(_vertexPrefab, point.position, Quaternion.identity)
                .transform.SetParent(point);
        }

        _vertexPrefabSet = true;
    }

    public void DeleteVerticesPrefabs()
    {
        foreach (var point in Points)
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

        _vertexPrefabSet = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        if (Points.Count < 2) return;

        for (var i = 0; i < Points.Count - 1; ++i)
        {
            CreateLineBetweenPoints(vh, 
                Points[i].transform.position, 
                Points[i + 1].transform.position, i == 0);
        }

        if(_loop)
        {
            CreateLineBetweenPoints(vh,
                Points[^1].transform.position,
                Points[0].transform.position);
            CreateLineBetweenPoints(vh,
                Points[0].transform.position,
                Points[1].transform.position);
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
        _connectionPoints[2] = vertex.position;
        vh.AddVert(vertex);
        vertex.position = firstPoint + offsetRight;
        if (_fillGaps) vertex.position += (Vector3) offsetFromCenter;
        _connectionPoints[3] = vertex.position;
        vh.AddVert(vertex);

        if (_fillGaps && !corner)
        {
            FillGaps(vh);
        }

        vertex.position = secondPoint + offsetLeft;
        if (_fillGaps) vertex.position -= (Vector3) offsetFromCenter;
        _connectionPoints[0] = vertex.position;
        vh.AddVert(vertex);
        vertex.position = secondPoint + offsetRight;
        if (_fillGaps) vertex.position -= (Vector3) offsetFromCenter;
        _connectionPoints[1] = vertex.position;
        vh.AddVert(vertex);

        var currentLastIndex = vh.currentVertCount - 1;
        vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
        vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
    }

    private void FillGaps(VertexHelper vh)
    {
        var a1 = _connectionPoints[1].y - _connectionPoints[0].y;
        var b1 = _connectionPoints[0].x - _connectionPoints[1].x;
        var c1 = -_connectionPoints[0].x * _connectionPoints[1].y + _connectionPoints[0].y * _connectionPoints[1].x;

        var a2 = _connectionPoints[3].y - _connectionPoints[2].y;
        var b2 = _connectionPoints[2].x - _connectionPoints[3].x;
        var c2 = -_connectionPoints[2].x * _connectionPoints[3].y + _connectionPoints[2].y * _connectionPoints[3].x;

        if (Mathf.Approximately(a1 * b2 - a2 * b1, 0))
        {
            var currentLastIndex = vh.currentVertCount - 1;
            vh.AddTriangle(currentLastIndex, currentLastIndex - 1, currentLastIndex - 3);
            vh.AddTriangle(currentLastIndex, currentLastIndex - 2, currentLastIndex - 3);
        }
        else
        {
            var center = new Vector2((b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1), (a2 * c1 - a1 * c2) / (a1 * b2 - a2 * b1));
            var firstPoints = EvaluateSlerpPoints(_connectionPoints[0], _connectionPoints[2], center);
            var secondPoints = EvaluateSlerpPoints(_connectionPoints[1], _connectionPoints[3], center);

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
