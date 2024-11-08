using UnityEngine;

public class Leaf
{

    public static int minLeafSize = 20;
    public static int maxLeafSize = 30;
    public static int numLeaves = 0;
    public static int roomBuffer = 2;
    public static GameObject roomPrefab;
    public static GameObject hallwayPrefab;
    public static GameObject dungeonRoot;
    public static float splitChance = 0.75f;
    private int x, y, width, height;
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
        background.GetComponent<Renderer>().material.color = Color.black;
        background.name = "Leaf" + numLeaves;

        // Set the parent of the background to the dungeonRoot
        background.transform.parent = dungeonRoot.transform;
    }

    public void CreateRoom()
    {
        // Create a room inside the leaf
        int roomWidth = width - roomBuffer;
        int roomHeight = height - roomBuffer;
        int roomX = x + roomBuffer / 2;
        int roomY = y + roomBuffer / 2;

        room = GameObject.Instantiate(roomPrefab);
        room.transform.position = new Vector3(roomX + roomWidth / 2, roomY + roomHeight / 2, 0);
        room.transform.localScale = new Vector3(roomWidth, roomHeight, 1);
        room.name = "Room" + numLeaves;

        // Set the parent of the room to the dungeonRoot
        room.transform.parent = dungeonRoot.transform;
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

        // If the leaf is too small to split, or if the leaf chooses not to split, then draw the room and return false
        if (max <= minLeafSize || (Random.Range(0f, 1f) > splitChance && width < maxLeafSize && height < maxLeafSize))  
        {
            CreateRoom();
            return false;
        }

        // Determine the split position
        int split = Random.Range(minLeafSize, max);

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
}

public class BSPDungeon : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject hallwayPrefab;
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
        Leaf.roomPrefab = roomPrefab;
        Leaf.hallwayPrefab = hallwayPrefab;
        Leaf.dungeonRoot = dungeonRoot;
        Leaf.splitChance = splitChance;
        Leaf.maxLeafSize = maxLeafSize;

        rootLeaf = new Leaf(0, 0, rootLeafSizeX, rootLeafSizeY);
        
        SplitAllLeaves(rootLeaf);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Leaf.numLeaves = 0;
            Destroy(dungeonRoot);
            Start();
        }   
    }

    void SplitAllLeaves(Leaf leaf)
    {
        if (leaf == null)
        {
            return;
        }

        if (leaf.Split())
        {
            SplitAllLeaves(leaf.leftChild);
            SplitAllLeaves(leaf.rightChild);
        }
    }
}
