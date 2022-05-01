using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    LayerMask collision_list;
    public float speed = 0;
    public float grid_step = 0;
    private Rigidbody rb;
    private float radius;
    private float movement_x;
    private float movement_y;
    Vector3 destination = Vector3.zero;
    Vector3 movement_vector = Vector3.zero;
    private float move_distance;
    private float check_distance;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        Debug.Log(radius);
        destination = transform.position;

        move_distance = radius;
        check_distance = move_distance + radius;
    }

    public void SetMovementVector(Vector2 movement)
    {
        movement_x = movement.x;
        movement_y = movement.y;
        if (movement_x == 0 && movement_y == 0)
             movement_vector = Vector3.zero;
        else if (Mathf.Abs(movement_x) > Mathf.Abs(movement_y))
        {
            if(movement_x > 0 && ValidateMovement(Vector3.right, check_distance))
                movement_vector = move_distance * Vector3.right;
            if(movement_x < 0 && ValidateMovement(Vector3.left, check_distance))
                movement_vector = move_distance * Vector3.left;
        } 
        else
        {
            if(movement_y > 0 && ValidateMovement(Vector3.forward, check_distance))
                movement_vector = move_distance * Vector3.forward;
            if(movement_y < 0 && ValidateMovement(Vector3.back, check_distance))
                movement_vector = move_distance * Vector3.back;
        }
    }

    void FixedUpdate()
    {
        // Vector3 movement = new Vector3(movement_x, 0.0f, movement_y);
        // rb.AddForce(movement * speed);
        Vector3 movement = Vector3.MoveTowards(transform.position, destination, speed);
        rb.MovePosition(movement);

        if ((Vector3)transform.position == destination && ValidateMovement(Vector3.Normalize(movement_vector), check_distance)){
            destination = (Vector3)transform.position + movement_vector;
        }        
    }

    bool ValidateMovement(Vector3 dir, float check_distance){
        Vector3 pos = transform.position;
        Vector3 central = Vector3.Normalize(dir)*(check_distance);
        Vector3 right = Vector3.Cross(Vector3.up,Vector3.Normalize(dir))*radius;
        Vector3 left = Vector3.Cross(Vector3.down,Vector3.Normalize(dir))*radius;

        bool hit_right = Physics.Linecast(pos, pos + central + right, collision_list);
        bool hit_left = Physics.Linecast(pos, pos + central + left, collision_list);
        Debug.DrawLine(pos,pos + central + right, Color.red, 0.1f);
        Debug.DrawLine(pos,pos + central + left, Color.red, 0.1f);
        Debug.Log(!(hit_right || hit_left));
        return !(hit_right || hit_left);
    }
}
 