using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChompmanGame : MonoBehaviour
{
    public const float CELL_SIZE = 1.0f;
    private const int MAX_SCORE_SIZE = 6;
    private const int PELLET_VALUE = 100;
    private const int GHOST_VALUE = 500;
    private const int POWERUP_VALUE = 1000;
    [SerializeField]
    GameObject mainMenu;

    [SerializeField]
    GameObject gameUI;

    [SerializeField]
    GameObject chompman;

    [SerializeField]
    GameObject pelletContainer;

    [SerializeField]
    GameObject powerUpContainer;

    [SerializeField]
    CellView initialSpawn;

    [SerializeField]
    bool randomSpawn = false;
    [SerializeField]
    GridSetter grid;
    [SerializeField]
    GameObject[] ghostPrefabs;
    [SerializeField]
    float ghostSpeed;
    [SerializeField]
    CellView[] ghostSpawns;
    [SerializeField]
    CellView[] ghostScatterPoints;
    
    [HideInInspector]
    public CellSet pathFindGrid;
    public static ChompmanGame instance = null;

    GameState gameState;
    enum GameState{
        MenuState,
        PlayState
    }

    PlayerController playerController;
    GameObject player;
    PlayerInput playerInput;
    int currentScore = 0;
    TextMeshProUGUI scoreBoard = null;
    int pelletCount;
    int powerUpCount;
    List<GameObject> pellets = new List<GameObject>();
    List<GameObject> powerUps = new List<GameObject>();
    BinkyController binky;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        bool parentSkipped = false;
        foreach(Transform pelletTransform in pelletContainer.GetComponentsInChildren<Transform>())
        {
            if (parentSkipped)
                pellets.Add(pelletTransform.gameObject);
            else
                parentSkipped = true;
        }
        parentSkipped = false;
        foreach(Transform powerUpTransform in powerUpContainer.GetComponentsInChildren<Transform>())
        {
            if (parentSkipped)
                powerUps.Add(powerUpTransform.gameObject);
            else
                parentSkipped = true;
        }
        scoreBoard = gameUI.GetComponentInChildren<TextMeshProUGUI>();
        pathFindGrid = grid.tiles;
        SwitchStateTo(GameState.MenuState);
    }

    private void SwitchStateTo(GameState state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.MenuState:
                playerInput.SwitchCurrentActionMap("Menu");
                mainMenu.SetActive(true);
                gameUI.SetActive(false);
                break;
            case GameState.PlayState:
                playerInput.SwitchCurrentActionMap("Player");
                mainMenu.SetActive(false);
                gameUI.SetActive(true);
                break;
        }
    }

    private void OnGameStart()
    {
        if (gameState == GameState.MenuState)
        {
            SwitchStateTo(GameState.PlayState);

            ResetLevel();

            Cell initialCell = initialSpawn.Cell;
            if (!randomSpawn)
            {
                //Non-random Instantiate
                player = Instantiate(chompman, initialCell.Vector3CoordCompletion(chompman.transform.position.y), Quaternion.identity);
            }    
            else
            {
                //Random Instantiate
                player = RandomSpawnOnPellet();
                pathFindGrid.GetCellByCoords(Cell.GetCoordsFromVector3(player.transform.position), out initialCell);
            }

            player.transform.parent = GameObject.Find("Characters").transform;
            playerController = player.GetComponent<PlayerController>();
            playerController.currentCell = initialCell;

            for(int i = 0; i < ghostPrefabs.Length; i++)
            {
                GameObject ghost = Instantiate(ghostPrefabs[i], ghostSpawns[i].Cell.Vector3CoordCompletion(ghostPrefabs[i].transform.position.y), Quaternion.identity);
                ghost.transform.parent = GameObject.Find("Characters").transform;
                GhostController ghostScript = ghost.GetComponent<GhostController>();
                ghostScript.GhostSetup(ghostSpawns[i].Cell, ghostScatterPoints[i].Cell, ghostSpeed);
                if (i == 0)
                {
                    binky = ghost.GetComponent<BinkyController>();
                }
                else if (i == 1)
                {
                    InkyController inky = ghost.GetComponent<InkyController>();
                    inky.SetBinky(binky);
                }
            }

            // foreach (Cell cell in pathFindGrid.Cells)
            // {
            //     foreach (Cell.Direction dir in System.Enum.GetValues(typeof(Cell.Direction)))
            //     {
            //         if(cell.GetNeighbour(dir) != null)
            //         {
            //             Vector3 origin = new Vector3(cell.coordinates.x, 0.5f ,cell.coordinates.y); 
            //             Debug.DrawLine(origin,origin + Cell.GetVector3FromDirection(dir)*ChompmanGame.CELL_SIZE, Color.blue, 1.5f);
            //         } 
            //     }
            // }
        }
    }

    private void ResetLevel()
    {
        SetScore(0);
        pelletCount = 0;
        powerUpCount = 0;
        foreach(GameObject pellet in pellets)
        {
            pellet.SetActive(true);
            pelletCount++;
        }
        foreach(GameObject powerUp in powerUps)
        {
            powerUp.SetActive(true);
            powerUpCount++;
        }
    }

    private void CheckForWin()
    {
        if(powerUpCount == 0 && pelletCount == 0)
        {
            PlayerDied();
            Destroy(player);
            GameEnded(true);
        }
    }

    public void GameEnded(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("Your score is: " + currentScore + ". You won!");
        }
        else
        {
            Debug.Log("Your score is: " + currentScore + ". You lost!");
        }
        SwitchStateTo(GameState.MenuState);
        ResetLevel();
    }

    private GameObject RandomSpawnOnPellet() {
        int rand_index = UnityEngine.Random.Range(0, pellets.Count);
        Vector3 spawn_position = new Vector3(pellets[rand_index].transform.position.x, chompman.transform.position.y, pellets[rand_index].transform.position.z);
        return Instantiate(chompman, spawn_position, Quaternion.identity);
    }

    private void OnMove(InputValue movement_value)
    {
        Vector2 movement_vector = movement_value.Get<Vector2>();
        playerController.SetMovementVector(movement_vector);
    }

    //Лучше реализовать через Event и Listener
    public void ObjectTaken(string obj)
    {
        if (obj == "Pellet")
        {
            SetScore(currentScore + PELLET_VALUE);
            pelletCount--;
        }
        if (obj == "PowerUp")
        {
            SetScore(currentScore + POWERUP_VALUE);
            powerUpCount--;
        }     
        if (obj == "Ghost")
            SetScore(currentScore + GHOST_VALUE);
        CheckForWin();
    }

    private void SetScore(int score)
    {
        currentScore = score;
        if (scoreBoard != null)
        {
            string score_string = currentScore.ToString();
            while (score_string.Length < MAX_SCORE_SIZE)
                score_string = "0" + score_string;
            scoreBoard.text = "SCORE: " + score_string;
        }
    }

    //public event Action<float> onScoreChange = delegate{Debug.LogWarning("WARNING: Score change amount not set!");};
    
    public delegate void MovementCallback (ref Cell currentCell, Cell.Direction movDir);
    public event MovementCallback onPlayerMoved;
    public void PlayerMoved(Cell.Direction movDir)
    {
        if (onPlayerMoved != null)
        {
            onPlayerMoved(ref playerController.currentCell, movDir);
        }
    }
    public event Action onPlayerDied;
    public void PlayerDied()
    {
        if (onPlayerDied != null)
        {
            onPlayerDied();
        }
    }
    public event Action onPowerUp;
    public void PowerUpTaken()
    {
        if (onPowerUp != null)
        {
            onPowerUp();
        }
    }
}
