using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [SerializeField]
    private Vector3 eatenPostition;
    [SerializeField]
    private Material primaryEyeMat;
    [SerializeField]
    private Material frightenedEyeMat;

    private GhostController ghost;
    private Vector3 defaultPosition;
    private Renderer rend;

    void Start()
    {
        ghost = GetComponentInParent<GhostController>();
        ghost.onGhostScared += ChangeToAfraid;
        ghost.onGhostRestored += ChangeToNormal;
        ghost.onGhostEaten += ChangeToEaten;

        defaultPosition = transform.localPosition;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = primaryEyeMat;
    }

    void ChangeToAfraid()
    {
        rend.sharedMaterial = frightenedEyeMat;
    }

    void ChangeToEaten()
    {
        transform.localPosition = eatenPostition;
        rend.sharedMaterial = frightenedEyeMat;
    }

    void ChangeToNormal()
    {
        transform.localPosition = defaultPosition;
        rend.sharedMaterial = primaryEyeMat;
    }
}
