using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    private Transform mapParent, floorParent, wallParent;

    private bool canGenereteNew = false;

    private int originalMapSize;


    [HideInInspector]
    public List<Vector3> createdTiles;


    public string seed;

    public bool useRandomSeed = true;

    public int mapSize; // In number of tiles.
    public int tileSize; // In number of pixels (square tiles).

    public float waitTime; // For the agent to wait (in seconds) before moving.
    public float chanceUp, chanceDown, chanceLeft; // To use if you want inequal chance of movement.

    // PREFABS
    public GameObject[] tiles;
    public GameObject wall;


    // WALL GENERATION
    private float minY = 999999, minX = 999999;
    private float maxY = 0, maxX = 0;
    private float xAmount, yAmount; // Amount of tiles in x and y directions.

    public float bufferX, bufferY; // Wall buffer size.



    void Start()
    {
        originalMapSize = mapSize;

        mapParent = new GameObject().transform;
        mapParent.name = "Map tiles";

        floorParent = new GameObject().transform;
        floorParent.name = "Floor tiles";
        floorParent.transform.parent = mapParent;

        wallParent = new GameObject().transform;
        wallParent.name = "Wall tiles";
        wallParent.transform.parent = mapParent;

        chanceDown += chanceUp;
        chanceLeft += chanceDown;

        StartCoroutine(GenerateMap());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canGenereteNew)
        {
            canGenereteNew = false;

            var children = new List<GameObject>();
            foreach (Transform child in floorParent) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));

            foreach (Transform child in wallParent) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));

            mapSize = originalMapSize;

            createdTiles.Clear();

            StartCoroutine(GenerateMap());
        }
    }

    IEnumerator GenerateMap()
    {
        print("Generating seed...");

        yield return new WaitForSeconds(Random.Range(0f, 5f));

        if (useRandomSeed)
            seed = Time.time.ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        for (int i = 0; i < mapSize; i++)
        {
            // 4 directional movement.
            /*
            int direction = Random.Range(0, 3);
            MoveAgent(direction);
            */

            // 4 directional random percentage movement.
            float randomDirection = random.Next(0, 100);
            int tileIndex = random.Next(0, tiles.Length);

            MoveAgentRandom(randomDirection);
            CreateTile(tileIndex);

            yield return new WaitForSeconds(waitTime);


            if(i == mapSize - 1)
            {
                canGenereteNew = true;
                /* Creates walls around the created level.
                FindWallValues();
                CreateWalls();
                */
            }

        }

        yield return 0;
    }

    void MoveAgent(int direction)
    {
        switch (direction)
        { 
            // UP
            case 0:
                transform.position = new Vector3(transform.position.x, transform.position.y + tileSize, 0);
                break;

            // DOWN
            case 1:
                transform.position = new Vector3(transform.position.x, transform.position.y - tileSize, 0);
                break;

            // LEFT
            case 2:
                transform.position = new Vector3(transform.position.x + tileSize, transform.position.y, 0);
                break;

            // RIGHT
            case 3:
                transform.position = new Vector3(transform.position.x - tileSize, transform.position.y, 0);
                break;
        }
    }

    void MoveAgentRandom(float randomDirection)
    {
        if(randomDirection < chanceUp)
        {
            MoveAgent(0);
        }else if(randomDirection < chanceDown)
        {
            MoveAgent(1);
        }else if(randomDirection < chanceLeft)
        {
            MoveAgent(2);
        }else
        {
            MoveAgent(3);
        }
    }

    void CreateTile(int tileIndex)
    {
        // If there is no tile at current position - add new tile.
        if (!createdTiles.Contains(transform.position))
        {
            GameObject newTile = Instantiate(tiles[tileIndex], transform.position, transform.rotation) as GameObject;
            newTile.transform.parent = floorParent;

            createdTiles.Add(newTile.transform.position);
        }
        else
        {
            // "Increase" the map size so that GenerateMap() does indeed generate the requested amount of tiles.
            mapSize++;
        }
    }

    void CreateWalls()
    {
        for(int x = 0; x < xAmount + 1; x++)
        {
            for(int y = 0; y < yAmount + 1; y++)
            {
                if (!createdTiles.Contains(new Vector3((minX - (bufferX * tileSize) / 2) + (x * tileSize), (minY - (bufferY * tileSize) / 2) + (y * tileSize))))
                {

                    GameObject newWall = Instantiate(wall, new Vector3((minX - (bufferX * tileSize) / 2) + (x * tileSize), (minY - (bufferY * tileSize) / 2) + (y * tileSize), 0), transform.rotation) as GameObject;
                    newWall.transform.parent = wallParent;
                }
            }
        }
    }

    void FindWallValues()
    {
        for(int i = 0; i < createdTiles.Count; i++)
        {
            if (createdTiles[i].y < minY)
                minY = createdTiles[i].y;
            if (createdTiles[i].y > maxY)
                maxY = createdTiles[i].y;

            if (createdTiles[i].x < minX)
                minX = createdTiles[i].x;
            if (createdTiles[i].x > maxX)
                maxX = createdTiles[i].x;

            xAmount = ((maxX - minX) / tileSize) + bufferX;
            yAmount = ((maxY - minY) / tileSize) + bufferY;
        }
    }

}
