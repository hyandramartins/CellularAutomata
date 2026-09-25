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
    public int totalCellsGround;
    public int totalPerimeter;

    public List<(int x, int y)> BFS(int[,] grid, int x, int y, int width, int height, bool[,] visited)
    {
        List<(int x, int y)> region = new List<(int x, int y)>();

        Queue<(int x, int y)> Q = new Queue<(int x, int y)>();

        Q.Enqueue((x, y));

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
                    /*else if (grid[neighborX, neighborY] == 1)
                    {
                        if (!edges.Contains((currentX, currentY)))
                            edges.Add((currentX, currentY));
                    }*/
                }
            }
        }
        return region;
    }

    public void AmountEdges(int[,] grid, int width, int height)
    {
        totalPerimeter = 0;

        List<(int x, int y)> edges = new List<(int x, int y)>();
        List<(int x, int y)> perimeter = new List<(int x, int y)>();

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                if (grid[x, y] == 0)
                {
                    if (isEdge(grid, x, y, width, height))
                    {
                        edges.Add((x, y));
                    }
                }
                else if (grid[x, y] == 1)
                {
                    if (isPerimeter(grid, x, y, width, height))
                    {
                        perimeter.Add((x, y));
                        totalPerimeter++;
                    }
                }
            }
        }

        /*foreach (var edge in edges)
        {
            grid[edge.x, edge.y] = 2;
        }*/

        foreach (var p in perimeter)
        {
            grid[p.x, p.y] = 3;
        }

        int greaterOne = totalCellsGround - edges.Count;

        Double magnitudeScore = (Double)greaterOne / totalCellsGround;
        Debug.Log($"Openness/Narrowness Score: {magnitudeScore}, Cells of ground > 1: {greaterOne}, total: {totalCellsGround}\n");
        Debug.Log($"Total Perimeter: {totalPerimeter}");
    }

    public bool isEdge(int[,] grid, int x, int y, int width, int height)
    {
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

    public bool isPerimeter(int[,] grid, int x, int y, int width, int height)
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
                    if (grid[neighborX, neighborY] == 0) return true;
                }
            }
        }
        return false;
    }
    public void FloodFill(int[,] grid, int width, int height)
    {
        List<List<(int x, int y)>> regions = new List<List<(int x, int y)>>();
        //List<(int x, int y)> edges = new List<(int x, int y)>();

        bool[,] visited = new bool[width, height];

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                if (grid[x, y] == 0 && visited[x, y] == false)
                    regions.Add(BFS(grid, x, y, width, height, visited));
            }
        }

        Debug.Log($"Amount of regions: {regions.Count}\n");

        int[] cellsAmount = new int[regions.Count];
        totalCellsGround = 0;

        for (int i = 0; i <= regions.Count - 1; i++)
        {
            cellsAmount[i] = regions[i].Count;
            totalCellsGround += regions[i].Count;
            Debug.Log($"Region {i}, number of cells : {cellsAmount[i]}\n");
        }

        /*foreach (var edge in edges)
        {
            grid[edge.x, edge.y] = 2;
        }*/

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

        float dwarfCaves = largestRegion * 0.2f;
        Debug.Log($"Dwarf Caves: {dwarfCaves}");

        for (int i = 0; i <= regions.Count - 1; i++)
        {
            if (regions[i].Count > dwarfCaves)
            {
                validRegion++;
                Debug.Log($"Index valid regions: {i}");
            }
        }

        var connectivityScore = 1 / Math.Pow(2, validRegion - 1);
        Debug.Log($"Connectivity Score: {connectivityScore}");
    }

    public void Complexy()
    {
        double r = Math.Sqrt(totalCellsGround / Math.PI);

        double roughnessMax = r / 2.0;
        double roughnessReal = (double)totalCellsGround/ totalPerimeter;

        double complexyScore = roughnessReal / roughnessMax;

        Debug.Log($"Complexy Score: {complexyScore}");
    }
}
