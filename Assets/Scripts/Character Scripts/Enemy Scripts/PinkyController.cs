using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyController : GhostController
{
    private CellSet grid;
    protected override void Start()
    {
        grid = ChompmanGame.instance.pathFindGrid;
        base.Start();
    }
    protected override Cell GetChaseTarget()
    {
        Cell target = TryGetChaseTarget(2);
        Cell.HighLightCell(target.coordinates, new Color((float)232/255, 0, (float)254/255, 1));
        return target;
    }

    private Cell TryGetChaseTarget(int tryCells)
    {
        Cell target = playerCell;
        Vector2 targetCoords = playerCell.coordinates + Cell.GetVector2FromDirection(playerMoveDir)*tryCells*ChompmanGame.CELL_SIZE;
        if(!grid.GetCellByCoords(targetCoords, out target))
            return TryGetChaseTarget(tryCells - 1);
        else
            return target;
    }
}
