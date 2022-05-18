using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaveController : GhostController
{
    protected override Cell GetChaseTarget()
    {
        Cell target = playerCell;
        float distance = Vector2.Distance(playerCell.coordinates, currentCell.coordinates);
        if (distance < ChompmanGame.CELL_SIZE*4)
        {
            target = scatterTarget;
        }  
        Cell.HighLightCell(target.coordinates, Color.green);
        return target;
    }
}
