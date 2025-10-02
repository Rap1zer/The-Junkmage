using UnityEngine;

public class SelectedItem
{
    public GameObject Obj { get; set; }
    public CellPos? Index { get; set; }

    public ItemUIType? Type => Obj != null || Item != null ? Item.UIType : null;
    public ItemBase Item => Obj != null ? Obj.GetComponent<ItemBase>() : null;

    public void Clear()
    {
        Obj = null;
        Index = null;
    }
}
