using UnityEngine;
using System.Collections;

public class AutomationTests : MonoBehaviour
{
    public TextAsset configurationsFile;
    [SerializeField] private MapGenerator mapGenerator;

    void Start()
    {
        StartCoroutine(ReadConfigurations());
    }

    IEnumerator ReadConfigurations()
    {
        string[] lines = configurationsFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');

            int id = int.Parse(values[0]);

            int width = int.Parse(values[1]);
            int height = int.Parse(values[2]);

            bool randomSeed = bool.Parse(values[3]);
            string seed = values[4];

            int percentFill = int.Parse(values[5]);

            int steps = int.Parse(values[6]);
            int steps2 = int.Parse(values[7]);

            int radius = int.Parse(values[8]);
            int radius2 = int.Parse(values[9]);

            int N = int.Parse(values[10]);
            int N2 = int.Parse(values[11]);

            bool mixRules = bool.Parse(values[12]);
            bool majority = bool.Parse(values[13]);
            bool caves = bool.Parse(values[14]);
            bool diamoeba = bool.Parse(values[15]);
            bool diamoebaCaves = bool.Parse(values[16]);

            mapGenerator.width = width;
            mapGenerator.height = height;
            mapGenerator.randomSeed = randomSeed;
            mapGenerator.seed = seed;
            mapGenerator.percentFill = percentFill;
            mapGenerator.steps = steps;
            mapGenerator.steps2 = steps2;
            mapGenerator.radius = radius;
            mapGenerator.radius2 = radius2;
            mapGenerator.N = N;
            mapGenerator.N2 = N2;
            mapGenerator.mixRules = mixRules;
            mapGenerator.rules.majority = majority;
            mapGenerator.rules.caves = caves;
            mapGenerator.rules.diamoeba = diamoeba;
            mapGenerator.rules.diamoebaCaves = diamoebaCaves;

            mapGenerator.ClearMap();
            mapGenerator.CreateMap();
            mapGenerator.DrawMap();
        
            mapGenerator.metrics.FloodFill(mapGenerator.map, mapGenerator.width, mapGenerator.height);
            mapGenerator.metrics.AmountEdges(mapGenerator.map, mapGenerator.width, mapGenerator.height);
            mapGenerator.metrics.Complexy();

            yield return new WaitForEndOfFrame();
            
            ScreenCapture.CaptureScreenshot($"Assets/Results/Map_{id}.png");

            Debug.Log(
                $"ID {id}\n" +
                $"Width: {width}\n" +
                $"Height: {height}\n" +
                $"Random Seed: {randomSeed}\n" +
                $"Seed: {seed}\n" +
                $"Percent Fill: {percentFill}\n" +
                $"Steps: {steps}\n" +
                $"Steps 2: {steps2}\n" +
                $"Radius: {radius}\n" +
                $"Radius 2: {radius2}\n" +
                $"N: {N}\n" +
                $"N2: {N2}\n" +
                $"Mix Rules: {mixRules}\n" +
                $"Majority: {majority}\n" +
                $"Caves: {caves}\n" +
                $"Diamoeba: {diamoeba}\n" +
                $"Diamoeba Caves: {diamoebaCaves}"
            );
        }
    }
}