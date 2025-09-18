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
    public GameObject cell;

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
    public void DrawGrid(Transform gridContainer)
    {
        Inventory inventory = Inventory.Instance;
        float cellLength = 100;
        float margin = 20;
        float offsetW = ((cellLength * inventory.width) + (margin * (inventory.width - 1))) / 2;
        float offsetH = ((cellLength * inventory.width) + (margin * (inventory.height - 1))) / 2;

        for (int y = 0; y < inventory.height; y++)
        {
            for (int x = 0; x < inventory.width; x++)
            {
                GameObject gridCell = Instantiate(cell, gridContainer);

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(((cellLength + margin) * x) - offsetW,
                ((cellLength + margin) * y) - offsetH, 0);
            }
        }
    }
}
