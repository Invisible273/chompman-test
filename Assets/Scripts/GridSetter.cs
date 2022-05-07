using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetter : MonoBehaviour
{
    [SerializeField]
    private GameObject probe;
    [SerializeField]
    private LayerMask collisionList;
    public HashSet<Cell> tiles = new HashSet<Cell>();
    private Dictionary<Vector2,CellView> cellDict = new Dictionary<Vector2, CellView>();
    private float probeRadius;

    void Start()
    {   
        //Function to set up clean CellViews in new level geometry
        //InitialCellViewSet();
        CellSet();
    }

    void InitialCellViewSet()
    {
        probeRadius = probe.GetComponent<SphereCollider>().radius * probe.transform.localScale.magnitude/2;
        foreach (CellView cellView in transform.gameObject.GetComponentsInChildren<CellView>())
        {
            bool canBeExited = false;
            Vector3 probeCenter = new Vector3(cellView.Cell.coordinates.x, probe.transform.position.y, cellView.Cell.coordinates.y);
            bool isSolid = Physics.Linecast(probeCenter + Vector3.up*ChompmanGame.CELL_SIZE, probeCenter, collisionList);
            if(isSolid)
                continue;
            foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
            {
                bool isValid = ValidateMovement(probeCenter, Cell.GetVector3FromDirection(dir));
                cellView.SetPathBool(dir, isValid);
                canBeExited = canBeExited || isValid;
            }
            cellView.isPassable = canBeExited;
        }
    }

    void CellSet()
    {
        foreach (CellView cellView in transform.gameObject.GetComponentsInChildren<CellView>())
        {
            cellDict.Add(cellView.Cell.coordinates,cellView);
        }
        foreach (CellView cellView in cellDict.Values)
        {
            if (cellView.isPassable)
            {
                AttemptTileAdd(cellView);
            }
        }
    }

    void AttemptTileAdd(CellView cellView)
    {
        if(!tiles.Contains(cellView.Cell))
        {
            tiles.Add(cellView.Cell);
        }
        foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(cellView.GetPathBool(dir) && cellView.Cell.GetNeighbour(dir) == null)
            {
                Vector2 neighbourCoordinates = cellView.Cell.coordinates + Cell.GetVector2FromDirection(dir)*ChompmanGame.CELL_SIZE;
                if(cellDict.ContainsKey(neighbourCoordinates))
                {
                    CellView neighbour = cellDict[neighbourCoordinates];
                    if(!tiles.TryGetValue(neighbour.Cell, out cellView.Cell.GetNeighbour(dir)))
                    {
                        AttemptTileAdd(neighbour);
                        if(!tiles.TryGetValue(neighbour.Cell, out cellView.Cell.GetNeighbour(dir)))
                        {
                            Debug.LogWarning("Unexpected behaviour while connecting neighbours.");
                        }
                    }
                }
                else
                {
                    cellView.SetPathBool(dir, false);
                    cellView.Cell.ClearNeighbour(dir);
                    Debug.LogWarning("Unexpected behaviour while checking Cell dictionary.");
                }
            }         
        }
        
    }
    bool ValidateMovement(Vector3 origin, Vector3 dir){
        float check_distance = ChompmanGame.CELL_SIZE + probeRadius;
        Vector3 central = Vector3.Normalize(dir)*(check_distance);
        Vector3 right = Vector3.Cross(Vector3.up,Vector3.Normalize(dir))*probeRadius;
        Vector3 left = Vector3.Cross(Vector3.down,Vector3.Normalize(dir))*probeRadius;

        bool hit_right = Physics.Linecast(origin, origin + central + right, collisionList);
        bool hit_left = Physics.Linecast(origin, origin + central + left, collisionList);
        Debug.DrawLine(origin,origin + central + right, Color.red, 0.1f);
        Debug.DrawLine(origin,origin + central + left, Color.red, 0.1f);
        return !(hit_right || hit_left);
    }
}
