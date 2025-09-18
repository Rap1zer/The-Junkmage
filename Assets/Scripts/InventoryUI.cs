using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemUIType
{
    Chest,
    Inventory
}


public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    private DraggableItem currentItem;
    private ItemUIType currentType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BeginDrag(PointerEventData eventData, DraggableItem draggableItem, ItemUIType type)
    {
        currentType = type;
        currentItem = draggableItem;
    }

    public void Drag(PointerEventData eventData)
    {
        
    }

    public void EndDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }
}
