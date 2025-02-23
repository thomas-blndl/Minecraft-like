using System;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Bounds bounds;

    public bool showGizmo = false;
    private float maxViewDst;

    public ChunkData chunkData { get; private set; }

    public bool IsModifiedByPlayer
    {
        get => chunkData.isModifiedByPlayer;
        set => chunkData.isModifiedByPlayer = value;
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    public void Initialize(ChunkData chunkData)
    {
        this.chunkData = chunkData;
        this.maxViewDst = World.maxViewDst;
        this.bounds = new Bounds(chunkData.worldPosition, Vector2.one * chunkData.chunkSize);
        RenderMesh(Chunk.GetChunkMeshData(chunkData));
        SetVisible(false);
    }

    private void RenderMesh(MeshData meshData)
    {
        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        int[] waterTriangles = meshData.waterMesh.triangles.Select(t => t + meshData.vertices.Count).ToArray();
        mesh.SetTriangles(waterTriangles, 1);

        mesh.uv = meshData.uvs.Concat(meshData.waterMesh.uvs).ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh colliderMesh = new Mesh();
        colliderMesh.vertices = meshData.colliderVertices.ToArray();
        colliderMesh.triangles = meshData.colliderTriangles.ToArray();
        colliderMesh.RecalculateNormals();

        meshCollider.sharedMesh = colliderMesh;
    }

    public void UpdateChunk(Vector3 viewerPosition)
    {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool visible = viewerDstFromNearestEdge <= maxViewDst;
        SetVisible(true);
    }

    internal bool IsVisible()
    {
        return this.meshFilter.gameObject.activeSelf;
    }

    public void SetVisible(bool visible)
    {
        this.meshFilter.gameObject.SetActive(visible);
    }

#if UNITY_EDITOR
        private void OnDrawGizmos()
    {
        if (showGizmo && Application.isPlaying && chunkData != null)
        {
            if (Selection.activeGameObject == gameObject)
            {
                if (chunkData.isModifiedByPlayer)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
            }
         


            Gizmos.DrawCube(transform.position + new Vector3(chunkData.chunkSize / 2f, chunkData.chunkHeight / 2f, chunkData.chunkSize / 2f),
                new Vector3(chunkData.chunkSize, chunkData.chunkHeight, chunkData.chunkSize));
        }
    }


#endif
}
