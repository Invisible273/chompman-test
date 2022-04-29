using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // [SerializeField]
    // LayerMask collision_ignore;
    public float speed = 0;
    // public float grid_step = 0;
    private Rigidbody rb;
    private float movement_x;
    private float movement_y;
    // Vector3 destination = Vector3.zero;
    // Vector3 movement_vector = Vector3.zero;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // destination = transform.position;
    }

    public void SetMovementVector(Vector2 movement)
    {
        movement_x = movement.x;
        movement_y = movement.y;
        // if (movement_x == 0 && movement_y == 0)
        //     movement_vector = Vector3.zero;
        // if (Mathf.Abs(movement_x) > Mathf.Abs(movement_y))
        // {
        //     if(movement_x > 0 && ValidateMovement(grid_step * Vector3.right))
        //         movement_vector = grid_step * Vector3.right;
        //     if(movement_x < 0 && ValidateMovement(grid_step * Vector3.left))
        //         movement_vector = grid_step * Vector3.left;
        // } 
        // else
        // {
        //     if(movement_y > 0 && ValidateMovement(grid_step * Vector3.forward))
        //         movement_vector = grid_step * Vector3.forward;
        //     if(movement_y < 0 && ValidateMovement(grid_step * Vector3.back))
        //         movement_vector = grid_step * Vector3.back;
        // }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movement_x, 0.0f, movement_y);
        rb.AddForce(movement * speed);
        // Vector3 movement = Vector3.MoveTowards(transform.position, destination, speed);
        // rb.MovePosition(movement);

        // if ((Vector3)transform.position == destination)
        //     destination = (Vector3)transform.position + movement_vector;
    }

    bool ValidateMovement(Vector3 dir){
        Vector3 pos = transform.position;
        bool hit = Physics.Linecast(pos, pos + dir + Vector3.Normalize(dir)*2.0f);
        Debug.Log(hit);
        return !hit;
    }
}
