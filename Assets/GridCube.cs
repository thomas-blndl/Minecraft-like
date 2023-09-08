using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCube : MonoBehaviour
{
    public static GridCube Instance;
    private int posX;
    private int posY;
    private int posZ;

    //Save a reference to the gameobject that gets placed on this cell
    public GameObject objectInThisGridSpace = null;

    public bool isOccupied = false;
    public bool isBeingLooked = false;

    private Renderer cubeRenderer;

    public void Start()
    {
        Instance = this;
        cubeRenderer = GetComponent<Renderer>();
    }

    public void Update()
    {
        //GetComponentInChildren<SpriteRenderer>().color = isBeingLooked ? Color.green : Color.white;
        if (isBeingLooked)
        {
            cubeRenderer.material.color = Color.green;
            isBeingLooked = false;
        }
        else
        {
            cubeRenderer.material.color = Color.white;
        }
        
    }

    public void SetPosition(int x, int y, int z)
    {
        posX = x;
        posY = y;
        posZ = z;
    }

    public Vector3Int GetPosition()
    {
        return new Vector3Int(posX, posY, posZ);
    }
}
