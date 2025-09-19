using System;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public static int itemPoolCount = 3;

    public ItemDatabase itemDatabase;
    private Inventory inventory;
    private GameObject itemDropsContainer;

    private bool inPlayerRange = false;
    private bool chestOpened = false;

    public ItemData[] ItemsInChest { get; private set; }

    // Event fired when chest opens
    public event Action<Chest> OnChestOpened;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = InventoryManager.Inventory;

        Transform canvas = GameObject.Find("Canvas").transform;
        itemDropsContainer = canvas.Find("Item Drops").gameObject;

        InventoryManager.Instance.RegisterChest(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inPlayerRange)
        {
            if (chestOpened) // ADD SECOND CHECK TO SEE IF ONE ITEM HAS BEEN TAKEN
                CloseChest();
            else
                OpenChest();
        }
    }

    private void OpenChest()
    {
        if (!inPlayerRange) return; // Too far away to open chest
        int itemCount = Mathf.Min(itemPoolCount, itemDatabase.items.Length); // Limit number of items
        ItemsInChest = new ItemData[itemCount]; // Reset Chest

        // Randomly select items from database
        for (int i = 0; i < itemCount; i++)
        {
            int random = UnityEngine.Random.Range(0, itemDatabase.items.Length);
            ItemsInChest[i] = itemDatabase.items[random];
        }

        OnChestOpened?.Invoke(this);
        InventoryManager.Instance.OpenInventory();
        chestOpened = true;
    }

    private void CloseChest()
    {
        OnChestOpened?.Invoke(null);
        InventoryManager.Instance.CloseInventory();
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
