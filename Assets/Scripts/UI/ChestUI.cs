using System;
using UnityEngine;

public class ChestUI
{
    private RectTransform[] chestSlots;
    private GameObject invContainer;
    private GameObject[] itemObjs;
    private ItemBase[] items;

    public ChestUI(RectTransform[] chestSlots, GameObject invContainer)
    {
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

    public ItemBase[] RenderChestItems(ItemData[] chestItems)
    {
        items = new ItemBase[chestItems.Length];
        itemObjs = new GameObject[chestItems.Length];

        for (int i = 0; i < chestItems.Length; i++)
        {
            if (chestItems[i] == null) continue;

            items[i] = CreateItem(chestItems[i], chestSlots[i].anchoredPosition, out itemObjs[i]);
        }

        return items;
    }

    private ItemBase CreateItem(ItemData data, Vector2 pos, out GameObject obj)
    {
        obj = UnityEngine.Object.Instantiate(data.prefab, invContainer.transform);

        // Initialise the component with the item data
        ItemBase itemBase = obj.GetComponent<ItemBase>();
        itemBase.Initialise(data);

        // Place it in the correct UI slot
        obj.GetComponent<RectTransform>().anchoredPosition = pos;

        return obj.GetComponent<ItemBase>();
    }
}