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

    public void RenderChestItems(ItemData[] chestItems)
    {
        for (int i = 0; i < chestItems.Length; i++)
        {
            if (chestItems[i] == null) continue;

            GameObject item = Object.Instantiate(chestItems[i].prefab, canvas.transform);
            item.GetComponent<ItemBase>().Initialise(chestItems[i]);
            item.GetComponent<RectTransform>().anchoredPosition = chestSlots[i].anchoredPosition;

            DraggableItem draggable = item.GetComponent<DraggableItem>();
            draggable.SetItemUIType(ItemUIType.Chest);
            draggable.Index = new Vector2Int(0, i);
        }
    }
}