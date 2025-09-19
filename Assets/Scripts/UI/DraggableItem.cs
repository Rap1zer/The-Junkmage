using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsDragging { get; private set; } = false;

    [SerializeField] private ItemUIType itemType = ItemUIType.Chest;
    
    public ItemUIType ItemType => itemType;
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

    public void SetItemUIType(ItemUIType type)
    {
        itemType = type;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.UI.BeginDrag(eventData, ItemType, Index);
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false; // UNNECESSARY CODE RIGHT NOW
        IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryManager.UI.Drag(eventData);
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryManager.UI.EndDrag(eventData);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true; // UNNECESSARY CODE RIGHT NOW
        IsDragging = false;
    }
}

