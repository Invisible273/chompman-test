// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CharacterController : MonoBehaviour
// {
//     [SerializeField]
//     protected LayerMask collision_list;
//     [SerializeField]
//     protected float speed;
//     protected Rigidbody rb;
//     protected float radius;

//     protected Vector3 destination = Vector3.zero;
//     protected Vector3 movement_vector = Vector3.zero;
//     struct ValidDirections 
//     {
//         public bool up;
//         public bool down;
//         public bool left;
//         public bool right;
//     };
//     protected bool has_teleported = false;
//     protected virtual void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         radius = ChompmanGame.CELL_SIZE * 0.8f;
//         destination = transform.position;
//     }

//     public void SetMovementVector(Vector2 movement)
//     {
//         if (movement.x == 0 && movement.y == 0)
//             movement_vector = Vector3.zero;
//         else if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
//         {
//             if(movement.x > 0 && ValidateMovement(Vector3.right, ChompmanGame.CELL_SIZE))
//                 movement_vector = ChompmanGame.CELL_SIZE * Vector3.right;
//             else if(movement.x < 0 && ValidateMovement(Vector3.left, ChompmanGame.CELL_SIZE))
//                 movement_vector = ChompmanGame.CELL_SIZE * Vector3.left;
//             else
//                 movement_vector = Vector3.zero;
//         } 
//         else
//         {
//             if(movement.y > 0 && ValidateMovement(Vector3.forward, ChompmanGame.CELL_SIZE))
//                 movement_vector = ChompmanGame.CELL_SIZE * Vector3.forward;
//             else if(movement.y < 0 && ValidateMovement(Vector3.back, ChompmanGame.CELL_SIZE))
//                 movement_vector = ChompmanGame.CELL_SIZE * Vector3.back;
//             else
//                 movement_vector = Vector3.zero;
//         }
//     }

//     protected virtual void FixedUpdate()
//     {
//         Vector3 movement = Vector3.MoveTowards(transform.position, destination, speed);
//         rb.MovePosition(movement);

//         if ((Vector3)transform.position == destination)
//         {
//             if (has_teleported)
//                 has_teleported = false;
//             if (movement_vector.magnitude > 0 && ValidateMovement(movement_vector, move_distance))
//             {
//                 destination = (Vector3)transform.position + movement_vector;
//             }
//             else if (movement_vector.magnitude.Equals(0) && !last_movement_attempt.Equals(Vector2.zero))
//             {
//                 SetMovementVector(last_movement_attempt);
//             }
//         }           
//     }

//     bool CheckForObstacles(Vector3 dir, float mov_dist){
//         float check_distance = mov_dist + radius;
//         Vector3 pos = transform.position;
//         Vector3 central = Vector3.Normalize(dir)*(check_distance);
//         Vector3 right = Vector3.Cross(Vector3.up,Vector3.Normalize(dir))*radius;
//         Vector3 left = Vector3.Cross(Vector3.down,Vector3.Normalize(dir))*radius;

//         bool hit_right = Physics.Linecast(pos, pos + central + right, collision_list);
//         bool hit_left = Physics.Linecast(pos, pos + central + left, collision_list);
//         Debug.DrawLine(pos,pos + central + right, Color.red, 0.1f);
//         Debug.DrawLine(pos,pos + central + left, Color.red, 0.1f);
//         return !(hit_right || hit_left);
//     }

//     protected virtual void OnTriggerEnter(Collider other)
//     {
//         if (other.gameObject.CompareTag("Teleporter") && !has_teleported)  
//         {   
//             int target_index = other.gameObject.transform.GetSiblingIndex() > 0 ? 0 : 1;
//             Transform next_teleporter = other.gameObject.transform.parent.GetChild(target_index);
//             Vector3 teleport_target = next_teleporter.position - other.gameObject.transform.position;
//             Vector3 offset = new Vector3(other.gameObject.transform.localScale.x*Mathf.Sign(-teleport_target.x), 0, 0);
//             transform.Translate(teleport_target + offset);
//             destination += teleport_target + offset;
//             has_teleported = true;
//         } 
//     }
// }