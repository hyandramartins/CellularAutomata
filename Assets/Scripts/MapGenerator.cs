using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public MapConfig mapConfig;

    public int[,] map;

    private int currentSteps, currentSteps2;

    private Metrics metrics;
    private Rules rules;

    public GameObject wallPrefab;
    public GameObject mapContainer;
    
    void Awake()
    {
        if(Instance == null) Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentSteps = -1;
        currentSteps2 = 0;

        metrics = Metrics.Instance;
        rules = Rules.Instance;
    }

    void Update()
    {
        if (mapConfig.viewSteps && Keyboard.current.qKey.wasPressedThisFrame)
        { 
            if (currentSteps == -1)
            {
                InitializeMap();
                DrawMap();
                currentSteps++;
                Debug.Log("Initial grid rand value.");
            }
            else if (mapConfig.mixRules)
                AdvanceSteps2();
            else
                AdvanceSteps();
        }

        else if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ClearMap();
            CreateMap();
            DrawMap();
            currentSteps = 100000000;
            currentSteps2 = 100000000;
            Debug.Log("Finish map\n");

            metrics.ConnectivityMetric(map, mapConfig.width, mapConfig.height);
            metrics.OpennessMetric(map, mapConfig.width, mapConfig.height);
            metrics.ComplexyMetric();

        }

        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ClearMap();
            currentSteps = -1;
            currentSteps2 = 0;
            Debug.Log("Reset map\n");
        }

    }

    public void InitializeMap()
    {
        map = new int[mapConfig.width, mapConfig.height];

        if (mapConfig.randomSeed)
        {
            mapConfig.seed = System.DateTime.Now.Ticks.ToString();
        }

        System.Random r = new System.Random(mapConfig.seed.GetHashCode());
        Debug.Log(mapConfig.seed);

        if (mapConfig.percentPerCell)
        {
            for (int x = 0; x <= mapConfig.width - 1; x++)
            {
                for (int y = 0; y <= mapConfig.height - 1; y++)
                {
                    map[x, y] = r.Next(0, 100) < mapConfig.percentFill ? 1 : 0;
                }
            }
        }
        else
        {
            List<(int x, int y)> positions = new List<(int x, int y)>();
            int px, py;

            int targetFilledCells = (int)(mapConfig.width * mapConfig.height * (mapConfig.percentFill / 100.00));

            for (int i = 0; i < targetFilledCells; i++)
            {
                do
                {
                    px = r.Next(0, mapConfig.width);
                    py = r.Next(0, mapConfig.height);
                }
                while (positions.Contains((px, py)));

                positions.Add((px, py));
            }

            foreach (var position in positions)
            {
                map[position.x, position.y] = 1;
            }
        }
    }

    public void ExecuteRules()
    {
        int wallCellsCount;
        int[,] newMap = (int[,])map.Clone();


        for (int x = 0; x <= mapConfig.width - 1; x++)
        {
            for (int y = 0; y <= mapConfig.height - 1; y++)
            {
                wallCellsCount = CountWalls(x, y);

                if (rules.majority)
                    rules.Majority(newMap, x, y, mapConfig.N, wallCellsCount);
                else if (rules.caves)
                    rules.Caves(newMap, x, y, mapConfig.N, wallCellsCount);
                else if (rules.diamoeba)
                    rules.Diamoeba(newMap, x, y, mapConfig.N, wallCellsCount);
                else
                    Debug.Log("No rule selected");
            }
        }
        map = (int[,])newMap.Clone();
    }

    public void ExecuteRules2()
    {
        int wallCellsCount;
        int[,] newMap = (int[,])map.Clone();

        if (currentSteps < mapConfig.steps)
        {
            for (int x = 0; x <= mapConfig.width - 1; x++)
            {
                for (int y = 0; y <= mapConfig.height - 1; y++)
                {
                    wallCellsCount = CountWalls(x, y);

                    if (rules.diamoebaCaves)
                        rules.DiamoebaCaves(newMap, x, y, mapConfig.N, mapConfig.N2, wallCellsCount, true);
                    else
                        Debug.Log("No rule selected");

                }
            }
            map = (int[,])newMap.Clone();
        }
        else
        {
            for (int x = 0; x <= mapConfig.width - 1; x++)
            {
                for (int y = 0; y <= mapConfig.height - 1; y++)
                {
                    wallCellsCount = CountWalls2(x, y);

                    if (rules.diamoebaCaves)
                        rules.DiamoebaCaves(newMap, x, y, mapConfig.N, mapConfig.N2, wallCellsCount, false);
                    else
                        Debug.Log("No rule selected");

                }
            }
            map = (int[,])newMap.Clone();
        }
    }

    public void AdvanceSteps()
    {
        if (currentSteps < mapConfig.steps)
        {
            ClearMap();
            ExecuteRules();

            DrawMap();

            currentSteps++;
            Debug.Log("Step: " + currentSteps);
        }
        else
            Debug.Log("completed iterations");
    }

    public void AdvanceSteps2()
    {
        if (currentSteps < mapConfig.steps)
        {
            ClearMap();
            ExecuteRules2();

            DrawMap();

            currentSteps++;
            Debug.Log("Step: " + currentSteps);
        }
        else
        {
            if (currentSteps2 < mapConfig.steps2)
            {
                ClearMap();
                ExecuteRules2();

                DrawMap();

                currentSteps2++;
                Debug.Log("Step: " + currentSteps);
            }
            else
                Debug.Log("completed iterations");
        }
    }
    public void CreateMap()
    {
        InitializeMap();

        int wallCellsCount;
        int[,] newMap = (int[,])map.Clone();

        if (mapConfig.mixRules)
        {
            for (int i = 0; i <= mapConfig.steps - 1; i++)
            {
                for (int x = 0; x <= mapConfig.width - 1; x++)
                {
                    for (int y = 0; y <= mapConfig.height - 1; y++)
                    {
                        wallCellsCount = CountWalls(x, y);

                        if (rules.diamoebaCaves)
                            rules.DiamoebaCaves(newMap, x, y, mapConfig.N, mapConfig.N2, wallCellsCount, true);
                        else
                            Debug.Log("No rule selected");

                    }
                }
                map = (int[,])newMap.Clone();
            }

            for (int i = 0; i <= mapConfig.steps2 - 1; i++)
            {
                for (int x = 0; x <= mapConfig.width - 1; x++)
                {
                    for (int y = 0; y <= mapConfig.height - 1; y++)
                    {
                        wallCellsCount = CountWalls2(x, y);

                        if (rules.diamoebaCaves)
                            rules.DiamoebaCaves(newMap, x, y, mapConfig.N, mapConfig.N2, wallCellsCount, false);
                        else
                            Debug.Log("No rule selected");

                    }
                }
                map = (int[,])newMap.Clone();
            }
        }

        else
        {
            for (int i = 0; i <= mapConfig.steps - 1; i++)
            {
                for (int x = 0; x <= mapConfig.width - 1; x++)
                {
                    for (int y = 0; y <= mapConfig.height - 1; y++)
                    {
                        wallCellsCount = CountWalls(x, y);

                        if (rules.majority)
                            rules.Majority(newMap, x, y, mapConfig.N, wallCellsCount);
                        else if (rules.caves)
                            rules.Caves(newMap, x, y, mapConfig.N, wallCellsCount);
                        else if (rules.diamoeba)
                            rules.Diamoeba(newMap, x, y, mapConfig.N, wallCellsCount);
                        else
                            Debug.Log("No rule selected");
                    }
                }
                map = (int[,])newMap.Clone();
            }
        }
        //DrawMap();
    }

    public int CountWalls(int x, int y)
    {
        int amount = 0;

        if (mapConfig.gridToroidal)
        {
            for (int dirX = x - mapConfig.radius; dirX <= x + mapConfig.radius; dirX++)
            {
                for (int dirY = y - mapConfig.radius; dirY <= y + mapConfig.radius; dirY++)
                {
                    int px = (dirX + mapConfig.width) % mapConfig.width;
                    int py = (dirY + mapConfig.height) % mapConfig.height;

                    if (dirX == x && dirY == y && !mapConfig.includeCurrentCell)
                    {
                        continue;
                    }
                    if (map[px, py] == 1)
                    {
                        amount++;
                    }
                }
            }
        }

        else
        {
            for (int dirX = x - mapConfig.radius; dirX <= x + mapConfig.radius; dirX++)
            {
                for (int dirY = y - mapConfig.radius; dirY <= y + mapConfig.radius; dirY++)
                {
                    if (dirX < 0 || dirX >= mapConfig.width || dirY < 0 || dirY >= mapConfig.height)
                    {
                        amount++;
                        continue;
                    }
                    if (dirX == x && dirY == y && !mapConfig.includeCurrentCell)
                    {
                        continue;
                    }
                    if (map[dirX, dirY] == 1)
                    {
                        amount++;
                    }
                }
            }
        }

        return amount;
    }

    public int CountWalls2(int x, int y)
    {
        int amount = 0;

        if (mapConfig.gridToroidal)
        {
            for (int dirX = x - mapConfig.radius2; dirX <= x + mapConfig.radius2; dirX++)
            {
                for (int dirY = y - mapConfig.radius2; dirY <= y + mapConfig.radius2; dirY++)
                {
                    int px = (dirX + mapConfig.width) % mapConfig.width;
                    int py = (dirY + mapConfig.height) % mapConfig.height;

                    if (dirX == x && dirY == y && !mapConfig.includeCurrentCell2)
                    {
                        continue;
                    }
                    if (map[px, py] == 1)
                    {
                        amount++;
                    }
                }
            }
        }

        else
        {
            for (int dirX = x - mapConfig.radius2; dirX <= x + mapConfig.radius2; dirX++)
            {
                for (int dirY = y - mapConfig.radius2; dirY <= y + mapConfig.radius2; dirY++)
                {
                    if (dirX < 0 || dirX >= mapConfig.width || dirY < 0 || dirY >= mapConfig.height)
                    {
                        amount++;
                        continue;
                    }
                    if (dirX == x && dirY == y && !mapConfig.includeCurrentCell2)
                    {
                        continue;
                    }
                    if (map[dirX, dirY] == 1)
                    {
                        amount++;
                    }
                }
            }
        }

        return amount;
    }


    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x <= mapConfig.width - 1; x++)
            {
                for (int y = 0; y <= mapConfig.height - 1; y++)
                {
                    //Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
                    switch (map[x, y])
                    {
                        case 0:
                            Gizmos.color = Color.white;
                            break;
                        case 1:
                            Gizmos.color = Color.black;
                            break;
                        case 2:
                            Gizmos.color = Color.red;
                            break;
                        case 3:
                            Gizmos.color = Color.green;
                            break;
                        default:
                            Gizmos.color = Color.yellow;
                            break;
                    }

                    Vector3 pos = new Vector3(x - mapConfig.width / 2 + 0.5f, y - mapConfig.height / 2 + 0.5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    public void DrawMap()
    {
        if (map != null)
        {
            for (int x = 0; x <= mapConfig.width - 1; x++)
            {
                for (int y = 0; y <= mapConfig.height - 1; y++)
                {
                    //transform.position = new Vector3(x - width/2 + .5f, y - height/2 + .5f, 0);
                    Vector3 position = new Vector3(x - mapConfig.width / 2 + 0.5f, y - mapConfig.height / 2 + 0.5f, 0);
                    if (map[x, y] == 1)
                    {
                        GameObject tile = Instantiate(wallPrefab, position, Quaternion.identity);
                        tile.transform.SetParent(mapContainer.transform, true);

                    }

                }
            }
        }
    }

    public void ClearMap()
    {
        foreach (Transform child in mapContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
