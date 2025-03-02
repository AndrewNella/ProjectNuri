using UnityEngine;


public enum GameState { FreeRoam, Battle }
public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleController battleController;

    [SerializeField] Camera worldCamera;

    GameState state;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerController.OnEncounter += StartRandomizedAreaBattle;
        battleController.OnBattleOver += EndBattle;
    }

    public void StartSpesificMonsterBattle(Entity _incomingMonsterEntity)
    {
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleController.StartBattle(_incomingMonsterEntity);

    }

    void StartRandomizedAreaBattle()
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

