using System;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class UILineRendererPointHandler : MonoBehaviour
{
    public int Index { get; set; }

    private bool _generated;
    private UnityAction<int> _onDestroyAction;

    public void Init(int index, UnityAction<int> onDestroyAction)
    {
        if(_generated) return;
        _generated = true;
        Index = index;
        _onDestroyAction = onDestroyAction;
    }

    private void Reset()
    {
        hideFlags = HideFlags.HideInInspector;
    }

    private void OnDestroy()
    {
        if(_onDestroyAction != null)
            _onDestroyAction.Invoke(Index);
    }
}