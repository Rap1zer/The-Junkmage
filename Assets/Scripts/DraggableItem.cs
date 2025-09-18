using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemUIType itemType = ItemUIType.Chest;
    public ItemUIType ItemType => itemType;

    private RectTransform rt;
    private Canvas canvas;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItemUIType(ItemUIType type)
    {
        itemType = type;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryUI.Instance.BeginDrag(eventData, this, ItemType);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryUI.Instance.Drag(eventData);
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryUI.Instance.EndDrag(eventData);
    }
}

