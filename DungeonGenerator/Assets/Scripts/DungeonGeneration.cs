using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;

public class DungeonGeneration : MonoBehaviour
{

    /*  ALGORITHM: Tree-based Backpropogating Dungeon Generation

    1. Start with spawn room (only has one connection)
    2. Place hallway tiles (Resources/Dungeon/Floor1/SpecialRooms/Hallway) in the direction of the connection for a random distance and possibly turn
        - If the hallway intersects with another room or hallway tile, re-march the hallway
        - If the hallway fails to be created after a certain number of attempts, backtrack twice and recreate the room that failed to connect
        - That is:

            [] = Room (of varying size)
            -- = Hallway
            ^ = 'Cursor' of hallway generation

            If the dungeon looks like this so far:

            [  ]--[ ]--[]--[   ]-/-  (where -/- is a failed hallway)
                             ^

            Then backtrack and re-march the previous hallway:

            [  ]--[ ]--[]-
                        ^


    3. Repeat until all connections are used

    */

    private GameObject[] rooms;

    public int hallwayMinLength = 3;
    public int hallwayMaxLength = 10;
    public float turnChance = 0.1f; // [0.0 - 1.0]
    public int roomsToGenerate = 20;

    private GameObject masterParent;
    private int hallwayNum = 0;

    public int roomsMade = 1;

