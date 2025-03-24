using UnityEngine;


public enum GameState { FreeRoam, Battle, Dialogue, CutScene }
public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleController battleController;

    OverworldUI overWorldUISystem;
    [SerializeField] GameObject overWorldUIParent;


    [SerializeField] Camera worldCamera;

    public GameState state;

    public FieldMonsterBase currentFieldMonsterBase { get; private set; }

    private void Awake()
    {
        instance = this;
        ConditionDataBase.Init();
        overWorldUISystem = overWorldUIParent.GetComponent<OverworldUI>();
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

    public void EnableOrDisableOverworldHUD(bool _incomingBool)
    {
        overWorldUIParent.SetActive(_incomingBool);
    }
    public void StartCutsceneState()
    {
        state = GameState.CutScene;
    }

    public void StartSpesificMonsterBattle(Entity _incomingMonsterEntity)
    {
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        _incomingMonsterEntity.Init();
        battleController.StartBattle(_incomingMonsterEntity);

    }

    public void StartSpesificMonsterBattle(Entity _incomingMonsterEntity, FieldMonsterBase _incomingFieldMonsterBase)
    {
        currentFieldMonsterBase = _incomingFieldMonsterBase;

        EnableOrDisableOverworldHUD(false);
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        _incomingMonsterEntity.Init();
        battleController.StartBattle(_incomingMonsterEntity);

    }

    void StartRandomizedAreaBattle()
    {
        EnableOrDisableOverworldHUD(false);
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        Entity areaEnemy = FindFirstObjectByType<MapArea>().GetComponent<MapArea>().GetRandomAreaEnemy();

        battleController.StartBattle(areaEnemy);
    }



    void EndBattle(bool _isBattleWon, bool _isEscape)
    {
        if (currentFieldMonsterBase != null && _isBattleWon)
        {
            if (_isEscape) currentFieldMonsterBase.EscapeStun();
            else currentFieldMonsterBase.OnDefeated();

        }
        overWorldUISystem.UpdateHUDPlayerStats();
        EnableOrDisableOverworldHUD(true);
        state = GameState.FreeRoam;
        battleController.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        if (currentFieldMonsterBase != null) currentFieldMonsterBase = null;
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

