using System.Collections;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using Unity.Mathematics;
using UnityEngine.Rendering;

public class Metrics : MonoBehaviour
{
    public static Metrics Instance { get; private set; }

    public int totalCellsGround;
    public int totalCellsPerimeter;
    public int totalCellsEdges;
    public double connectivityScore;
    public double complexyScore;
    public double opennessScore;

    private List<(int x, int y)> edges;
    private List<(int x, int y)> perimeter;
    private List<List<(int x, int y)>> regions;
    private List<List<(int x, int y)>> regionsDwarf;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public List<(int x, int y)> BFS(int[,] grid, int x, int y, int width, int height, bool[,] visited)
    {
        List<(int x, int y)> region = new List<(int x, int y)>();

        Queue<(int x, int y)> Q = new Queue<(int x, int y)>();

        Q.Enqueue((x, y));
        visited[x, y] = true;
        region.Add((x, y));

        while (Q.Count > 0)
        {
            List<(int x, int y)> neighbors = new List<(int x, int y)>();
            var (currentX, currentY) = Q.Dequeue();

            neighbors.Add((currentX, currentY - 1));
            neighbors.Add((currentX, currentY + 1));
            neighbors.Add((currentX - 1, currentY));
            neighbors.Add((currentX + 1, currentY));

            foreach (var neighbor in neighbors)
            {
                int neighborX = neighbor.x;
                int neighborY = neighbor.y;

                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (grid[neighborX, neighborY] == 0 && visited[neighborX, neighborY] == false)
                    {
                        visited[neighborX, neighborY] = true;
                        Q.Enqueue((neighborX, neighborY));
                        region.Add((neighborX, neighborY));
                    }
                }
            }
        }
        return region;
    }

    public void FloodFill(int[,] grid, int width, int height)
    {
        regions = new List<List<(int x, int y)>>();

        bool[,] visited = new bool[width, height];

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                if (grid[x, y] == 0 && visited[x, y] == false)
                    regions.Add(BFS(grid, x, y, width, height, visited));
            }
        }
    }

    public void ConnectivityMetric(int[,] grid, int width, int height)
    {

        FloodFill(grid, width, height);

        Debug.Log($"Amount of regions: {regions.Count}\n");

        totalCellsGround = 0;

        for (int i = 0; i <= regions.Count - 1; i++)
        {
            totalCellsGround += regions[i].Count;

            Debug.Log($"Region {i}, number of cells : {regions[i].Count}\n");
        }

        int largestRegion = 0;
        int indexLargestRegion = 0;

        for (int i = 0; i <= regions.Count - 1; i++)
        {
            if (regions[i].Count > largestRegion)
            {
                largestRegion = regions[i].Count;
                indexLargestRegion = i;
            }
        }

        Debug.Log($"Number of cells in the larger region: {largestRegion}, index: {indexLargestRegion}");

        int validRegion = 0;

        float dwarfCavesThreshold = largestRegion * 0.2f;
        Debug.Log($"Dwarf Caves Threshold: {dwarfCavesThreshold}");

        regionsDwarf = new List<List<(int x, int y)>>();

        for (int i = 0; i <= regions.Count - 1; i++)
        {
            if (regions[i].Count > dwarfCavesThreshold)
            {
                validRegion++;

                //totalCellsGround += regions[i].Count;

                Debug.Log($"Index valid regions: {i}");
            }
            else
            {
                regionsDwarf.Add(regions[i]);
            }
        }

        connectivityScore = 1 / Math.Pow(2, validRegion - 1);
        Debug.Log($"Connectivity Score: {connectivityScore}");
    }

    public void OpennessMetric(int[,] grid, int width, int height)
    {
        totalCellsPerimeter = 0;
        totalCellsEdges = 0;

        edges = new List<(int x, int y)>();
        perimeter = new List<(int x, int y)>();

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                if (grid[x, y] == 0)
                {
                    if (IsEdge(grid, x, y, width, height))
                    {
                        edges.Add((x, y));
                        totalCellsEdges++;
                    }
                }
                else if (grid[x, y] == 1)
                {
                    if (IsPerimeter(grid, x, y, width, height))
                    {
                        perimeter.Add((x, y));
                        totalCellsPerimeter++;
                    }
                }
            }
        }

        foreach (var edge in edges)
        {
            grid[edge.x, edge.y] = 2;
        }

        foreach (var p in perimeter)
        {
            grid[p.x, p.y] = 3;
        }

        int greaterOne = totalCellsGround - totalCellsEdges;
        
        opennessScore = (Double)greaterOne / totalCellsGround;
        Debug.Log($"Openness/Narrowness Score: {opennessScore}, Amount cells of ground > 1: {greaterOne}, total cell of ground: {totalCellsGround}\n");

        Debug.Log($"Total Perimeter: {totalCellsPerimeter}");
    }

    public bool IsEdge(int[,] grid, int x, int y, int width, int height)
    {
        // if(IsDwarfRegion(x, y))
        // {
        //     return false;
        // }

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int neighborX = x + dx;
                int neighborY = y + dy;

                if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height)
                    return true;

                if (grid[neighborX, neighborY] == 1) return true;
            }
        }
        return false;
    }

    public bool IsPerimeter(int[,] grid, int x, int y, int width, int height)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int neighborX = x + dx;
                int neighborY = y + dy;

                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    //if (grid[neighborX, neighborY] == 0 && !IsDwarfRegion(neighborX, neighborY)) return true;
                    if (grid[neighborX, neighborY] == 0) return true;
                }
            }
        }
        return false;
    }

    public bool IsDwarfRegion(int x, int y)
    {
        foreach (var region in regionsDwarf)
        {
            if (region.Contains((x, y)))
            {
                return true;
            }
        }

        return false;
    }

    public void ComplexyMetric()
    {
        double r = Math.Sqrt(totalCellsGround / Math.PI);

        double roughnessMax = r / 2.0;
        double roughnessReal = (double)totalCellsGround / totalCellsPerimeter;

        complexyScore = 1.0 - (roughnessReal / roughnessMax);

        Debug.Log($"Complexy Score: {complexyScore}");
    }
}
