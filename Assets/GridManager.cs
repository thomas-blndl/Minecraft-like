// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GridManager : MonoBehaviour
// {
//     public static GridManager Instance;
//     public int gridSizeX = 100;
//     public int gridSizeY = 100;
//     public int gridSizeZ = 100;
//     public float blockSize = 1.0f;
//     public Vector3 minWorldPosition = new Vector3(-100, 0, -100);
//     public GameObject blockPrefab;
//     private Grid grid;


    

//     // Start is called before the first frame update
//     void Start()
//     {
//         Instance = this;
//         grid = GetComponent<Grid>();
//     }

//     public Vector3Int WorldToGridPosition(Vector3 worldPosition)
//     {
//         int x = Mathf.FloorToInt((worldPosition.x - minWorldPosition.x) / blockSize);
//         int y = Mathf.FloorToInt((worldPosition.y - minWorldPosition.y) / blockSize);
//         int z = Mathf.FloorToInt((worldPosition.z - minWorldPosition.z) / blockSize);
//         return new Vector3Int(x, y, z);
//     }




//     public Vector3 GridToWorldPosition(Vector3Int gridPosition)
//     {
//         float x = gridPosition.x * blockSize + minWorldPosition.x;
//         float y = gridPosition.y * blockSize + minWorldPosition.y;
//         float z = gridPosition.z * blockSize + minWorldPosition.z;
//         return new Vector3(x, y, z);
//     }


//     public void AddBlock(GameObject blockPrefab, Vector3Int gridPosition)
//     {
//         if (IsWithinGridBounds(gridPosition) && grid[gridPosition.x, gridPosition.y, gridPosition.z] == null)
//         {
//             Vector3 worldPosition = GridToWorldPosition(gridPosition);
//             GameObject newBlock = Instantiate(blockPrefab, worldPosition, Quaternion.identity);
//             grid[gridPosition.x, gridPosition.y, gridPosition.z] = newBlock;
//         }
//     }

//     public void RemoveBlock(Vector3Int gridPosition)
//     {
//         if (IsWithinGridBounds(gridPosition) && grid[gridPosition.x, gridPosition.y, gridPosition.z] != null)
//         {
//             Destroy(grid[gridPosition.x, gridPosition.y, gridPosition.z]);
//             grid[gridPosition.x, gridPosition.y, gridPosition.z] = null;
//         }
//     }

//     public bool IsWithinGridBounds(Vector3Int gridPosition)
//     {
//         return gridPosition.x >= 0 && gridPosition.x < gridSizeX &&
//             gridPosition.y >= 0 && gridPosition.y < gridSizeY &&
//             gridPosition.z >= 0 && gridPosition.z < gridSizeZ;
//     }



// }
