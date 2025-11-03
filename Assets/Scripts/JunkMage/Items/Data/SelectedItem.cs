using UnityEngine;

public class SelectedItem
{
    public ItemBase Item { get; set; }

    public void Clear()
    {
        Item = null;
    }
}
