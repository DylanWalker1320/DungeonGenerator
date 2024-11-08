using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Leaf
{

    public static int minLeafSize = 20;
    public static int numLeaves = 0;
    public static int roomBuffer = 2;
    public static GameObject dungeonRoot;
    public static Tilemap tilemap;
    public static RuleTile ruleTile;
    public int x, y, width, height;
    public Leaf leftChild, rightChild;
    private GameObject room;
    
    private GameObject background;

    public Leaf(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;

        numLeaves++;
        
        DrawBackground();
    }

    public void DrawBackground()
    {
        // Literally create a new Square GameObject and scale it
        background = GameObject.CreatePrimitive(PrimitiveType.Cube);
        background.transform.position = new Vector3(x + width / 2, y + height / 2, 0);
        background.transform.localScale = new Vector3(width, height, 1);
        background.name = "Leaf" + numLeaves;

        // Choose random color for the background
        background.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        // Set the parent of the background to the dungeonRoot
        background.transform.parent = dungeonRoot.transform;
    }

    public bool Split()
    {

        // If this leaf has already been split, return false
        if (leftChild != null || rightChild != null)
        {
            return false;
        }

        // Determine direction of split
        // If the width is >25% larger than height, split vertically
        // If the height is >25% larger than width, split horizontally
        bool splitH = Random.Range(0, 1) > 0.5f;
        if (width > height && width / height >= 1.25)
        {
            splitH = false;
        }
        else if (height > width && height / width >= 1.25)
        {
            splitH = true;
        }

        /*
        Example: If leaf is 30x30 and minLeafSize is 10, then the maximum split size is 20. 

        |-----------------|-----------------|
        |                 |                 |
        |                 |                 |
        |                 |                 |
        |                 |                 |
        |                 |                 |
        |                 |                 |
        |---------|-------|------|----------|
        0         10     15     20         30
                 min    split   max

        Can split anywhere between 10 and 20
        */

        // Determine the maximum split size
        int max = (splitH ? height : width) - minLeafSize;

        // Determine the split position
        int split = Random.Range(minLeafSize, max);

        // Check if split would be too small
        if (max <= minLeafSize)
        {
            return false;
        }

        // Create the children based on the split direction
        if (splitH)
        {
            leftChild = new Leaf(x, y, width, split);
            rightChild = new Leaf(x, y + split, width, height - split);
        }
        else
        {
            leftChild = new Leaf(x, y, split, height);
            rightChild = new Leaf(x + split, y, width - split, height);
        }

        // Undo the draw of the parent
        GameObject.Destroy(background);

        // Return true if the leaf was split
        return true;

    }

    public void CreateRooms()
    {
        // Generate rooms for this Leaf and all of its children.
        if (leftChild != null || rightChild != null)
        {
            if (leftChild != null)
            {
                leftChild.CreateRooms();
            }
            if (rightChild != null)
            {
                rightChild.CreateRooms();
            }

            // Create a hallway between left and right children
            if (leftChild != null && rightChild != null)
            {
                CreateHallway(leftChild.GetRoomCenter(), rightChild.GetRoomCenter());
            }
        }
        else
        {
            // Generate the room within the boundaries of this leaf.
            for (int u = x + roomBuffer; u < x + width - roomBuffer; u++)
            {
                for (int v = y + roomBuffer; v < y + height - roomBuffer; v++)
                {
                    tilemap.SetTile(new Vector3Int(u, v, 0), ruleTile);
                }
            }
        }
    }


    public Vector2Int GetRoomCenter()
    {
        int centerX = x + roomBuffer + width / 2;
        int centerY = y + roomBuffer + height / 2;
        return new Vector2Int(centerX, centerY);
    }

    private void CreateHallway(Vector2Int start, Vector2Int end, int hallwayWidth = 3)
    {
        if (Mathf.Abs(start.x - end.x) > Mathf.Abs(start.y - end.y))
        {
            // Horizontal hallway
            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int y = start.y;

            for (int x = minX; x <= maxX; x++)
            {
                for (int i = -hallwayWidth / 2; i <= hallwayWidth / 2; i++)
                {
                    tilemap.SetTile(new Vector3Int(x, y + i, 0), ruleTile);
                }
            }
        }
        else
        {
            // Vertical hallway
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);
            int x = start.x;

            for (int y = minY; y <= maxY; y++)
            {
                for (int i = -hallwayWidth / 2; i <= hallwayWidth / 2; i++)
                {
                    tilemap.SetTile(new Vector3Int(x + i, y, 0), ruleTile);
                }
            }
        }
    }
}

public class BSPDungeon : MonoBehaviour
{
    public Tilemap tilemap;
    public RuleTile ruleTile;
    public int roomBuffer = 2;
    public int minLeafSize = 10;
    public int maxLeafSize = 30;
    public int rootLeafSizeX = 100;
    public int rootLeafSizeY = 100;
    public float splitChance = 0.75f;

    private Leaf rootLeaf;
    private GameObject dungeonRoot;

    void Start()
    {
        dungeonRoot = new GameObject();
        dungeonRoot.name = "DungeonRoot";

        Leaf.minLeafSize = minLeafSize;
        Leaf.dungeonRoot = dungeonRoot;
        Leaf.tilemap = tilemap;
        Leaf.ruleTile = ruleTile;
        Leaf.roomBuffer = roomBuffer;

        CreateLeaves();
        rootLeaf.CreateRooms();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Leaf.numLeaves = 0;
            Destroy(dungeonRoot);
            ClearTilemap();
            Start();
        }
    }

    void CreateLeaves()
    {
        List<Leaf> leafList = new List<Leaf>();
        rootLeaf = new Leaf(0, 0, rootLeafSizeX, rootLeafSizeY);
        leafList.Add(rootLeaf);

        bool didSplit = true;
        
        // Split the leaves until no more leaves can be split
        while (didSplit)
        {
            didSplit = false;
            // Important: using a for loop here instead of foreach because we will be adding to the list
            for (int i = 0; i < leafList.Count; i++)
            {
                Leaf l = leafList[i];
                if (l.leftChild == null && l.rightChild == null)
                {
                    if (l.Split())
                    {
                        leafList.Add(l.leftChild);
                        leafList.Add(l.rightChild);
                        didSplit = true;
                    }
                }
            }
        }
    }

    void ClearTilemap()
    {
        tilemap.ClearAllTiles();
    }
}
