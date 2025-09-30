using UnityEngine;

public class Door : MonoBehaviour
{
    public Room FromRoom { get; private set; }
    public Room ToRoom { get; private set; }

    private GameObject doorObj;
    
    public void Initialize(Room fromRoom, Room toRoom, GameObject doorObj)
    {
        FromRoom = fromRoom;
        ToRoom = toRoom;
        this.doorObj = doorObj;

        doorObj.SetActive(true);

        // Rename the GameObject in the hierarchy
        string fromIndex = (fromRoom == null) ? "null" : fromRoom.Index.ToString();
        string toIndex = (toRoom == null) ? "null" : toRoom.Index.ToString();
        doorObj.name = $"Door{fromIndex}to{toIndex}";
    }

    public void OpenDoor()
    {
        if (!FromRoom.Cleared)
        {
            Debug.LogWarning("Cannot open door: fromRoom is not cleared.");
            return;
        }

        doorObj.GetComponent<SpriteRenderer>().enabled = false;
        doorObj.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int newRoomIndex = RoomManager.Instance.CurrentRoomIndex == FromRoom.Index
                ? ToRoom.Index
                : FromRoom.Index;

            RoomManager.Instance.SetPlayerRoom(newRoomIndex);
        }
    }
}
