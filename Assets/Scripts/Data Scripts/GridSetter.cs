using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetter : MonoBehaviour
{
    [SerializeField]
    private GameObject probe;
    [SerializeField]
    private LayerMask collisionList;
    public CellSet tiles = new CellSet();
    private float probeRadius;

    private void Start()
    {   
        //Function to set up clean CellViews in new level geometry
        //InitialCellViewSet();
        SetCellSet();
    }

    private void InitialCellViewSet()
    {
        probeRadius = probe.GetComponent<SphereCollider>().radius * probe.transform.localScale.magnitude/2;
        foreach (CellView cellView in transform.gameObject.GetComponentsInChildren<CellView>())
        {
            // Run if tiles are not alligned properly
            // float xPos = Mathf.Round(cellView.transform.localPosition.x);
            // float zPos = Mathf.Round(cellView.transform.localPosition.z);
            // Vector3 correctedPosition = new Vector3(xPos, cellView.transform.localPosition.y, zPos);
            // cellView.transform.Translate(correctedPosition - cellView.transform.localPosition);

            Vector3 probeCenter = new Vector3(cellView.Cell.coordinates.x, probe.transform.position.y, cellView.Cell.coordinates.y);
            bool isSolid = Physics.Linecast(probeCenter + Vector3.up*ChompmanGame.CELL_SIZE, probeCenter, collisionList);
            if(isSolid)
                continue;
            foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
            {
                bool isValid = ValidateMovement(probeCenter, Cell.GetVector3FromDirection(dir));
                cellView.SetPathBool(dir, isValid);
            }
        }
    }

    private void SetCellSet()
    {
        foreach (CellView cellView in transform.gameObject.GetComponentsInChildren<CellView>())
        {
            tiles.Add(cellView.Cell);
        }
        foreach (CellView cellView in transform.gameObject.GetComponentsInChildren<CellView>())
        {
            SetNeighbours(cellView);
        }
    }

    private void SetNeighbours(CellView cellView)
    {
        foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(cellView.GetPathBool(dir))
            {
                Vector2 neighbourCoordinates = cellView.Cell.coordinates + Cell.GetVector2FromDirection(dir)*ChompmanGame.CELL_SIZE;
                Cell neighbour = new Cell();
                if(tiles.GetCellByCoords(neighbourCoordinates,out neighbour))
                {
                    if(!tiles.SetCellNeighbour(cellView.Cell, neighbour, dir))
                        Debug.LogWarning("Unexpected behaviour while setting neighbour " + neighbourCoordinates + " of Cell " + cellView.Cell.coordinates);
                }
                else
                {
                    cellView.SetPathBool(dir, false);
                    Debug.LogWarning("Unexpected behaviour while checking neighbour " + neighbourCoordinates + " of Cell " + cellView.Cell.coordinates);
                }
            }         
        }
        
    }
    private bool ValidateMovement(Vector3 origin, Vector3 dir){
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
