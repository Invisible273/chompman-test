using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public bool isPassable = false;
    [SerializeField]
    private bool pathUp = false;
    [SerializeField]
    private bool pathRight = false;
    [SerializeField]
    private bool pathLeft = false;
    [SerializeField]
    private bool pathDown = false;

    private Cell cell;
    public Cell Cell => cell;

    private void Awake() 
    {
        cell = new Cell(transform.position);
    }

    public Vector3 GetVector3Position()
    {
        return new Vector3(cell.coordinates.x,transform.position.y,cell.coordinates.y);
    }

    public void UpdatePathBools()
    {
        if(!isPassable) return;
        foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(cell.GetPathBool(dir))
                SetPathBool(dir, true);
            else
                SetPathBool(dir, false);
        }
    }

    public void SetPathBool(Cell.Direction dir, bool value)
    {
        switch (dir)
        {
            case Cell.Direction.Up:
                pathUp = value;
                break;
            case Cell.Direction.Down:
                pathDown = value;
                break;
            case Cell.Direction.Right:
                pathRight = value;
                break;
            case Cell.Direction.Left:
                pathLeft = value;
                break;
            default: 
                Debug.LogError("Unexpected error while setting path bool.");
                break;
        }  
    }

    public bool GetPathBool(Cell.Direction dir)
    {
        switch (dir)
        {
            case Cell.Direction.Up:
                return pathUp;
            case Cell.Direction.Down:
                return pathDown;
            case Cell.Direction.Right:
                return pathRight;
            case Cell.Direction.Left:
                return pathLeft;
            default: 
                Debug.LogError("Unexpected error while getting path bool.");
                return false;
        }  
    }
}
