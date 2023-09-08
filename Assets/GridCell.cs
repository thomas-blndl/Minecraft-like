using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public static GridCell Instance;
    private int posX;
    private int posY;

    //Save a reference to the gameobject that gets placed on this cell
    public GameObject objectInThisGridSpace = null;

    public bool isOccupied = false;
    public bool isBeingLooked = false;

    public void Start()
    {
        Instance = this;
    }

    public void Update()
    {
        GetComponentInChildren<SpriteRenderer>().color = isBeingLooked ? Color.green : Color.white;
        isBeingLooked = false;
    }

    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }
}
