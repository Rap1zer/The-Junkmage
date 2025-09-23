using System;
using UnityEngine;

public class InventoryRenderer
{
    private Canvas canvas;
    private RectTransform[] chestSlots;
    private GameObject invContainer;
    private GameObject[] itemObjs;
    private IItem[] items;

    public InventoryRenderer(Canvas canvas, RectTransform[] chestSlots, GameObject invContainer)
    {
        this.canvas = canvas;
        this.chestSlots = chestSlots;
        this.invContainer = invContainer;
    }

    public void ClearChestItems()
    {
        for (int i = 0; i < itemObjs.Length; i++)
        {
            if (itemObjs[i] != null && items[i].UIType == ItemUIType.Chest) UnityEngine.Object.Destroy(itemObjs[i]);
        }
    }

    public IItem[] RenderChestItems(ItemData[] chestItems)
    {
        items = new IItem[chestItems.Length];
        itemObjs = new GameObject[chestItems.Length];

        for (int i = 0; i < chestItems.Length; i++)
        {
            if (chestItems[i] == null) continue;

            itemObjs[i] = UnityEngine.Object.Instantiate(chestItems[i].prefab, invContainer.transform);
            itemObjs[i].GetComponent<ItemBase>().Initialise(chestItems[i]);
            itemObjs[i].GetComponent<RectTransform>().anchoredPosition = chestSlots[i].anchoredPosition;
            items[i] = itemObjs[i].GetComponent<IItem>();

            DraggableItem draggable = itemObjs[i].GetComponent<DraggableItem>();
            draggable.Index = new Vector2Int(0, i);
        }

        return items;
    }
}