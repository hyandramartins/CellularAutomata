using System.Collections;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class Metrics : MonoBehaviour
{
    public List<(int x, int y)> BFS(int[,] grid, int x, int y, int width, int height, bool[,] visited, List<(int x, int y)> edges)
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
                    else if (grid[neighborX, neighborY] == 1)
                    {
                        if (!edges.Contains((currentX, currentY)))
                            edges.Add((currentX, currentY));
                    }
                }
            }
        }
        return region;
    }
    public void FloodFill(int[,] grid, int width, int height)
    {
        List<List<(int x, int y)>> regions = new List<List<(int x, int y)>>();
        List<(int x, int y)> edges = new List<(int x, int y)>();

        bool[,] visited = new bool[width, height];

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                if (grid[x, y] == 0 && visited[x, y] == false)
                    regions.Add(BFS(grid, x, y, width, height, visited, edges));
            }
        }

        Debug.Log($"Amount of regions: {regions.Count}\n");
        
        int[] cellsAmount = new int[regions.Count];
        int totalCellsGround = 0;

        for (int i = 0; i <= regions.Count - 1; i++)
        {
            cellsAmount[i] = regions[i].Count;
            totalCellsGround += regions[i].Count;
            Debug.Log($"Region {i}, number of cells : {cellsAmount[i]}\n");
        }

        foreach (var edge in edges)
        {
            grid[edge.x, edge.y] = 2;
        }
        
        var connectivityScore = 1 / Math.Pow(2, regions.Count - 1);
        Debug.Log($"Connectivity Score: {connectivityScore}");

        int greaterOne = totalCellsGround - edges.Count;

        Double magnitudeScore = (Double)greaterOne / totalCellsGround;
        Debug.Log($"Openness/Narrowness Score: {magnitudeScore}, Cells of ground > 1: {greaterOne}, total: {totalCellsGround}\n");

    }
}
