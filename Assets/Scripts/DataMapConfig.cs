using System.Collections.Generic;

[System.Serializable]
public class DataMapConfig 
{
    public int id;
    public int width;
    public int height;

    public bool randomSeed;
    public string seed;
    public int percentFill;

    public bool mixRules;

    public int steps;
    public int radius;
    public int N;

    public int steps2;
    public int radius2;
    public int N2;

    public bool majority;
    public bool caves;
    public bool diamoeba;
    public bool diamoebaCaves;
}

[System.Serializable]
public class ListDataMapConfig
{
    public List<DataMapConfig> configurations;
}
