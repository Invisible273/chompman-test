using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    private Vector3 movementVector = Vector3.zero;
    private Cell.Direction prevDir = Cell.Direction.Up;

    public void SetMovementVector(Vector2 movement)
    {
        if (movement.x == 0 && movement.y == 0)
            movementVector = Vector3.zero;
        else if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            if(movement.x > 0)
                movementVector = Vector3.right;
            else
                movementVector = Vector3.left;
        } 
        else
        {
            if(movement.y > 0)
                movementVector = Vector3.forward;
            else
                movementVector = Vector3.back;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (transform.position == destination)
        { 
            ChompmanGame.instance.PlayerMoved(prevDir);
            if (movementVector != Vector3.zero && currentCell.GetPathBool(movementVector))
            {
                destinationCell = currentCell.GetNeighbour(movementVector);
                destination = destinationCell.Vector3CoordCompletion(transform.position.y);
                prevDir = Cell.GetDirectionFromVector3(movementVector);
            }
        }           
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(other.gameObject.CompareTag("Pellet"))
        {
            ChompmanGame.instance.ObjectTaken(other.gameObject.tag);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("PowerUp"))
        {
            ChompmanGame.instance.PowerUpTaken();
            ChompmanGame.instance.ObjectTaken(other.gameObject.tag);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Ghost"))
        {
            GhostController ghost = other.gameObject.GetComponent<GhostController>();
            if (ghost.currentState == GhostController.GhostState.Eaten)
                return;
            else if (ghost.currentState == GhostController.GhostState.Frightened)
            {
                ChompmanGame.instance.ObjectTaken(other.gameObject.tag);
                ghost.GhostEaten();
            }
            else
            {
                ChompmanGame.instance.PlayerDied();
                ChompmanGame.instance.GameEnded(false);
                Destroy(gameObject);
            }
        }
    }
}
 