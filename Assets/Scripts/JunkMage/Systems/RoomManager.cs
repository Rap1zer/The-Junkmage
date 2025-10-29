using JunkMage.Environment;
using UnityEngine;

namespace JunkMage.Combat
{
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        public int CurrentRoomIndex { get; private set; }
    
        public Room[] rooms = new Room[6];
        public GameObject[] doorObjs = new GameObject[5];
        public int[] enemyCounts = new int[6];

        public event System.Action<int> OnPlayerEnterRoom;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initialise the room classes and their doors.
            for (int i = rooms.Length - 1; i >= 0; i--)
            {
                if (i == rooms.Length - 1)
                {
                    rooms[i].Initialise(i, enemyCounts[i], null, null);
                }
                else if (i == 0)
                {
                    rooms[i].Initialise(i, enemyCounts[i], rooms[1], doorObjs[0]);
                }
                else
                {
                    rooms[i].Initialise(i, enemyCounts[i], rooms[i + 1], doorObjs[i]);
                }
            }
        }

        public void SetPlayerRoom(int roomIndex)
        {
            if (CurrentRoomIndex == roomIndex) return;

            CurrentRoomIndex = roomIndex;
            OnPlayerEnterRoom?.Invoke(roomIndex);
        }
    }
}
