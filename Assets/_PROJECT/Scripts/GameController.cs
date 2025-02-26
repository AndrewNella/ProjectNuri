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
        battleController.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        Entity areaEnemy = FindFirstObjectByType<MapArea>().GetComponent<MapArea>().GetRandomAreaEnemy();

        battleController.StartBattle(areaEnemy);
    }

    void EndBattle(bool _isBattleWon)
    {
        state = GameState.FreeRoam;
        battleController.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
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

