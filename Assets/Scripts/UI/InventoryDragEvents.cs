using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InventoryDragEvents
{
    public static event Action<GameObject, PointerEventData> OnBeginDrag;
    public static event Action<GameObject, PointerEventData> OnDrag;
    public static event Action<GameObject, PointerEventData> OnEndDrag;

    public static void RaiseBeginDrag(GameObject obj, PointerEventData data)
        => OnBeginDrag?.Invoke(obj, data);

    public static void RaiseDrag(GameObject obj, PointerEventData data)
        => OnDrag?.Invoke(obj, data);

    public static void RaiseEndDrag(GameObject obj, PointerEventData data)
        => OnEndDrag?.Invoke(obj, data);
}
