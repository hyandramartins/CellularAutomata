using UnityEngine;
using UnityEngine.InputSystem;
public class MapGenerator : MonoBehaviour
{
    [SerializeField] int width, height;
    private int[,] map;
    [SerializeField] private string seed;
    [SerializeField] private bool randomSeed;
    [Range(0, 100)]
    [SerializeField] private int percentFill;
    [SerializeField] private int steps;
    public GameObject prefab;

    void Start()
    {   
        //initializeMap();
        CreateMap();
        //Draw();
    }

    void Update()
    {   
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {   
            //initializeMap();
            CreateMap();
            //Draw();
        }
    }

    public void initializeMap()
    {
        map = new int[width, height];

        if (randomSeed)
        {
            seed = System.DateTime.Now.Ticks.ToString();
        }

        System.Random r = new System.Random(seed.GetHashCode());
        Debug.Log(seed);

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                if(x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1; //borda
                }
                else
                {
                    map[x, y] = r.Next(0, 100) < percentFill ? 1 : 0; // 1 = bloco
                }      
            }
        }
    }

    public void CreateMap()
    {
        initializeMap();
        int count = 0;
        int amountWall;
        int[,] newMap = (int[,])map.Clone();
        while (count < steps)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    amountWall = CountWalls(x, y);
                    if (amountWall > 4)
                    {
                        newMap[x, y] = 1;
                    }
                    else if (amountWall <= 4)
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
            map = (int[,])newMap.Clone();
            count++;
        }
    }

    public int CountWalls(int x, int y)
    {
        int amount = 0;
        for (int dirX = x - 1; dirX <= x + 1; dirX++)
        {
            for (int dirY = y - 1; dirY <= y + 1; dirY++)
            {
                if (dirX < 0 || dirX >= width || dirY < 0 || dirY >= height)
                {
                    amount++;
                }
                else if (dirX == x && dirY == y)
                {
                    continue;
                }
                else if (map[dirX, dirY] == 1)
                {
                    amount++;
                }
            }
        }
        return amount;
    }

    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
                    Vector3 pos = new Vector3(x - width/2 + .5f, y - height/2 + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
    
    private void Draw()
    {
        if (map != null)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {   
                    //transform.position = new Vector3(x - width/2 + .5f, y - height/2 + .5f, 0);
                    Vector3 pos = new Vector3(x - width/2 + .5f, y - height/2 + .5f, 0);
                    if(map[x, y] == 1)
                    {
                        Instantiate(prefab, pos, Quaternion.identity);
                    }
                
                }
            }
        }
    }
}
