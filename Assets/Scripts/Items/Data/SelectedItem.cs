using UnityEngine;

public class SelectedItem
{
    public GameObject Obj { get; set; }
    public CellPos? Index { get; set; }

    public StorageType? Type => Obj != null || Item != null ? Item.StorageType : null;
    public ItemBase Item => Obj != null ? Obj.GetComponent<ItemBase>() : null;

    public void Clear()
    {
        Obj = null;
        Index = null;
    }
}
