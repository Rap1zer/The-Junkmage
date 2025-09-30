using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    public int CurrentRoomIndex { get; private set; }

    public event System.Action<int> OnPlayerEnterRoom;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetPlayerRoom(int roomIndex)
    {
        if (CurrentRoomIndex == roomIndex) return;

        CurrentRoomIndex = roomIndex;
        OnPlayerEnterRoom?.Invoke(roomIndex);
    }
}
