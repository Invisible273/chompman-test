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
    CellView initialSpawn;

    [SerializeField]
    bool randomSpawn = false;
    [SerializeField]
    GameObject grid;
    
    [HideInInspector]
    public CellSet pathFindGrid;
    public static ChompmanGame instance = null;

    GameStates gameState;
    enum GameStates{
        MenuState,
        GameState
    }

    PlayerController playerController;
    GameObject player;
    PlayerInput playerInput;
    int currentScore = 0;
    TextMeshProUGUI scoreBoard = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SwitchStateTo(GameStates.MenuState);
    }

    void SwitchStateTo(GameStates state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameStates.MenuState:
                playerInput.SwitchCurrentActionMap("Menu");
                mainMenu.SetActive(true);
                gameUI.SetActive(false);
                break;
            case GameStates.GameState:
                playerInput.SwitchCurrentActionMap("Player");
                mainMenu.SetActive(false);
                gameUI.SetActive(true);
                break;
        }
    }

    //Player Input
    void OnGameStart()
    {
        if (gameState == GameStates.MenuState)
        {
            SwitchStateTo(GameStates.GameState);

            pathFindGrid = grid.GetComponent<GridSetter>().tiles;
            Cell initialCell = initialSpawn.Cell;
            if (!randomSpawn)
            {
                //Non-random Instantiate
                player = Instantiate(chompman, new Vector3(initialCell.coordinates.x,chompman.transform.position.y,initialCell.coordinates.y), Quaternion.identity);
            }    
            else
            {
                //Random Instantiate
                player = RandomSpawnOnPellet();
                Vector2 currentPlayerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
                pathFindGrid.GetCellByCoords(currentPlayerPosition, out initialCell);
            }

            player.transform.parent = GameObject.Find("Characters").transform;
            playerController = player.GetComponent<PlayerController>();
            playerController.currentCell = initialCell;

            scoreBoard = gameUI.GetComponentInChildren<TextMeshProUGUI>();

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

    GameObject RandomSpawnOnPellet() {
        Transform[] pellet_coords = pelletContainer.GetComponentsInChildren<Transform>();
        int rand_index = UnityEngine.Random.Range(0,pellet_coords.Length);
        Vector3 spawn_position = new Vector3(pellet_coords[rand_index].position.x, chompman.transform.position.y, pellet_coords[rand_index].position.z);
        return Instantiate(chompman, spawn_position, Quaternion.identity);
    }

    void OnMove(InputValue movement_value)
    {
        Vector2 movement_vector = movement_value.Get<Vector2>();
        playerController.SetMovementVector(movement_vector);
    }

    //Лучше реализовать через Event и Listener
    public void ObjectTaken(string obj)
    {
        if (obj == "Pellet")
            SetScore(currentScore + PELLET_VALUE);
        if (obj == "PowerUp")
            SetScore(currentScore + POWERUP_VALUE);
    }

    void SetScore(int score)
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
}
