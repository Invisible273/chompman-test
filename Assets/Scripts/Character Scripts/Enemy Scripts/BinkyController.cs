using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinkyController : GhostController
{
    protected override Cell GetChaseTarget()
    {
        Cell target = playerCell;
        Cell.HighLightCell(target.coordinates, new Color((float)254/255, (float)161/255, 0, 1));
        return playerCell;
    }
}
