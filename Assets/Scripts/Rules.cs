using UnityEditor.Tilemaps;
using UnityEngine;

// wall --> 1
// empty --> 0
public class Rules : MonoBehaviour
{
    public bool majority;
    public bool caves;
    public bool diamoeba;
    public bool diamoebaCaves;

    public void Majority(int[,] grid, int x, int y, int limiar, int neighborhoodValue)
    {
        if(neighborhoodValue >= limiar)
            grid[x, y] = 1;
        else
            grid[x, y] = 0;
    }

    public void Caves(int[,] grid, int x, int y, int limiar, int neighborhoodValue)
    {
        if(grid[x, y] == 1)
        {
            if(neighborhoodValue >= limiar - 1)
                grid[x, y] = 1;
            else
                grid[x, y] = 0;
        }
        else
        {
            if(neighborhoodValue >= limiar)
                grid[x, y] = 1;
            else
                grid[x, y] = 0;
        }
    }

    public void Diamoeba(int[,] grid, int x, int y, int limiar, int neighborhoodValue)
    {
        if(grid[x, y] == 1)
        {
            if(neighborhoodValue >= 5)
            
                grid[x, y] = 1;
            else
                grid[x, y] = 0;
        }
        else
        {
            if(neighborhoodValue == 3 || neighborhoodValue >= 5)
                grid[x, y] = 1; 
        }
    }

    public void DiamoebaCaves(int[,] grid, int x, int y, int limiar, int limiar2, int neighborhoodValue, bool firstRule)
    {
        if(firstRule)
            Diamoeba(grid, x, y, limiar, neighborhoodValue);
        else
            Caves(grid, x, y, limiar2, neighborhoodValue);
    }

}
