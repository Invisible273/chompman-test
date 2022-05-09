using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;

    [HideInInspector]
    public Cell currentCell;
    private Cell destinationCell;
    Rigidbody rb;
    Vector3 destination = Vector3.zero;
    Vector3 movement_vector = Vector3.zero;
    Vector2 last_movement_attempt = Vector2.zero;
    bool has_teleported;

    bool aiEnabled = false;
    Cell[] targets;
    Cell currentTarget;
    int currentTargetIndex;
    Cell.Direction prevDir;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        destinationCell = currentCell;
        destination = transform.position;
        InitializeAI();
    }

    void InitializeAI()
    {
        aiEnabled = true;
        Transform[] targetsTransform = GameObject.Find("Pickups/PowerUps").GetComponentsInChildren<Transform>();
        targets = new Cell[targetsTransform.Length - 1];
        for(int i = 1; i < targetsTransform.Length; i++)
        {
            ChompmanGame.instance.pathFindGrid.GetCellByCoords(Cell.GetCoordsFromVector3(targetsTransform[i].position),out targets[i-1]);
        }
        currentTargetIndex = 0;
        currentTarget = targets[currentTargetIndex];
        prevDir = Cell.Direction.Up;
    }

    void CalculateMovement()
    {
        float travelDistance = float.MaxValue;
        Cell.Direction tempDir = prevDir;
        foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(prevDir == Cell.GetOppositeDirection(dir) || currentCell.GetNeighbour(dir) == null)
                continue;
            float distance = Vector2.Distance(currentCell.GetNeighbour(dir).coordinates,currentTarget.coordinates);
            if (distance < travelDistance)
            {
                travelDistance = distance;
                destinationCell = currentCell.GetNeighbour(dir);
                tempDir = dir;
            }
        }
        prevDir = tempDir;
        destination = destinationCell.Vector3CoordCompletion(transform.position.y);
    }

    public void SetMovementVector(Vector2 movement)
    {
        if(!aiEnabled)
        {
            last_movement_attempt = movement;       
            if (movement.x == 0 && movement.y == 0)
                movement_vector = Vector3.zero;
            else if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                if(movement.x > 0)
                    movement_vector = Vector3.right;
                else
                    movement_vector = Vector3.left;
            } 
            else
            {
                if(movement.y > 0)
                    movement_vector = Vector3.forward;
                else
                    movement_vector = Vector3.back;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.MoveTowards(transform.position, destination, speed);
        rb.MovePosition(movement);

        if (transform.position == destination)
        { 
            if (has_teleported)
            {
                has_teleported = false;
                if(!ChompmanGame.instance.pathFindGrid.GetCellByCoords(Cell.GetCoordsFromVector3(transform.position),out currentCell))
                    Debug.LogError("Unexpected behaviour after teleportation.");
                destinationCell = currentCell;
            }
            else
                currentCell = destinationCell;
            if(!aiEnabled)
            {
                if (movement_vector != Vector3.zero && currentCell.GetPathBool(movement_vector))
                {
                    destinationCell = currentCell.GetNeighbour(movement_vector);
                    destination = destinationCell.Vector3CoordCompletion(transform.position.y);
                }
                else if (movement_vector == Vector3.zero && last_movement_attempt != Vector2.zero)
                {
                    SetMovementVector(last_movement_attempt);
                }
            }
            else
            {
                if(currentCell == currentTarget)
                {
                    currentTargetIndex++;
                    if(currentTargetIndex < targets.Length)
                        currentTarget = targets[currentTargetIndex];
                    else
                    {
                        aiEnabled = false;
                        Debug.Log("Victory!");
                    }       
                }
                CalculateMovement();
            }
        }           
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pellet"))
        {
            ChompmanGame.instance.ObjectTaken(other.gameObject.tag);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("PowerUp"))
        {
            ChompmanGame.instance.ObjectTaken(other.gameObject.tag);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Teleporter") && !has_teleported)  
        {   
            int target_index = other.gameObject.transform.GetSiblingIndex() > 0 ? 0 : 1;
            Transform next_teleporter = other.gameObject.transform.parent.GetChild(target_index);
            Vector3 teleport_target = next_teleporter.position - other.gameObject.transform.position;
            Vector3 offset = new Vector3(other.gameObject.transform.localScale.x*Mathf.Sign(-teleport_target.x), 0, 0);
            transform.Translate(teleport_target + offset);
            destination += teleport_target + offset;
            has_teleported = true;
        } 
    }
}
 