using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemUIType
{
    Chest,
    Inventory
}


public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    private GameObject currentItem;
    private RectTransform currentRT;
    private ItemUIType? currentType;

    private GameObject[,] cells;

    [SerializeField] private Canvas canvas;
    private GraphicRaycaster raycaster;

    [SerializeField] private GameObject cellPrefab;
    private RectTransform[] chestItemPos;
    private GameObject itemDropsContainer;

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

        raycaster = canvas.GetComponent<GraphicRaycaster>();
        itemDropsContainer = canvas.transform.Find("Item Drops").gameObject;
        chestItemPos = new RectTransform[Chest.itemPoolCount];
        for (int i = 0; i < chestItemPos.Length; i++)
        {
            chestItemPos[i] = itemDropsContainer.transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    public void BeginDrag(PointerEventData eventData, ItemUIType type)
    {
        currentItem = eventData.pointerDrag;
        currentType = type;
        currentRT = currentItem.GetComponent<RectTransform>();
    }

    public void Drag(PointerEventData eventData)
    {
        foreach (var cell in cells)
        {
            cell.GetComponent<Image>().color = Color.white;
        }

        // Vector2 localMousePos;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     canvas.transform as RectTransform,
        //     currentRT.position,
        //     canvas.worldCamera,
        //     out localMousePos
        // );
        // Debug.Log(localMousePos);

        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        RectTransform itemRT = currentItem.transform.Find("Pivot").GetComponent<RectTransform>();

        Vector3 itemPos = canvasRT.InverseTransformPoint(itemRT.position);
        Debug.Log(itemPos);

        Vector2Int nearestCell = GetNearestGridPosition(itemPos);

        Vector2Int itemShape = Inventory.Instance.ChestItemsData[currentItem.GetComponent<DraggableItem>().Index.y].size;

        if (CanPlaceItem(itemShape, nearestCell))
        {
            int itemWidth = itemShape.x;
            int itemHeight = itemShape.y;

            for (int y = 0; y < itemHeight; y++)
            {
                for (int x = 0; x < itemWidth; x++)
                {
                    cells[nearestCell.x + x, nearestCell.y + y].GetComponent<Image>().color = Color.green;
                }
            }
        }
        else
        {
            // optionally highlight red if it doesn't fit
        }
    }

    public void EndDrag(PointerEventData eventData)
    {
        currentItem = null;
        currentType = null;
        currentRT = null;
    }

    public void DrawGrid(Transform gridContainer)
    {
        Inventory inventory = Inventory.Instance;
        cells = new GameObject[inventory.width, inventory.height];

        float cellLength = 100;
        float margin = 20;
        float offsetW = ((cellLength * inventory.width) + (margin * (inventory.width - 1))) / 2;
        float offsetH = ((cellLength * inventory.width) + (margin * (inventory.height - 1))) / 2;

        for (int y = 0; y < inventory.height; y++)
        {
            for (int x = 0; x < inventory.width; x++)
            {
                GameObject gridCell = Instantiate(cellPrefab, gridContainer);
                cells[x, y] = gridCell;

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(((cellLength + margin) * x) - offsetW,
                ((cellLength + margin) * y) - offsetH, 0);
            }
        }
    }

    public void RenderItems()
    {
        ItemData[] items = Inventory.Instance.ChestItemsData;

        for (int i = 0; i < Inventory.Instance.ChestItemsData.Length; i++)
        {
            if (items[i] == null) continue;
            GameObject item = Instantiate(items[i].prefab, canvas.transform);
            item.GetComponent<RectTransform>().anchoredPosition = chestItemPos[i].anchoredPosition;

            DraggableItem draggableItem = item.GetComponent<DraggableItem>();
            draggableItem.SetItemUIType(ItemUIType.Chest);
            draggableItem.Index = new Vector2Int(0, i);
        }

        itemDropsContainer.SetActive(true);
    }
    
    private Vector2Int GetNearestGridPosition(Vector2 mousePos)
    {
        float cellLength = 100;
        float margin = 20;
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        // Calculate offset (same as in DrawGrid)
        float offsetW = ((cellLength * width) + (margin * (width - 1))) / 2;
        float offsetH = ((cellLength * height) + (margin * (height - 1))) / 2;

        float localX = mousePos.x + offsetW;
        float localY = mousePos.y + offsetH;

        int gridX = Mathf.Clamp(Mathf.RoundToInt(localX / (cellLength + margin)), 0, width - 1);
        int gridY = Mathf.Clamp(Mathf.RoundToInt(localY / (cellLength + margin)), 0, height - 1);

        return new Vector2Int(gridX, gridY);
    }

    
    private bool CanPlaceItem(Vector2Int shape, Vector2Int nearestCell)
    {
        int itemWidth = shape.x;
        int itemHeight = shape.y;

        for (int y = 0; y < itemHeight; y++)
        {
            for (int x = 0; x < itemWidth; x++)
            {
                int cellX = nearestCell.x + x;
                int cellY = nearestCell.y + y;

                if (cellX >= cells.GetLength(0) || cellY >= cells.GetLength(1))
                    return false;

                // Optional: check if the cell is already occupied
                // if (Inventory.Instance.IsCellOccupied(cellX, cellY)) return false;
            }
        }

        return true;
    }
}

