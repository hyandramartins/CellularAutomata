using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class MapGenerator : MonoBehaviour
{

    public GameObject wallPrefab;
    public GameObject mapContainer;
    public bool viewSteps;
    private int[,] map;
    private int currentSteps = 0, currentSteps2 = 0;
    [SerializeField] int width, height;
    [SerializeField] private string seed;
    [SerializeField] private bool randomSeed;
    [Range(0, 100)]
    [SerializeField] private int percentFill;
    [SerializeField] private int steps, steps2;
    [SerializeField] private int radius, radius2;
    [SerializeField] private int N, N2;
    [SerializeField] private bool percentPerCell;
    [SerializeField] private bool gridToroidal;
    [SerializeField] private bool includeCurrentCell, includeCurrentCell2;
    [SerializeField] private Rules rules;
    [SerializeField] private bool mixRules;

    void Start()
    {
        InitializeMap();
    }

    void Update()
    {
        if (viewSteps && Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (mixRules)
                AdvanceSteps2();
            else
                AdvanceSteps();
        }

        else if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ClearMap();
            CreateMap();
        }

        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ClearMap();
            InitializeMap();
            currentSteps = 0;
            currentSteps2 = 0;
        }

    }

    public void InitializeMap()
    {
        map = new int[width, height];

        if (randomSeed)
        {
            seed = System.DateTime.Now.Ticks.ToString();
        }

        System.Random r = new System.Random(seed.GetHashCode());
        Debug.Log(seed);

        if (percentPerCell)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    map[x, y] = r.Next(0, 100) < percentFill ? 1 : 0;
                }
            }
        }
        else
        {
            List<(int, int)> positions = new List<(int, int)>();
            int px, py;
            int amount = (int)(width * height * (percentFill / 100.00));
            for (int i = 0; i < amount; i++)
            {
                do
                {
                    px = r.Next(0, width);
                    py = r.Next(0, height);
                }
                while (positions.Contains((px, py)));

                positions.Add((px, py));
            }

            foreach (var p in positions)
            {
                map[p.Item1, p.Item2] = 1;
            }
        }
    }

    public void ExecuteRules()
    {
        int amountWall;
        int[,] newMap = (int[,])map.Clone();


        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                amountWall = CountWalls(x, y);

                if (rules.majority)
                    rules.Majority(newMap, x, y, N, amountWall);
                else if (rules.caves)
                    rules.Caves(newMap, x, y, N, amountWall);
                else if (rules.diamoeba)
                    rules.Diamoeba(newMap, x, y, N, amountWall);
                else
                    Debug.Log("No rule selected");
            }
        }
        map = (int[,])newMap.Clone();
    }

    public void ExecuteRules2()
    {
        int amountWall;
        int[,] newMap = (int[,])map.Clone();

        if (currentSteps < steps)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    amountWall = CountWalls(x, y);

                    if (rules.diamoebaCaves)
                        rules.DiamoebaCaves(newMap, x, y, N, N2, amountWall, true);

                }
            }
            map = (int[,])newMap.Clone();
        }
        else
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    amountWall = CountWalls2(x, y);

                    if (rules.diamoebaCaves)
                        rules.DiamoebaCaves(newMap, x, y, N, N2, amountWall, false);

                }
            }
            map = (int[,])newMap.Clone();
        }
    }

    public void AdvanceSteps()
    {
        if (currentSteps < steps)
        {
            if (currentSteps == 0)
            {
                DrawMap();
                currentSteps++;
                Debug.Log("Step: " + currentSteps);
            }
            else
            {
                ClearMap();
                ExecuteRules();

                DrawMap();

                currentSteps++;
                Debug.Log("Step: " + currentSteps);
            }
        }
        else
            Debug.Log("completed iterations");
    }

    public void AdvanceSteps2()
    {
        if (currentSteps < steps)
        {
            if (currentSteps == 0)
            {
                DrawMap();
                currentSteps++;
                Debug.Log("Step: " + currentSteps);
            }
            else
            {
                ClearMap();
                ExecuteRules2();

                DrawMap();

                currentSteps++;
                Debug.Log("Step: " + currentSteps);
            }
        }
        else
        {
            if (currentSteps2 < steps2)
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

        int amountWall;
        int[,] newMap = (int[,])map.Clone();

        if (mixRules)
        {
            for (int i = 0; i <= steps - 1; i++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    for (int y = 0; y <= height - 1; y++)
                    {
                        amountWall = CountWalls(x, y);

                        if (rules.diamoebaCaves)
                            rules.DiamoebaCaves(newMap, x, y, N, N2, amountWall, true);

                    }
                }
                map = (int[,])newMap.Clone();
            }

            for (int i = 0; i <= steps2 - 1; i++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    for (int y = 0; y <= height - 1; y++)
                    {
                        amountWall = CountWalls2(x, y);

                        if (rules.diamoebaCaves)
                            rules.DiamoebaCaves(newMap, x, y, N, N2, amountWall, false);

                    }
                }
                map = (int[,])newMap.Clone();
            }
        }

        else
        {
            for (int i = 0; i <= steps - 1; i++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    for (int y = 0; y <= height - 1; y++)
                    {
                        amountWall = CountWalls(x, y);

                        if (rules.majority)
                            rules.Majority(newMap, x, y, N, amountWall);
                        else if (rules.caves)
                            rules.Caves(newMap, x, y, N, amountWall);
                        else if (rules.diamoeba)
                            rules.Diamoeba(newMap, x, y, N, amountWall);
                        else
                            Debug.Log("No rule selected");
                    }
                }
                map = (int[,])newMap.Clone();
            }
        }
        DrawMap();
    }

    public int CountWalls(int x, int y)
    {
        int amount = 0;

        if (gridToroidal)
        {
            for (int dirX = x - radius; dirX <= x + radius; dirX++)
            {
                for (int dirY = y - radius; dirY <= y + radius; dirY++)
                {
                    int px = (dirX + width) % width;
                    int py = (dirY + height) % height;

                    if (dirX == x && dirY == y && !includeCurrentCell)
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
            for (int dirX = x - radius; dirX <= x + radius; dirX++)
            {
                for (int dirY = y - radius; dirY <= y + radius; dirY++)
                {
                    if (dirX < 0 || dirX >= width || dirY < 0 || dirY >= height)
                    {
                        amount++;
                        continue;
                    }
                    if (dirX == x && dirY == y && !includeCurrentCell)
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

        if (gridToroidal)
        {
            for (int dirX = x - radius2; dirX <= x + radius2; dirX++)
            {
                for (int dirY = y - radius2; dirY <= y + radius2; dirY++)
                {
                    int px = (dirX + width) % width;
                    int py = (dirY + height) % height;

                    if (dirX == x && dirY == y && !includeCurrentCell2)
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
            for (int dirX = x - radius2; dirX <= x + radius2; dirX++)
            {
                for (int dirY = y - radius2; dirY <= y + radius2; dirY++)
                {
                    if (dirX < 0 || dirX >= width || dirY < 0 || dirY >= height)
                    {
                        amount++;
                        continue;
                    }
                    if (dirX == x && dirY == y && !includeCurrentCell2)
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

    /*
    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
                    Vector3 pos = new Vector3(x - width / 2 + .5f, y - height / 2 + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
    */
    private void DrawMap()
    {
        if (map != null)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    //transform.position = new Vector3(x - width/2 + .5f, y - height/2 + .5f, 0);
                    Vector3 position = new Vector3(x - width / 2 + .5f, y - height / 2 + .5f, 0);
                    if (map[x, y] == 1)
                    {
                        GameObject tile = Instantiate(wallPrefab, position, Quaternion.identity);
                        tile.transform.SetParent(mapContainer.transform, true);

                    }

                }
            }
        }
    }

    private void ClearMap()
    {
        foreach (Transform child in mapContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}