using UnityEngine;

public class Door
{
    public Room FromRoom { get; private set; }
    public Room ToRoom { get; private set; }

    private GameObject doorObj;

    public Door(Room fromRoom, Room toRoom, GameObject doorObj)
    {
        this.FromRoom = fromRoom;
        this.ToRoom = toRoom;
        this.doorObj = doorObj;
        doorObj.SetActive(true);
    }

    public void OpenDoor()
    {
        if (!FromRoom.Cleared)
        {
            Debug.LogWarning("Cannot open door: fromRoom is not cleared.");
            return;
        }
        doorObj.SetActive(false);
    }
}
