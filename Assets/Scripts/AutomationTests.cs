using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;

public class AutomationTests : MonoBehaviour
{
    public TextAsset configurationsFile;
    public string path;
    private MapGenerator mapGenerator;
    private Metrics metrics;
    private Rules rules;

    //public Camera cam;
    void Start()
    {
        mapGenerator = MapGenerator.Instance;
        metrics = Metrics.Instance;
        rules = Rules.Instance;

        path = "Assets/Results/metrics.csv";
        File.WriteAllText(path, "id,ConnectivityScore,Openness/NarrownessScore,ComplexyScore\n");

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

            mapGenerator.mapConfig.width = width;
            mapGenerator.mapConfig.height = height;
            mapGenerator.mapConfig.randomSeed = randomSeed;
            mapGenerator.mapConfig.seed = seed;
            mapGenerator.mapConfig.percentFill = percentFill;
            mapGenerator.mapConfig.steps = steps;
            mapGenerator.mapConfig.steps2 = steps2;
            mapGenerator.mapConfig.radius = radius;
            mapGenerator.mapConfig.radius2 = radius2;
            mapGenerator.mapConfig.N = N;
            mapGenerator.mapConfig.N2 = N2;
            mapGenerator.mapConfig.mixRules = mixRules;
            rules.majority = majority;
            rules.caves = caves;
            rules.diamoeba = diamoeba;
            rules.diamoebaCaves = diamoebaCaves;

            mapGenerator.ClearMap();
            mapGenerator.CreateMap();
            mapGenerator.DrawMap();

            // mapGenerator.metrics.ConnectivityMetric(mapGenerator.map, mapGenerator.width, mapGenerator.height);
            // mapGenerator.metrics.OpennessMetric(mapGenerator.map, mapGenerator.width, mapGenerator.height);
            // mapGenerator.metrics.ComplexyMetric();

            metrics.ConnectivityMetric(mapGenerator.map, mapGenerator.mapConfig.width, mapGenerator.mapConfig.height);
            metrics.OpennessMetric(mapGenerator.map, mapGenerator.mapConfig.width, mapGenerator.mapConfig.height);
            metrics.ComplexyMetric();

            yield return new WaitForEndOfFrame();

            string pathImage = $"Assets/Results/Map_{id}.png";

            SaveImageMap(mapGenerator.map, width, height, pathImage);
            //ScreenCapture.CaptureScreenshot($"Assets/Results/Map_{id}.png");

            //string text = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}\n", id, mapGenerator.metrics.connectivityScore, mapGenerator.metrics.opennessScore, mapGenerator.metrics.complexyScore);
            string text = string.Format(CultureInfo.InvariantCulture,"{0},{1},{2},{3}\n", id, metrics.connectivityScore, metrics.opennessScore, metrics.complexyScore);
            File.AppendAllText(path, text);

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

    private void SaveImageMap(int[,] map, int width, int height, string pathImage)
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x <= width - 1; x++)
        {
            for (int y = 0; y <= height - 1; y++)
            {
                Color color;
                switch (map[x, y])
                {
                    case 0:
                        color = Color.white;
                        break;
                    case 1:
                        color = Color.black;
                        break;
                    case 2:
                        color = Color.white;
                        break;
                    case 3:
                        color = Color.black;
                        break;
                    default:
                        color = Color.yellow;
                        break;
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();

        File.WriteAllBytes(pathImage, bytes);
    }
}