using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [HideInInspector]
    public static MapController _instance;

    public const int chunkSize = 20;

    [SerializeField, NotNull]
    private GameObject cubePrefab;

    private MeshRenderer meshRenderer;
    private float cubeSize;

    private GameObject currCube = null;
    private Vector3 currPosition = Vector3.zero;

    public int mapWidth = 1;
    public int mapHeight = 1;
    public int mapScale = 1;
    private int mapSeed;

    public int maxHeight = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _instance = this;
        mapSeed = UnityEngine.Random.Range(0, 10000);
        meshRenderer = cubePrefab.GetComponent<MeshRenderer>();
        cubeSize = meshRenderer.bounds.size.x;
        //GenerateNoiseTerrain(Vector3.zero, 100);
    }

    public GameObject GenerateNoiseTerrain(Vector3 position, int size)
    {
        currPosition = position;

        GameObject chunk = new GameObject("Chunk");
        chunk.AddComponent<ChunkController>();
        chunk.transform.position = position;
        chunk.transform.SetParent(transform, false);

        float[,] heightMap = Noise.GenerateNoiseMap(size, size, mapScale, mapSeed, new Vector2(position.x, position.z));

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                int heightToPlace = Mathf.RoundToInt((cubeSize * (heightMap[x, z] * maxHeight)));

                currCube = Instantiate(cubePrefab);
                currCube.transform.SetParent(chunk.transform, false);
                currCube.transform.position = currPosition + new Vector3(
                    cubeSize * x,
                    heightToPlace,
                    cubeSize * z
                );

                ProcessCubeColor(currCube);
            }
        }

        return chunk;
    }
    private void ProcessCubeColor(GameObject cube)
    {
        int height = (int)cube.transform.position.y;
        Renderer cubeRenderer = currCube.GetComponent<Renderer>();

        if (height <= maxHeight * 0.30)
        {
            cubeRenderer.material.color = UnityEngine.Color.blue;
        }
        else if (height >= maxHeight * 0.80)
        {
            cubeRenderer.material.color = UnityEngine.Color.white;
        }
        else if (height >= maxHeight * 0.50 && height <= maxHeight * 0.80)
        {
            cubeRenderer.material.color = UnityEngine.Color.grey;
        }
        else if (height <= maxHeight * 0.35 && height >= maxHeight * 0.30)
        {
            cubeRenderer.material.color = UnityEngine.Color.yellow;
        }
        else if (height <= maxHeight * 0.50 && height >= maxHeight * 0.35)
        {
            cubeRenderer.material.color = UnityEngine.Color.green;
        }
    }
}
