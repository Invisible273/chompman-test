using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    protected float speed = 0;

    [HideInInspector]
    public Cell currentCell;
    protected Cell destinationCell;
    protected Vector3 destination = Vector3.zero;
    protected Rigidbody rb;
    private bool hasTeleported;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        destinationCell = currentCell;
        destination = transform.position;
    }

    protected virtual void FixedUpdate()
    {
        Vector3 movement = Vector3.MoveTowards(transform.position, destination, speed);
        rb.MovePosition(movement);

        if (transform.position == destination)
        { 
            if (hasTeleported)
            {
                hasTeleported = false;
                if(!ChompmanGame.instance.pathFindGrid.GetCellByCoords(Cell.GetCoordsFromVector3(transform.position),out destinationCell))
                    Debug.LogError("Unexpected behaviour after teleportation.");
            }
            currentCell = destinationCell;
        }       
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Teleporter") && !hasTeleported)  
        {   
            Transform thisTeleporter = other.gameObject.transform;
            int targetIndex = thisTeleporter.GetSiblingIndex() > 0 ? 0 : 1;
            Transform otherTeleporter = other.gameObject.transform.parent.GetChild(targetIndex);
            Vector3 teleportDest = otherTeleporter.position - thisTeleporter.position;
            Vector3 offset = new Vector3(thisTeleporter.localScale.x*Mathf.Sign(-teleportDest.x)*ChompmanGame.CELL_SIZE, 0, 0);
            transform.Translate(teleportDest + offset, Space.World);
            destination += teleportDest + offset;
            hasTeleported = true;
        } 
    }
}
    // Leftover gatherer AI script
    // bool aiEnabled = false;
    // Cell[] targets;
    // Cell currentTarget;
    // int currentTargetIndex;
    // Cell.Direction prevDir;

    // void InitializeAI()
    // {
    //     aiEnabled = true;
    //     Transform[] targetsTransform = GameObject.Find("Pickups/PowerUps").GetComponentsInChildren<Transform>();
    //     targets = new Cell[targetsTransform.Length - 1];
    //     for(int i = 1; i < targetsTransform.Length; i++)
    //     {
    //         ChompmanGame.instance.pathFindGrid.GetCellByCoords(Cell.GetCoordsFromVector3(targetsTransform[i].position),out targets[i-1]);
    //     }
    //     currentTargetIndex = 0;
    //     currentTarget = targets[currentTargetIndex];
    //     prevDir = Cell.Direction.Up;
    // }

    // void CalculateMovement()
    // {
    //     if(!aiEnabled)
    //         return;
    //     float travelDistance = float.MaxValue;
    //     Cell.Direction tempDir = prevDir;
    //     foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
    //     {
    //         if(dir == Cell.GetOppositeDirection(prevDir) || !currentCell.GetPathBool(dir))
    //             continue;
    //         float distance = Vector2.Distance(currentCell.GetNeighbour(dir).coordinates,currentTarget.coordinates);
    //         if (distance < travelDistance)
    //         {
    //             travelDistance = distance;
    //             destinationCell = currentCell.GetNeighbour(dir);
    //             tempDir = dir;
    //         }
    //     }
    //     prevDir = tempDir;
    //     destination = destinationCell.Vector3CoordCompletion(transform.position.y);
    // }
            // In FixedUpdate()
            //     for(int i = 0; i < targets.Length; i++)
            //     {
            //         if(targets[i] == currentCell)
            //         {
            //             targets[i] = null;
            //         }
            //     }
            //     while(targets[currentTargetIndex] == null)
            //     {
            //         currentTargetIndex++;
            //         if(currentTargetIndex < targets.Length)
            //             currentTarget = targets[currentTargetIndex];
            //         else
            //         {
            //             aiEnabled = false;
            //             Debug.Log("Victory!");
            //             break;
            //         }       
            //     }
            //     CalculateMovement();

    // Leftover advanced AI script
    // void CalculateMovementAdvanced()
    // {
    //     if(!aiEnabled)
    //         return;
    //     int shortestDist = int.MaxValue;
    //     Cell.Direction tempDir = prevDir;
    //     foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
    //     {
    //         if(prevDir == Cell.GetOppositeDirection(dir) || !currentCell.GetPathBool(dir))
    //             continue;
    //         int distance = CalculatePathLength(currentCell.GetNeighbour(dir), dir, 0) + 1;
    //         if (distance < shortestDist)
    //         {
    //             shortestDist = distance;
    //             destinationCell = currentCell.GetNeighbour(dir);
    //             tempDir = dir;
    //         }
    //     }
    //     prevDir = tempDir;
    //     destination = destinationCell.Vector3CoordCompletion(transform.position.y);
    // }

    // int CalculatePathLength(Cell checkCell, Cell.Direction lastDir, int iterNum)
    // {
    //     iterNum++;
    //     if(checkCell == currentTarget || iterNum > 20)
    //     {
    //         return 0;
    //     }
    //     else
    //     {
    //         int shortestDist = int.MaxValue;
    //         foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
    //         {
    //             if(lastDir == Cell.GetOppositeDirection(dir) || !currentCell.GetPathBool(dir))
    //                 continue;
    //             int distance = CalculatePathLength(currentCell.GetNeighbour(dir), dir, iterNum) + 1;
    //             if (distance < shortestDist)
    //             {
    //                 shortestDist = distance;
    //             }
    //         }
    //         return shortestDist;
    //     }   
    // }
 