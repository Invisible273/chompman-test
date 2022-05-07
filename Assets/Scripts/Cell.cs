using UnityEngine;

public class Cell
{
    private Vector2 _coords;
    public Vector2 coordinates => _coords;

    private Cell _upN;
    private Cell _downN;
    private Cell _rightN;
    private Cell _leftN;

    private bool _pathUp = false;
    private bool _pathDown = false;
    private bool _pathRight = false;
    private bool _pathLeft = false;

    public enum Direction{Up, Down, Right, Left};

    public Cell()
    {
        SetCoordinates(Vector2.zero);
    }

    public Cell(Vector2 coords)
    {
        SetCoordinates(coords);
    }

    public Cell(float x, float y)
    {
        SetCoordinates(x, y);
    }

    public Cell(Vector3 coords)
    {
        SetCoordinates(coords.x, coords.z);
    }

    public void SetCoordinates(Vector2 coords)
    {
        _coords.x = coords.x;
        _coords.y = coords.y;
    }

    public void SetCoordinates(float x, float y)
    {
        _coords.x = x;
        _coords.y = y;
    }

    public void SetNeighbour(ref Cell neighbour, Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                _upN = neighbour;
                break;
            case Direction.Down:
                _downN = neighbour;
                break;
            case Direction.Right:
                _rightN = neighbour;
                break;
            case Direction.Left:
                _leftN = neighbour;
                break;
            default: 
                Debug.LogError("Unexpected error while setting Cell neighbour.");
                break;
        }
        UpdatePathBools();   
    }

    public void SetNeighbour(ref Cell neighbour, Vector3 dir)
    {
        if (dir == Vector3.forward)
            _upN = neighbour;
        else if (dir == Vector3.back)
            _downN = neighbour;
        else if (dir == Vector3.right)
            _rightN = neighbour;
        else if (dir == Vector3.left)
            _leftN = neighbour;
        else
            Debug.LogError("Incorrect Vector3 passed while setting Cell neighbour.");

        UpdatePathBools();
    }

    public ref Cell GetNeighbour(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return ref _upN;
            case Direction.Down:
                return ref _downN;
            case Direction.Right:
                return ref _rightN;
            case Direction.Left:
                return ref _leftN;
            default: 
                Debug.LogError("Unexpected error while getting Cell neighbour.");
                return ref _upN;
        }   
    }

    public ref Cell GetNeighbour(Vector3 dir)
    {
        if (dir == Vector3.forward)
            return ref _upN;
        else if (dir == Vector3.back)
            return ref _downN;
        else if (dir == Vector3.right)
            return ref _rightN;
        else if (dir == Vector3.left)
            return ref _leftN;
        else
            Debug.LogError("Incorrect Vector3 passed while getting Cell neighbour.");
            return ref _upN;
    }

    public void ClearNeighbour(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                _upN = null;
                break;
            case Direction.Down:
                _downN = null;
                break;
            case Direction.Right:
                _rightN = null;
                break;
            case Direction.Left:
                _leftN = null;
                break;
            default: 
                Debug.LogError("Unexpected error while clearing Cell neighbour.");
                break;
        }
        UpdatePathBools();   
    }

    public void ClearNeighbour(Vector3 dir)
    {
        if (dir == Vector3.forward)
            _upN = null;
        else if (dir == Vector3.back)
            _downN = null;
        else if (dir == Vector3.right)
            _rightN = null;
        else if (dir == Vector3.left)
            _leftN = null;
        else
            Debug.LogError("Incorrect Vector3 passed while clearing Cell neighbour.");

        UpdatePathBools();
    }

    public void SetPathBool(Direction dir, bool value)
    {
        switch (dir)
        {
            case Direction.Up:
                _pathUp = value;
                break;
            case Direction.Down:
                _pathDown = value;
                break;
            case Direction.Right:
                _pathRight = value;
                break;
            case Direction.Left:
                _pathLeft = value;
                break;
            default: 
                Debug.LogError("Unexpected error while setting path bool.");
                break;
        }  
    }

    public bool GetPathBool(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return _pathUp;
            case Direction.Down:
                return _pathDown;
            case Direction.Right:
                return _pathRight;
            case Direction.Left:
                return _pathLeft;
            default: 
                Debug.LogError("Unexpected error while getting path bool.");
                return false;
        }  
    }

    private void UpdatePathBools()
    {
        foreach (Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(GetNeighbour(dir) == null)
                SetPathBool(dir, false);
            else
                SetPathBool(dir, true);
        }
    }

    public static Vector3 GetVector3FromDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Vector3.forward;
            case Direction.Down:
                return Vector3.back;
            case Direction.Right:
                return Vector3.right;
            case Direction.Left:
                return Vector3.left;
            default: 
                Debug.LogError("Unexpected error while getting Vector3 from Direction.");
                return Vector3.zero;
        }   
    }

    public static Vector2 GetVector2FromDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Right:
                return Vector2.right;
            case Direction.Left:
                return Vector2.left;
            default: 
                Debug.LogError("Unexpected error while getting Vector2 from Direction.");
                return Vector2.zero;
        }   
    }
}
