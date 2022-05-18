using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRotator : MonoBehaviour
{
    public Vector3 rotation = Vector3.zero;
    private void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
