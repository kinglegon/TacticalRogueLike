using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    int roomSizeX = 15;
    int roomSizeY = 10;

    int roomCount;
    int roomCountCutoff = 3;

    [SerializeField] GameObject[] roomPrefabs;
    List<GameObject> spawnedRooms;

    // Start is called before the first frame update
    void Start()
    {
        roomCount = 0;
        spawnedRooms = new List<GameObject>();
        //creates a starting room with 2-4 doors
        GameObject staringRoom = 
            SpawnRoom(0, 0, "Bottom", 2, 4);

        List<GameObject> newlySpawnedRooms = 
            SpawnInDoorways(2, 4, staringRoom);

        foreach(GameObject room in newlySpawnedRooms)
        {
            SpawnInDoorways(1, 1, room);

        }
        
        
        //after they made the amount of doors before cuttoff close off all the rooms with open ends
        //SpawnInDoorways(1, 1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject SpawnRoom(int locationX, int locationY, string doorway, int minDoors, int maxDoors)
    {
        //makes a list of possible rooms based on which way the doorway connects
        List<GameObject> possibleRooms = getCompatableRooms(doorway, minDoors, maxDoors);
        //choses a room out of the possible rooms
        GameObject chosenRoom = possibleRooms[Random.Range(0, possibleRooms.Count - 1)];
        //adds room to the scene at desired location
        GameObject spawnedRoom = Instantiate(chosenRoom, new Vector3(locationX, locationY, 1), chosenRoom.transform.rotation);
        //adds the chosen room to the list of chosen rooms
        spawnedRooms.Add(spawnedRoom);
        //adds to the room count
        roomCount++;

        return spawnedRoom;
    }

    List<GameObject> getCompatableRooms(string doorway,int minDoors, int maxDoors)
    {
        //intilizes list for possible rooms
        List<GameObject> possibleRooms = new List<GameObject>();

        //loops through all the rooms in the prefabs
        for (int i = 0; i < roomPrefabs.Length; i++)
        {
            RoomProperties currentRoomProperties = roomPrefabs[i].GetComponent<RoomProperties>();
            //checks if there's a left doorway opening to the room and adds it to the list
            if (doorway.Equals("Left"))
            {
                if (currentRoomProperties.leftDoorway && currentRoomProperties.doorwayCount <= maxDoors && currentRoomProperties.doorwayCount >= minDoors)
                {
                    possibleRooms.Add(roomPrefabs[i]);
                }
            }
            //checks if there's a right doorway opening to the room and adds it to the list
            else if (doorway.Equals("Right"))
            {
                if (currentRoomProperties.rightDoorway && currentRoomProperties.doorwayCount <= maxDoors && currentRoomProperties.doorwayCount >= minDoors)
                {
                    possibleRooms.Add(roomPrefabs[i]);
                }
            }
            //checks if there's a top doorway opening to the room and adds it to the list
            else if (doorway.Equals("Top"))
            {
                if (currentRoomProperties.topDoorway && currentRoomProperties.doorwayCount <= maxDoors && currentRoomProperties.doorwayCount >= minDoors)
                {
                    possibleRooms.Add(roomPrefabs[i]);
                }
            }
            //checks if there's a bottom doorway opening to the room and adds it to the list
            else if (doorway.Equals("Bottom"))
            {
                if (currentRoomProperties.bottomDoorway && currentRoomProperties.doorwayCount <= maxDoors && currentRoomProperties.doorwayCount >= minDoors)
                {
                    possibleRooms.Add(roomPrefabs[i]);
                }
            }
        }

        //returns the list of potential rooms
        return possibleRooms;
    }

    List<GameObject> SpawnInDoorways(int minDoorCount, int maxDoorCount, GameObject room)
    {
        List<GameObject> spawnedRooms = new List<GameObject>();
        //gets room properties on the current room
        RoomProperties roomProperties = room.GetComponent<RoomProperties>();

        //checks if doorway is open and spawns a room on the other side of that doorway with an opposite opening with the amount of rooms specified
        if (roomProperties.topOpen)
        {
            GameObject spawnedRoom = 
                SpawnRoom((int)room.transform.position.x, (int)room.transform.position.y + roomSizeY, "Bottom", minDoorCount, maxDoorCount);
            roomProperties.topOpen = false;
            spawnedRoom.GetComponent<RoomProperties>().bottomOpen = false;
            spawnedRooms.Add(spawnedRoom);

        }
        if (roomProperties.bottomOpen)
        {
            GameObject spawnedRoom =
                SpawnRoom((int)room.transform.position.x, (int)room.transform.position.y - roomSizeY, "Top", minDoorCount, maxDoorCount);
            roomProperties.bottomOpen = false;
            spawnedRoom.GetComponent<RoomProperties>().topOpen = false;
            spawnedRooms.Add(spawnedRoom);
        }
        if (roomProperties.leftOpen)
        {
            GameObject spawnedRoom =
                SpawnRoom((int)room.transform.position.x - roomSizeX, (int)room.transform.position.y, "Right", minDoorCount, maxDoorCount);
            roomProperties.leftOpen = false;
            spawnedRoom.GetComponent<RoomProperties>().rightOpen = false;
            spawnedRooms.Add(spawnedRoom);
        }
        if (roomProperties.rightOpen)
        {
            GameObject spawnedRoom =
                SpawnRoom((int)room.transform.position.x + roomSizeX, (int)room.transform.position.y, "Left", minDoorCount, maxDoorCount);
            roomProperties.rightOpen = false;
            spawnedRoom.GetComponent<RoomProperties>().leftOpen = false;
            spawnedRooms.Add(spawnedRoom);
        }

        return spawnedRooms;
    }
}
