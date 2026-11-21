using UnityEngine;

[CreateAssetMenu(menuName = "Map/map config")]
public class MapConfig : ScriptableObject
{
    public string mapID { get; private set; } 

    public int width, height;

    public string seed;
    public bool randomSeed;

    [Range(0, 100)]
    public int percentFill;
    public bool percentPerCell;

    public bool viewSteps;
    public bool gridToroidal;

    public bool mixRules;

    [Header("Rules 1")]
    public int steps;
    public int radius;
    public int N;
    public bool includeCurrentCell;

    [Header("Rules 2")]
    public int  steps2;
    public int radius2;
    public int N2;
    public bool includeCurrentCell2;

    private void OnValidate()
    {
        if(string.IsNullOrEmpty(mapID))
        {
            mapID = Global.GenerateID();
        }
    }
}


