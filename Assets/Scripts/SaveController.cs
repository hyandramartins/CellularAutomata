using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public string saveFilePath;
    //public MapGenerator mapGenerator;
    private List<int[,]> mapValid = new List<int[,]>();

    void Start()
    {
        saveFilePath = "Assets/Results/saveDataMap.txt";
        File.WriteAllText(saveFilePath, "");
    }

    public void SaveMap(int [,]map, int id)
    {
        int[,] mapCopy = (int[,])map.Clone();

        mapValid.Add(mapCopy);
        Debug.Log($"Salvando mapa {id}: {mapCopy[0, 0]}");

        File.AppendAllText(saveFilePath, $"{id}\n");

    }

    public int[,] LoadMap()
    {   
        if(mapValid.Count > 0)
        {
            int randIndexMapValidated = Random.Range(0, mapValid.Count);
            Debug.Log($"Carregando índice {randIndexMapValidated}: {mapValid[randIndexMapValidated][0, 0]}");
            return mapValid[randIndexMapValidated];
        }
        else
        {
            Debug.Log("Nenhum mapa valido!");
            return null;
        } 
    }
}
