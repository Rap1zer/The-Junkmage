using System;
using UnityEngine;

public class ChestUI
{
    private RectTransform[] chestSlots;
    private GameObject chestContainer;
    private GameObject[] itemObjs;
    private ItemBase[] items;

    public ChestUI(RectTransform[] chestSlots, GameObject chestContainer)
    {
        this.chestSlots = chestSlots;
        this.chestContainer = chestContainer;
    }

    public void ClearChestItems()
    {
        for (int i = 0; i < itemObjs.Length; i++)
        {
            if (itemObjs[i] != null && items[i].StorageType == StorageType.Chest) UnityEngine.Object.Destroy(itemObjs[i]);
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
        obj = UnityEngine.Object.Instantiate(data.prefab, chestContainer.transform);

        // Initialise the component with the item data
        ItemBase itemBase = obj.GetComponent<ItemBase>();
        itemBase.Initialise(data);

        // Place it in the correct UI slot
        obj.GetComponent<RectTransform>().anchoredPosition = pos;

        return obj.GetComponent<ItemBase>();
    }
    
    public bool SnapItemBackToChest(ItemBase item)
    {
        for (int i = 0; i < chestSlots.Length; i++)
        {
            RectTransform slot = chestSlots[i];

            // Check if dropped over this slot
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, item.transform.position);
            if (RectTransformUtility.RectangleContainsScreenPoint(slot, screenPoint))
            {
                RectTransform itemRT = item.GetComponent<RectTransform>();
                itemRT.SetParent(chestContainer.transform, worldPositionStays: false);
                itemRT.anchoredPosition = slot.anchoredPosition;

                return true;
            }
        }

        return false;
    }
}