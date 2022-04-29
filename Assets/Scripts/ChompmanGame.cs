using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChompmanGame : MonoBehaviour
{
    [SerializeField]
    GameObject main_menu;

    [SerializeField]
    GameObject game_ui;

    [SerializeField]
    GameObject chompman;

    GameStates game_state;
    PlayerController player_controller;
    GameObject player;
    enum GameStates{
        MenuState,
        GameState
    }

    PlayerInput player_input;
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
            player = Instantiate(chompman, new Vector3(0, 0.5f, -8.0f), Quaternion.identity);
            player.transform.parent = GameObject.Find("Characters").transform;
            player_controller = player.GetComponent<PlayerController>();
            //Debug.Log("Spacebar!");
        }
    }

    void OnMove(InputValue movement_value)
    {
        Vector2 movement_vector = movement_value.Get<Vector2>();
        //Debug.Log(movement_vector);
        player_controller.SetMovementVector(movement_vector);
    }
}
