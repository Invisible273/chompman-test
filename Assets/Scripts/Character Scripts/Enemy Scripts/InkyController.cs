using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyController : GhostController
{
    private GhostController binky;
    private CellSet grid;
    protected override void Start()
    {
        grid = ChompmanGame.instance.pathFindGrid;
        base.Start();
    }

    public void SetBinky(BinkyController binkyController)
    {
        binky = binkyController;
    }

    protected override Cell GetChaseTarget()
    {
        Cell pivotPoint = TryGetCellByDir(playerCell, playerMoveDir, 1);
        Cell target = TryGetCellByCoords(pivotPoint, -binky.currentCell.coordinates);
        Cell.HighLightCell(target.coordinates, Color.cyan);
        return target;
    }
        
    private Cell TryGetCellByDir(Cell originCell, Cell.Direction dir, int tryCells)
    {
        Cell target = originCell;
        Vector2 targetCoords = originCell.coordinates + Cell.GetVector2FromDirection(dir)*tryCells*ChompmanGame.CELL_SIZE;
        if(!grid.GetCellByCoords(targetCoords, out target))
            return TryGetCellByDir(playerCell, dir, tryCells - (int)Mathf.Sign(tryCells));
        else
            return target;
    }

    private Cell TryGetCellByCoords(Cell originCell, Vector2 tryCoords)
    {
        Cell cellX = TryGetCellByDir(originCell, Cell.Direction.Right, (int)(originCell.coordinates.x + tryCoords.x));
        return TryGetCellByDir(cellX, Cell.Direction.Up, (int)(originCell.coordinates.y + tryCoords.y));
    }
}
