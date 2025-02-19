using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public bool autoUpdate;

    public MapDisplay mapDisplay;

    public TextField noiseMapText;

    public void GenerateMap()
    {

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, 100, Vector2.zero);

        mapDisplay.DrawNoiseMap(noiseMap);
       // noiseMapText.text = mapDisplay.ToString();
    }

}