using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : CharacterController
{
    private const float FRIGHTENED_TIME = 8.0f;
    [SerializeField]
    private GameObject ghostBody;
    [SerializeField]
    private Cell.Direction[] directionsToRespawn;
    
    public enum GhostState {
        Chase,
        Scatter,
        Frightened,
        Eaten,
        Passive
    }

    [HideInInspector]
    public GhostState currentState = GhostState.Passive;
    protected Cell playerCell;
    protected Cell.Direction playerMoveDir;
    protected Cell scatterTarget;
    private Cell ghostHouseEntrance;
    private Cell currentTarget;
    private Cell.Direction prevDir;
    private float[] scatterChaseSequence = {7.0f, 20.0f, 7.0f, 20.0f, 5.0f, 20.0f, 5.0f};
    private int scatterChaseProgress;
    private float timeSinceSequenceChange;
    private bool sequenceTimerPaused;
    private float frightenedTimer;
    private bool isRespawning;
    private int currentRespawnStep;

    protected override void Start()
    {
        base.Start();
        prevDir = Cell.Direction.Up;
        InitializeAI();
    }

    public void GhostSetup(Cell spawnCell, Cell sctrTarget, float spd)
    {
        currentCell = spawnCell;
        ghostHouseEntrance = GetCellFromBacktrack(currentCell, directionsToRespawn, directionsToRespawn.Length);
        scatterTarget = sctrTarget;
        if(speed == 0)
        {
            speed = spd;
        }
    }

    private Cell GetCellFromBacktrack(Cell originCell, Cell.Direction[] directions, int directionStep)
    {
        directionStep--;
        if(directionStep >= 0)
        {
            Cell pathCell = originCell.GetNeighbour(Cell.GetOppositeDirection(directions[directionStep]));
            return GetCellFromBacktrack(pathCell, directions, directionStep);
        }
        else
            return originCell;
    }

    private void InitializeAI()
    {
        ChompmanGame.instance.onPlayerMoved += OnPlayerCellUpdate;
        ChompmanGame.instance.onPlayerDied += OnPlayerChased;
        ChompmanGame.instance.onPowerUp += OnPlayerPoweredUp;

        ChangeStateTo(GhostState.Passive);
        isRespawning = true;
        currentRespawnStep = directionsToRespawn.Length;
        timeSinceSequenceChange = 0;
        scatterChaseProgress = 0;
    }

    private void OnPlayerCellUpdate(ref Cell currentPlayerCell, Cell.Direction moveDir)
    {
        playerCell = currentPlayerCell;
        playerMoveDir = moveDir;
    }

    private void OnPlayerChased()
    {
        ChangeStateTo(GhostState.Passive);
        Destroy(gameObject);
    }

    private void OnPlayerPoweredUp()
    {
        if (currentState != GhostState.Eaten && currentState != GhostState.Passive)
            GhostScared();
    }

    private void ChangeStateTo(GhostState state)
    {
        switch(state)
        {
            case GhostState.Chase:
                currentTarget = GetChaseTarget();
                sequenceTimerPaused = false;
                if(currentState == GhostState.Frightened || currentState == GhostState.Scatter)
                {
                    TurnAround(); 
                }
                break;
            case GhostState.Scatter:
                currentTarget = scatterTarget;
                sequenceTimerPaused = false;
                if(currentState == GhostState.Chase)
                {
                    TurnAround();
                }
                break;
            case GhostState.Frightened:
                sequenceTimerPaused = true;
                TurnAround();
                frightenedTimer = 0;
                break;
            case GhostState.Eaten:
                currentTarget = ghostHouseEntrance;
                sequenceTimerPaused = true;
                break;
            case GhostState.Passive:
                currentTarget = currentCell;
                sequenceTimerPaused = true;
                break;
            default:
                currentTarget = currentCell;
                Debug.LogError("Unexpected behaviour when changing ghost state.");
                break;
        }
        currentState = state;
    }

    protected virtual Cell GetChaseTarget()
    {
        return playerCell;
    }

    private void CalculateMovement()
    {
        float travelDistance = float.MaxValue;
        Cell.Direction tempDir = prevDir;
        foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(dir == Cell.GetOppositeDirection(prevDir) || !currentCell.GetPathBool(dir))
                continue;
            float distance = Vector2.Distance(currentCell.GetNeighbour(dir).coordinates,currentTarget.coordinates);
            if (distance < travelDistance)
            {
                travelDistance = distance;
                destinationCell = currentCell.GetNeighbour(dir);
                tempDir = dir;
            }
        }
        RotateToDir(tempDir);
        destination = destinationCell.Vector3CoordCompletion(transform.position.y);
    }

    private void CalculateFrightenedMovement()
    {
        List<Cell.Direction> openDirs = new List<Cell.Direction>();
        Cell.Direction chosenDir = prevDir;
        foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
        {
            if(dir == Cell.GetOppositeDirection(prevDir) || !currentCell.GetPathBool(dir))
                continue;
            else
                openDirs.Add(dir);
        }
        chosenDir = openDirs[UnityEngine.Random.Range(0, openDirs.Count)];
        RotateToDir(chosenDir);
        destinationCell = currentCell.GetNeighbour(chosenDir);
        destination = destinationCell.Vector3CoordCompletion(transform.position.y);
    }

    private void RotateToDir(Cell.Direction dir)
    {
        if (dir == Cell.GetOppositeDirection(prevDir))
        {
            transform.Rotate(0.0f,180.0f,0.0f);
        }
        else
        {
            transform.localRotation *= Quaternion.FromToRotation(Cell.GetVector3FromDirection(prevDir),Cell.GetVector3FromDirection(dir));
        }
        prevDir = dir;
    }

    private void NextRespawnStep()
    {
        currentRespawnStep++;
        if (currentRespawnStep < directionsToRespawn.Length)
        {
            Vector2 destinationCoords = currentCell.coordinates + Cell.GetVector2FromDirection(directionsToRespawn[currentRespawnStep]);
            if(!ChompmanGame.instance.pathFindGrid.GetCellByCoords(destinationCoords, out destinationCell))
                Debug.LogError("Error while moving to respawn point.");
            destination = destinationCell.Vector3CoordCompletion(transform.position.y);
        }
        else
        {
            GhostRestored();
            ChangeStateTo(GhostState.Passive);
        }
    }

    private void NextReturnStep()
    {
        currentRespawnStep--;
        if (currentRespawnStep >= 0)
        {
            Cell.Direction movementDirection = Cell.GetOppositeDirection(directionsToRespawn[currentRespawnStep]);
            Vector2 destinationCoords = currentCell.coordinates + Cell.GetVector2FromDirection(movementDirection);
            if(!ChompmanGame.instance.pathFindGrid.GetCellByCoords(destinationCoords, out destinationCell))
                Debug.LogError("Error while moving from respawn point.");
            destination = destinationCell.Vector3CoordCompletion(transform.position.y);
            RotateToDir(movementDirection);
        }
        else
        {
            isRespawning = false;
            ScatterOrChase();
        }
    }

    private void TurnAround()
    {
        RotateToDir(Cell.GetOppositeDirection(prevDir));
        if (transform.position != destination)
        {
            Cell tempCell = destinationCell;
            destinationCell = currentCell;
            currentCell = tempCell;

            destination = destinationCell.Vector3CoordCompletion(transform.position.y);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (transform.position == destination)
        {
            switch(currentState)
            {
                case GhostState.Chase:
                    currentTarget = GetChaseTarget();
                    CalculateMovement();  
                    break;
                case GhostState.Scatter:
                    CalculateMovement();
                    break;
                case GhostState.Frightened:
                    CalculateFrightenedMovement();
                    break;
                case GhostState.Eaten:
                    if(currentCell == ghostHouseEntrance && !isRespawning)
                    {
                        isRespawning = true;
                        currentRespawnStep = -1;
                    }

                    if(isRespawning)
                        NextRespawnStep();
                    else
                        CalculateMovement(); 
                    break;
                case GhostState.Passive:
                    if(isRespawning)
                        NextReturnStep();
                    break;
                default:
                    Debug.LogError("Unexpected behaviour during ghost movement update.");
                    break;
            }
        }           
    }

    private void Update()
    {
        if (!sequenceTimerPaused)
        {
            timeSinceSequenceChange += Time.deltaTime;
            if (scatterChaseProgress < scatterChaseSequence.Length && timeSinceSequenceChange >= scatterChaseSequence[scatterChaseProgress])
            {
                timeSinceSequenceChange = 0;
                scatterChaseProgress++;
                ScatterOrChase();
            }
        }
        if (currentState == GhostState.Frightened)
        {
            frightenedTimer += Time.deltaTime;
            if(frightenedTimer >= FRIGHTENED_TIME)
            {
                GhostRestored();
                ScatterOrChase();
            }
        }
    }

    private void ScatterOrChase()
    {
        if (scatterChaseProgress % 2 == 0)
        {
            ChangeStateTo(GhostState.Scatter);
        }
        else
        {
            ChangeStateTo(GhostState.Chase);
        }
    }

    public event Action onGhostScared;
    public void GhostScared()
    {
        if (onGhostScared != null)
        {
            onGhostScared();
        }
        ChangeStateTo(GhostState.Frightened);
    }
    public event Action onGhostRestored;
    public void GhostRestored()
    {
        if (onGhostRestored != null)
        {
            onGhostRestored();
        }
        ghostBody.SetActive(true);
    }
    public event Action onGhostEaten;
    public void GhostEaten()
    {
        if (onGhostEaten != null)
        {
            onGhostEaten();
        }
        ghostBody.SetActive(false);
        ChangeStateTo(GhostState.Eaten);
    }
}
