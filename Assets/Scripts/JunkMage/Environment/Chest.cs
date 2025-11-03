using System;
using System.Collections.Generic;
using System.Linq;
using JunkMage.Systems;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public static int itemPoolCount = 3;

    public ItemDatabase itemDatabase;

    private bool inPlayerRange;
    private bool chestOpened;

    public ItemData[] ItemsInChest { get; private set; } = new ItemData[itemPoolCount];
    public int ItemsTaken { get; private set; }

    // Dictionary to track items: Key = Item ID, Value = taken or not
    public Dictionary<Guid, bool> ChestItems { get; } = new();

    // Event fired when chest opens
    public event Action<Chest> OnChestOpened;
    public event Action OnChestClosed;

    void Start()
    {
        InventoryPresenter inventory = GameObject.Find("Game Manager").GetComponent<InventoryPresenter>();
        inventory.RegisterChest(this);
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
        ChestItems.Clear(); // Reset the dictionary

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

    public void SetItemIds(ItemBase[] items)
    {
        if (!chestOpened) return;

        ChestItems.Clear();
        foreach (var item in items)
        {
            ChestItems[item.Id] = false; // initialize as not taken
        }
    }

    public bool CanTakeItem(ItemBase item) => ChestItems.ContainsKey(item.Id) && !ChestItems[item.Id];

    public void TakeItem(ItemBase item)
    {
        if (CanTakeItem(item))
        {
            ChestItems[item.Id] = true; // mark item as taken
            ItemsTaken++;
        }
    }
    
    public void UndoTakeItem(ItemBase item)
    {
        if (ChestItems.ContainsKey(item.Id) && ChestItems[item.Id])
        {
            ChestItems[item.Id] = false;
            ItemsTaken = Mathf.Max(0, ItemsTaken - 1);
        }
    }

    public bool IsItemTaken(Guid id)
    {
        return ChestItems.ContainsKey(id) && ChestItems[id];
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