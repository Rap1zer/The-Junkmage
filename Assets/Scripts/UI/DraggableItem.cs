using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsDragging { get; private set; } = false;
    public Vector2Int Index { get; set; }

    private RectTransform rt;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        IsDragging = true;

        InventoryDragEvents.RaiseBeginDrag(gameObject, Index, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;

        InventoryDragEvents.RaiseDrag(gameObject, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        IsDragging = false;

        InventoryDragEvents.RaiseEndDrag(gameObject, eventData);
    }
}
