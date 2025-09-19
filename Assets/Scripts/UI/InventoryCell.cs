using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedItem = eventData.pointerDrag;

        if (draggedItem != null)
        {
            Debug.Log(draggedItem.name + " dropped onto " + gameObject.name);
            
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.localPosition = Vector3.zero; // snap to slot
        }
    }
}
