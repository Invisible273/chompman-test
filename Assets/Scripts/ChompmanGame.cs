using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChompmanGame : MonoBehaviour
{
    private const int MAX_SCORE_SIZE = 6;
    private const int PELLET_VALUE = 100;
    [SerializeField]
    GameObject main_menu;

    [SerializeField]
    GameObject game_ui;

    [SerializeField]
    GameObject chompman;

    //[HideInInspector]
    public static ChompmanGame instance = null;

    GameStates game_state;
    enum GameStates{
        MenuState,
        GameState
    }

    PlayerController player_controller;
    GameObject player;
    PlayerInput player_input;
    int current_score = 0;
    TextMeshProUGUI score_board = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        player_input = GetComponent<PlayerInput>();
        SwitchStateTo(GameStates.MenuState);
    }

    void SwitchStateTo(GameStates state)
    {
        game_state = state;
        switch (game_state)
        {
            case GameStates.MenuState:
                player_input.SwitchCurrentActionMap("Menu");
                main_menu.SetActive(true);
                game_ui.SetActive(false);
                break;
            case GameStates.GameState:
                player_input.SwitchCurrentActionMap("Player");
                main_menu.SetActive(false);
                game_ui.SetActive(true);
                break;
        }
    }

    //Player Input
    void OnGameStart()
    {
        if (game_state == GameStates.MenuState)
        {
            SwitchStateTo(GameStates.GameState);

            player = Instantiate(chompman, chompman.transform.position, Quaternion.identity);
            player.transform.parent = GameObject.Find("Characters").transform;
            player_controller = player.GetComponent<PlayerController>();

            score_board = game_ui.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void OnMove(InputValue movement_value)
    {
        Vector2 movement_vector = movement_value.Get<Vector2>();
        player_controller.SetMovementVector(movement_vector);
    }

    //Лучше реализовать через Event и Listener
    public void ObjectTaken(string obj)
    {
        if (obj == "Pellet")
            SetScore(current_score + PELLET_VALUE);
    }

    void SetScore(int score)
    {
        current_score = score;
        if (score_board != null)
        {
            string score_string = current_score.ToString();
            while (score_string.Length < MAX_SCORE_SIZE)
                score_string = "0" + score_string;
            score_board.text = "SCORE: " + score_string;
        }
    }

    //public event Action<float> onScoreChange = delegate{Debug.LogWarning("WARNING: Score change amount not set!");};
}
