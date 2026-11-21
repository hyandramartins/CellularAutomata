using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;
using JetBrains.Annotations;

public class AutomationTests : MonoBehaviour
{
    public TextAsset configurationsFile;
    public string metricsFilePath;
    public string ImageFilepath;

    private MapGenerator mapGenerator;
    private Metrics metrics;
    private Rules rules;

    void Start()
    {
        mapGenerator = MapGenerator.Instance;
        metrics = Metrics.Instance;
        rules = Rules.Instance;

        metricsFilePath = "Assets/Results/metrics.csv";
        //File.WriteAllText(path, "id,ConnectivityScore,Openness/NarrownessScore,ComplexyScore\n");

        StartCoroutine(ReadConfigurations());
    }

    private IEnumerator ReadConfigurations()
    {   
        File.WriteAllText(metricsFilePath, "");

        ListDataMapConfig data = JsonUtility.FromJson<ListDataMapConfig>(configurationsFile.text);

        foreach(DataMapConfig config in data.configurations)
        {
            mapGenerator.mapConfig.width = config.width;
            mapGenerator.mapConfig.height = config.height;

            mapGenerator.mapConfig.randomSeed = config.randomSeed;
            mapGenerator.mapConfig.seed = config.seed;
            mapGenerator.mapConfig.percentFill = config.percentFill;

            mapGenerator.mapConfig.mixRules = config.mixRules;

            mapGenerator.mapConfig.steps = config.steps;
            mapGenerator.mapConfig.radius = config.radius;
            mapGenerator.mapConfig.N = config.N;

            mapGenerator.mapConfig.steps2 = config.steps2;
            mapGenerator.mapConfig.radius2 = config.radius2;
            mapGenerator.mapConfig.N2 = config.N2;

            rules.majority = config.majority;
            rules.caves = config.caves;
            rules.diamoeba = config.diamoeba;
            rules.diamoebaCaves = config.diamoebaCaves;

            mapGenerator.ClearMap();
            mapGenerator.CreateMap();
            mapGenerator.DrawMap();

            metrics.ConnectivityMetric(
                mapGenerator.map,
                mapGenerator.mapConfig.width,
                mapGenerator.mapConfig.height
            );

            metrics.OpennessMetric(
                mapGenerator.map,
                mapGenerator.mapConfig.width,
                mapGenerator.mapConfig.height
            );

            metrics.ComplexyMetric();

            yield return new WaitForEndOfFrame();

            ImageFilepath = $"Assets/Results/Map_{config.id}.png";

            SaveImageMap(mapGenerator.map, config.width, config.height, ImageFilepath);

            string metricsResult = $"ID: {config.id}\nConnectivityScore: {metrics.connectivityScore}\nOpenness/NarrownessScore: {metrics.opennessScore}\nComplexyScore: {metrics.complexyScore}\n\n";
            
            File.AppendAllText(metricsFilePath, metricsResult);
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