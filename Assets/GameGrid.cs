using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameGrid : MonoBehaviour
{
    public static GameGrid Instance;
    private int height = 50;
    private int width = 8;
    private int depth = 8;
    private float gridSpaceSize = 1f;

    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject gridCubePrefab;

    private GameObject[,] gameGrid;
    private GameObject[,,] gameGrid3D;

    private float playerRange = 10;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        // CreateGrid();        
        CreateGrid3D();        
    }
    public void Update()
    {
        //IsMouseOverAGridSpace();
        IsMouseOverAGridCube();
    }

    private void CreateGrid()
    {
        if(gridCellPrefab == null)
        {
            Debug.LogError("gridCellPrefab est null !");
            return;
        }

        gameGrid = new GameObject[height, width];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                gameGrid[x,y] = Instantiate(gridCellPrefab, new Vector3(x*gridSpaceSize, y*gridSpaceSize), Quaternion.identity);
                gameGrid[x,y].GetComponent<GridCell>().SetPosition(x,y);
                gameGrid[x,y].transform.parent = transform;
                gameGrid[x,y].gameObject.name = "Grid Space (x :"+ x +"; y : "+y+")";
            } 
        }

    }
    private void CreateGrid3D()
    {
        if(gridCubePrefab == null)
        {
            Debug.LogError("gridCubePrefab est null !");
            return;
        }

        gameGrid3D = new GameObject[height, width, depth];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if(y != 0)
                        continue;
                    gameGrid3D[x,y,z] = Instantiate(gridCubePrefab, new Vector3(x*gridSpaceSize, y*gridSpaceSize, z*gridSpaceSize), Quaternion.identity);
                    gameGrid3D[x,y,z].GetComponent<GridCube>().SetPosition(x,y,z);
                    gameGrid3D[x,y,z].transform.parent = transform;
                    gameGrid3D[x,y,z].gameObject.name = "Grid Cube (x :"+ x +"; y : "+y+"(z :"+ z +")";
                }
            } 
        }

    }

    public Vector2Int GetGridPosFromWorldPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPos.y / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(y, 0, height);

        return new Vector2Int(x,y);
    }

    public Vector3 GetWorldPosFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * gridSpaceSize;
        float y = gridPos.y * gridSpaceSize;

        return new Vector3(x,0,y);
    }

    Vector3 WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.y);
        int z = Mathf.RoundToInt(worldPosition.z);
        return new Vector3(x, y, z);
    }

    Vector3 GridToWorld(Vector3 gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
    }

    public bool IsMouseOverAGridSpace()
    {
        var main = Camera.main.transform;
        //Debug.DrawRay(main.position, main.forward*100, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(main.position, main.forward, out hit, 100f)) 
        {
            SpriteRenderer sr = hit.transform.GetComponentInChildren<SpriteRenderer>();
            if(hit.transform.gameObject.tag == "GridCell" && sr != null)
            {
                hit.transform.gameObject.GetComponent<GridCell>().isBeingLooked = true;
                return true;
            }
        }
        return false;
    }

    public bool IsMouseOverAGridCube()
    {
        var main = Camera.main.transform;
        //Debug.DrawRay(main.position, main.forward*100, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(main.position, main.forward, out hit, 100f)) 
        {
            if(hit.transform.gameObject.tag == "GridCube")
            {
                hit.transform.gameObject.GetComponent<GridCube>().isBeingLooked = true;
                Debug.Log("je regarde" + hit.transform.gameObject.name);
                return true;
            }
        }
        return false;
    }

    void PlaceBlock(Vector3 gridPosition)
    {
        int x = (int)gridPosition.x;
        int y = (int)gridPosition.y;
        int z = (int)gridPosition.z;

        if (gameGrid3D[x, y, z] == null)
        {
            Vector3 worldPosition = GridToWorld(gridPosition);
            gameGrid3D[x, y, z] = Instantiate(gridCubePrefab, worldPosition, Quaternion.identity);
        }
    }

    void RemoveBlock(Vector3 gridPosition)
    {
        int x = (int)gridPosition.x;
        int y = (int)gridPosition.y;
        int z = (int)gridPosition.z;

        if (gameGrid3D[x, y, z] != null)
        {
            Destroy(gameGrid3D[x, y, z]);
            gameGrid3D[x, y, z] = null;
        }
    }


    public void PlaceBlock()
    {
        var main = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(main.position, main.forward, out hit, playerRange))
        {
            Vector3 gridPosition = WorldToGrid(hit.point + hit.normal * 0.5f);
            PlaceBlock(gridPosition);
        }
    }

    public void RemoveBlock()
    {
        var main = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(main.position, main.forward, out hit, playerRange))
        {
            if (hit.collider.CompareTag("GridCube"))
            {
                Vector3 gridPosition = WorldToGrid(hit.point - hit.normal * 0.5f);
                RemoveBlock(gridPosition);
            }
        }
    }

}
