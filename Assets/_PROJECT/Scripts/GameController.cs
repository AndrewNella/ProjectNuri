using UnityEngine;


public enum GameState { FreeRoam, Battle }
public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleController battleController;

    [SerializeField] Camera worldCamera;

    GameState state;
    void Start()
    {
        playerController.OnEncounter += StartBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
    }
    void Update()
    {
        switch (state)
        {
            case GameState.Battle:
                battleController.HandleUpdate();
                break;
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            default:
                break;
        }
    }
}

