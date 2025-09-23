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
        if (!InventoryManager.Instance.CanDrag(eventData.pointerDrag.GetComponent<IItem>())) return;
        InventoryManager.UI.BeginDrag(eventData, Index);
        canvasGroup.alpha = 0.7f;
        IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!InventoryManager.Instance.CanDrag(eventData.pointerDrag.GetComponent<IItem>())) return;
        InventoryManager.UI.Drag(eventData);
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!InventoryManager.Instance.CanDrag(eventData.pointerDrag.GetComponent<IItem>())) return;
        InventoryManager.UI.EndDrag(eventData);
        canvasGroup.alpha = 1f;
        IsDragging = false;
    }
}

