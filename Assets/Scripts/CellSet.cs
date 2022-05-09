using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSet
{
    private Cell defaultCell;
    private HashSet<Cell> _cells;

    public HashSet<Cell> Cells => _cells;
    private Dictionary<Vector2, Cell> _cellPosDict;
    public CellSet()
    {
        _cells = new HashSet<Cell>();
        _cellPosDict = new Dictionary<Vector2, Cell>();
        defaultCell = new Cell();
    }

    public void Add(Cell cell)
    {
        if(_cells.Add(cell))
        {
            _cellPosDict.Add(cell.coordinates, cell);
        }
    }

    public void Remove(Cell cell)
    {
        if(_cells.Remove(cell))
        {
            _cellPosDict.Remove(cell.coordinates);
        }
    }

    public bool GetCellByCoords(Vector2 coords, out Cell cellRef)
    {
        cellRef = defaultCell;
        if(_cellPosDict.ContainsKey(coords))
        {
            Cell targetCell = _cellPosDict[coords];
            _cells.TryGetValue(targetCell,out cellRef);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetCellNeighbour(Cell targetCell, Cell neighbour, Cell.Direction dir)
    {
        if(_cells.Contains(targetCell) && _cells.Contains(neighbour))
        {
            Cell actualCell = new Cell();
            Cell actualNeighbour = new Cell();
            _cells.TryGetValue(targetCell, out actualCell);
            _cells.TryGetValue(neighbour, out actualNeighbour);
            actualCell.SetNeighbour(ref actualNeighbour,dir);
            return true;
        }
        else
            return false;
    }
}
