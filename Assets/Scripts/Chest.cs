using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    private Inventory inventory;
    private GameObject itemDropsContainer;

    private bool inPlayerRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GameObject.Find("Game Manager").GetComponent<Inventory>();

        Transform canvas = GameObject.Find("Canvas").transform;
        itemDropsContainer = canvas.Find("Item Drops").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inPlayerRange) OpenChest();
        }
    }

    private void OpenChest()
    {
        if (!inPlayerRange) return;
        int itemPoolCount = 3;
        ItemData[] items = new ItemData[Mathf.Min(itemPoolCount, itemDatabase.items.Length)];

        for (int i = 0; i < items.Length; i++)
        {
            int random = Random.Range(0, itemDatabase.items.Length);
            items[i] = itemDatabase.items[random];
            Inventory.Instance.ChestItemsData[i] = items[i];
        }

        InventoryUI.Instance.RenderItems(items);
        inventory.OpenInventory();
    }

    private void CloseChest()
    {
        itemDropsContainer.SetActive(false);
        inventory.CloseInventory();
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
