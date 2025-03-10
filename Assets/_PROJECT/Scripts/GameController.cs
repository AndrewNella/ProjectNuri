using UnityEngine;


public enum GameState { FreeRoam, Battle, Dialogue }
public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleController battleController;

    [SerializeField] Camera worldCamera;

    public GameState state;

    private void Awake()
    {
        instance = this;
        ConditionDataBase.Init();
    }
    void Start()
    {
        playerController.OnEncounter += StartRandomizedAreaBattle;
        battleController.OnBattleOver += EndBattle;

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            state = GameState.Dialogue;
        };
        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (state == GameState.Dialogue)
            {
                state = GameState.FreeRoam;
            }
        };
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
        if (playerController.GetIsInMenu() == false && state != GameState.FreeRoam)
        {
            playerController.SetIsInMenu(true);
        }

        if (state == GameState.FreeRoam && playerController.GetIsInMenu() == true)
        {
            playerController.SetIsInMenu(false);

        }

        switch (state)
        {
            case GameState.Battle:
                battleController.HandleUpdate();
                break;
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            case GameState.Dialogue:
                DialogueManager.Instance.HandleUpdate();
                break;
            default:
                break;
        }
    }
}

