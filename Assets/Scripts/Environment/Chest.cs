using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public static int itemPoolCount = 3;

    public ItemDatabase itemDatabase;

    private bool inPlayerRange = false;
    private bool chestOpened = false;

    public ItemData[] ItemsInChest { get; private set; } = new ItemData[itemPoolCount];
    public int ItemsTaken { get; private set; }

    // Dictionary to track items: Key = Item ID, Value = taken or not
    public Dictionary<Guid, bool> chestItems { get; private set; } = new Dictionary<Guid, bool>();

    // Event fired when chest opens
    public event Action<Chest> OnChestOpened;
    public event Action OnChestClosed;

    void Start()
    {
        InventoryManager.Instance.RegisterChest(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inPlayerRange && !chestOpened)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (!inPlayerRange) return;
        chestOpened = true;

        int itemCount = Mathf.Min(itemPoolCount, itemDatabase.items.Length);
        ItemsInChest = new ItemData[itemCount];
        chestItems.Clear(); // Reset the dictionary

        // Randomly select items from database
        for (int i = 0; i < itemCount; i++)
        {
            int random = UnityEngine.Random.Range(0, itemDatabase.items.Length);
            ItemsInChest[i] = itemDatabase.items[random];
        }

        OnChestOpened?.Invoke(this);
    }

    public void CloseChest()
    {
        OnChestClosed?.Invoke();
    }

    public void SetItemIds(IItem[] items)
    {
        if (!chestOpened) return;

        chestItems.Clear();
        foreach (var item in items)
        {
            chestItems[item.Id] = false; // initialize as not taken
        }
    }

    public bool CanTakeItem(IItem item) => chestItems.ContainsKey(item.Id) && !chestItems[item.Id];

    public void TakeItem(IItem item)
    {
        if (CanTakeItem(item))
        {
            chestItems[item.Id] = true; // mark item as taken
            ItemsTaken++;
        }
    }

    public bool IsItemTaken(Guid id)
    {
        return chestItems.ContainsKey(id) && chestItems[id];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) inPlayerRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) inPlayerRange = false;
    }
}