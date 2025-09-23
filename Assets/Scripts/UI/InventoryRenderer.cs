using UnityEngine;

public class InventoryRenderer
{
    private Canvas canvas;
    private RectTransform[] chestSlots;

    public InventoryRenderer(Canvas canvas, RectTransform[] chestSlots)
    {
        this.canvas = canvas;
        this.chestSlots = chestSlots;
    }

    public IItem[] RenderChestItems(ItemData[] chestItems)
    {
        IItem[] items = new IItem[chestItems.Length];

        for (int i = 0; i < chestItems.Length; i++)
        {
            if (chestItems[i] == null) continue;

            GameObject itemObj = Object.Instantiate(chestItems[i].prefab, canvas.transform);
            itemObj.GetComponent<ItemBase>().Initialise(chestItems[i]);
            itemObj.GetComponent<RectTransform>().anchoredPosition = chestSlots[i].anchoredPosition;
            items[i] = itemObj.GetComponent<IItem>();

            DraggableItem draggable = itemObj.GetComponent<DraggableItem>();
            draggable.Index = new Vector2Int(0, i);
        }

        return items;
    }
}