    private void Start()
    {

        masterParent = new GameObject("Dungeon");

        // Load rooms
        rooms = Resources.LoadAll<GameObject>("Dungeon/Floor1/Rooms");
        GameObject spawnRoom = Resources.Load<GameObject>("Dungeon/Floor1/SpecialRooms/Spawn");

        // Generate spawn room
        spawnRoom = Instantiate(spawnRoom, Vector3.zero, Quaternion.identity, parent: masterParent.transform);

        GenerateRoom(spawnRoom);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            // Delete the "Dungeon" object and all its children
            Destroy(masterParent);

            // Reset the rooms made counter
            roomsMade = 1;

            Start();
        }
    }

    // Recursive room generation
    private void GenerateRoom(GameObject parent){
        Transform[] connectors = GetConnectors(parent);

        if (connectors == null || connectors.Length == 0)
            return;
        
        foreach (Transform connector in connectors)
        { 
            // Get the direction of the connector
            // TODO: This should be done in a more robust way, probably by making a script that holds the direction of the connector
            // but alas, it is almost the witching hour and I am tired
            Vector3 direction;
            switch (connector.name)
            {
                case "Connector1":
                    direction = Vector3.up;
                    break;
                case "Connector2":
                    direction = Vector3.right;
                    break;
                case "Connector3":
                    direction = Vector3.down;
                    break;
                case "Connector4":
                    direction = Vector3.left;
                    break;
                default:
                    Debug.LogError("Invalid connector name: " + connector.name);
                    direction = Vector3.zero;
                    break;
            }

            // March a hallway in the direction of the connector
            Vector3[] hallwayEnd = MarchHallway(connector.position, direction);

            // Find a room to connect to
            GameObject[] roomPack = FindRoomToConnect(hallwayEnd[1]);

            // Instantiate the room
            GameObject room = Instantiate(roomPack[0], hallwayEnd[0] - roomPack[1].transform.position, Quaternion.identity, parent: masterParent.transform);

            // Disable the used connectors
            // TODO: Dear god, this is a mess. I'm sorry.
            parent.transform.Find("Connectors/" + connector.name).gameObject.SetActive(false);
            room.transform.Find("Connectors/Connector" + roomPack[1].name[9]).gameObject.SetActive(false);

            // Hit me baby, one more time
            roomsMade++;
            GenerateRoom(room);
        }
    }

    /// <summary>
    /// Generates a hallway with a chance to turn at each step.
    /// </summary>
    /// <param name="startPosition">The starting position of the hallway.</param>
    /// <param name="initialDirection">The initial direction to extend the hallway.</param>
    /// <returns>[0] The final position of the hallway, [1] The direction it was heading in as a unit vector.</returns>
    private Vector3[] MarchHallway(Vector3 startPosition, Vector3 initialDirection)
    {

        // Load the prefabs for straight, left turn, and right turn hallway tiles
        GameObject hallwayTilePrefab = Resources.Load<GameObject>("Dungeon/Floor1/SpecialRooms/Hallway");
        GameObject hallwayLeftPrefab = Resources.Load<GameObject>("Dungeon/Floor1/SpecialRooms/HallwayLeft");
        GameObject hallwayRightPrefab = Resources.Load<GameObject>("Dungeon/Floor1/SpecialRooms/HallwayRight");

        // Define the sizes of each tile
        float hallwayTileLength = Mathf.Max(hallwayTilePrefab.transform.localScale.x, hallwayTilePrefab.transform.localScale.y); // Length of the straight tile
        float cornerTileLength = Mathf.Max(hallwayLeftPrefab.transform.localScale.x, hallwayLeftPrefab.transform.localScale.x);  // Length of the corner tile

        // Initialize the position and direction
        Vector3 direction = initialDirection.normalized;
        Vector3 currentPosition = startPosition - direction * hallwayTileLength / 2; // Start one half tile back because the tile is centered

        bool hasTurnedRight = false;
        bool hasTurnedLeft = false;

        // Parent object to hold all hallway tiles
        GameObject hallwayParent = new GameObject("Hallway" + hallwayNum);
        hallwayParent.transform.parent = masterParent.transform;
        GameObject tile = null;

        int hallwayLength = Random.Range(hallwayMinLength, hallwayMaxLength);

        for (int i = 0; i < hallwayLength; i++)
        {
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction); // Default rotation
        
            // Decide whether to turn based on chance, ensuring it's not at the start or end and that it hasn't already turned in that direction
            if ((Random.value < turnChance) && (i > 0) && (i < hallwayLength - 1) && (!hasTurnedRight || !hasTurnedLeft))
            {

                // Randomly decide to turn left or right
                bool turnRight = Random.value > 0.5f;

                if (turnRight && !hasTurnedRight)
                {
                    // Right turn                                                                                                      | -> | = 1/2 + 1/2
                    currentPosition += direction * (hallwayTileLength / 2 + cornerTileLength / 2); // Move by half of each tile [    ][ ][    ]
                    tile = Instantiate(hallwayRightPrefab, currentPosition, Quaternion.Euler(0, 0, -90)); 
                    direction = Quaternion.Euler(0, 0, -90) * direction;  // Update direction to point right
                    hasTurnedRight = true;
                    currentPosition -= direction * (hallwayTileLength / 2 - cornerTileLength / 2); // Similar to above, but subtracting for the corner tile because hallway tile movement assumes it starts at -1/2 hallwaytile length
                }
                else if (!hasTurnedLeft)
                {
                    // Left turn
                    currentPosition += direction * (hallwayTileLength / 2 + cornerTileLength / 2);
                    tile = Instantiate(hallwayLeftPrefab, currentPosition, Quaternion.Euler(0, 0, 90));
                    direction = Quaternion.Euler(0, 0, 90) * direction; // Update direction to point left
                    hasTurnedLeft = true;
                    currentPosition -= direction * (hallwayTileLength / 2 - cornerTileLength / 2);
                }
            }
            else
            {
                // Place a straight hallway tile and move by the straight tile length
                currentPosition += direction * hallwayTileLength;
                tile = Instantiate(hallwayTilePrefab, currentPosition, rotation);
            }

            tile.transform.parent = hallwayParent.transform;

        }

        // Pack up the final position and direction
        Vector3[] vecs = new Vector3[2];
        vecs[0] = currentPosition + direction * hallwayTileLength / 2; // Add half a tile length to get the tip of the last tile
        vecs[1] = direction;

        return vecs;
    }

    private GameObject[] FindRoomToConnect(Vector3 direction)
    {
        // Load all rooms
        GameObject[] rooms = Resources.LoadAll<GameObject>("Dungeon/Floor1/Rooms");

        List<GameObject> validRooms = new List<GameObject>();

        int code = 0;

        GameObject connectorUsed = null;

        // RoomInfo connector codes are set up with 1 = up, 2 = right, 3 = down, 4 = left. (clockwise from up)
        // Inverse the direction to match the direction of the connector

        if (direction.x > 0){ // Left
            code = 4; // Right connector
        } else if (direction.x < 0){ // Right
            code = 2; // Left connector
        } else if (direction.y > 0){ // Up
            code = 3; // Down connector
        } else if (direction.y < 0){ // Down
            code = 1; // Up connector
        }

        // Find all rooms that have a connector in the given direction
        foreach (GameObject room in rooms)
        {
            RoomInfo ri = room.GetComponent<RoomInfo>();
            if (ri.ConnectorDirections.Contains(code))
            {
                validRooms.Add(room);
            }
        }

        if (validRooms.Count == 0)
        {
            Debug.LogError("No valid rooms found for direction " + direction);
            return null;
        }

        // If we've hit the max number of rooms, return a deadend
        // TODO: This could be made better... make a folder for deadends and return a random one?
        if (roomsMade >= roomsToGenerate)
        {
            foreach (GameObject room in validRooms)
            {
                RoomInfo ri = room.GetComponent<RoomInfo>();
                if (ri.ConnectorDirections.Length == 1)
                {
                    return new GameObject[] { room, room.transform.Find("Connectors/Connector" + code).gameObject };
                }
            }
        }

        GameObject[] roomPack = new GameObject[2];
        roomPack[0] = validRooms[Random.Range(0, validRooms.Count)];
        // Get child object "Connector" + code
        roomPack[1] = roomPack[0].transform.Find("Connectors/Connector" + code).gameObject;

        // Return a random room from the list of valid rooms
        return roomPack;
    }


    // Find the child GameObject named "Connectors" under this room
    private Transform[] GetConnectors(GameObject parent){
        
        Transform connectorsParent = parent.transform.Find("Connectors");
        Transform[] connectors = null;

        if (connectorsParent != null)
        {
            // Get all "Connector" children under the "Connectors" GameObject
            connectors = connectorsParent.GetComponentsInChildren<Transform>();

            // Filter out the parent "Connectors" object itself
            connectors = System.Array.FindAll(connectors, c => c != connectorsParent);
            
        }
        else
        {
            Debug.LogError("No 'Connectors' object found in " + gameObject.name);
        }

        return connectors;
    }
}
