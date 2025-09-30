using UnityEngine;

public class SelectedItem
{
    public GameObject Obj { get; set; }
    public CellPos? Index { get; set; }

    public ItemUIType? Type => Obj != null || Item != null ? Item.UIType : null;
    public IItem Item => Obj != null ? Obj.GetComponent<IItem>() : null;

    public void Clear()
    {
        Obj = null;
        Index = null;
    }
}
