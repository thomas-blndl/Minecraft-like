using UnityEngine;
using System.Collections;
using System;

public static class Noise
{

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, Vector2 perlinPosition)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (x + seed + perlinPosition.x) / scale;
                float sampleY = (y + seed + perlinPosition.y) / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }

}
