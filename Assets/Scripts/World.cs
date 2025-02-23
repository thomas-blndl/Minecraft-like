using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int waterThreshold = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

    public const float maxViewDst = 16*10;

    public Transform viewer;
    public static Vector2 viewerPosition;
    int chunksVisibleInViewDst;

    private int seed;


    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();
    List<ChunkRenderer> chunksVisibleLastUpdate = new List<ChunkRenderer>();

    private Thread _generationThread;
    private Thread _updateVisibleChunks;


    private void Start()
    {
        seed = UnityEngine.Random.Range(0, 999999);
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        _updateVisibleChunks = new Thread(UpdateVisibleChunks);
        //GenerateWorld();
    }
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }
/*
    public void GenerateWorld()
    {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {

                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPosition, data);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.Initialize(data);
            chunkRenderer.UpdateChunk(meshData);

        }
    }
*/

    void UpdateVisibleChunks()
    {
        List<Vector3Int> chunksToGenerateList = new List<Vector3Int>();

        for (int i = 0; i < chunksVisibleLastUpdate.Count; i++)
        {
            chunksVisibleLastUpdate[i].SetVisible(false);
        }
        chunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector3Int chunkCoord = new Vector3Int((currentChunkCoordX + xOffset) * chunkSize, 0, (currentChunkCoordY + yOffset) * chunkSize);

                if (chunkDictionary.ContainsKey(chunkCoord))
                {
                    if (chunkDictionary[chunkCoord] != null)
                    {
                        chunkDictionary[chunkCoord].UpdateChunk(viewerPosition);
                        if (chunkDictionary[chunkCoord].IsVisible())
                        {
                            chunksVisibleLastUpdate.Add(chunkDictionary[chunkCoord]);
                        }
                    }
                }
                else
                {
                    ChunkData data = new ChunkData(chunkSize, chunkHeight, this, chunkCoord);
                    //GenerateVoxels(data);
                    Debug.Log("Creating thread");
                    _generationThread = new Thread(() => GenerateVoxels(data));
                    _generationThread.Start();

                    chunkDataDictionary.Add(chunkCoord, data);
                    chunkDictionary.Add(chunkCoord, null);
                    chunksToGenerateList.Add(chunkCoord);
                }
            }
        }
        GeneratesChunks(chunksToGenerateList);
    }

    private void GeneratesChunks(List<Vector3Int> chunksToGenerateList)
    {
        List<Vector3Int> chunksToGenerateListTmp = new List<Vector3Int>(chunksToGenerateList);

        foreach (Vector3Int chunkCoord in chunksToGenerateListTmp)
        {
            ChunkData data = chunkDataDictionary[chunkCoord];
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            chunkObject.transform.parent = transform;
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary[chunkCoord] = chunkRenderer;
            chunkRenderer.Initialize(data);

            chunksToGenerateList.Remove(chunkCoord);
        }
    }

    private void GenerateVoxels(ChunkData data)
    {
        Debug.Log("Generating Voxels");
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPosition.x + x + seed) * noiseScale , (data.worldPosition.z + z + seed) * noiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight);

                for (int y = 0; y < chunkHeight; y++)
                {
                    BlockType voxelType = BlockType.Dirt;
                    if (y > groundPosition)
                    {
                        if (y < waterThreshold)
                        {
                            voxelType = BlockType.Water;
                        }
                        else
                        {
                            voxelType = BlockType.Air;
                        }
                    }
                    else if (y == groundPosition)
                    {
                        if (y < waterThreshold + 3)
                        {
                            voxelType = BlockType.Sand;
                        }
                        else
                        {
                            voxelType = BlockType.Grass;
                        }
                    }

                    if (y <= groundPosition)
                    {
                        if (y > chunkHeight * 0.80)
                        {
                            voxelType = BlockType.Stone;
                        }
                        if (y > chunkHeight * 0.90)
                        {
                            voxelType = BlockType.StoneSnow;
                        }
                    }

                    if (y < groundPosition -3)
                    {
                        voxelType = BlockType.Stone;
                    }

                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
        Debug.Log("created Voxels");

    }

    public BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return BlockType.Nothing;
        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }

}