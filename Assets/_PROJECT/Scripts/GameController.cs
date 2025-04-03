using UnityEngine;
using Cinemachine;


public enum GameState { FreeRoam, Battle, Dialogue, CutScene, Pause, Busy }
public class GameController : MonoBehaviour
{

    public bool isDataLoaded { get; private set; } = false;
    [Header("Camera Data")]
    [SerializeField] CinemachineVirtualCamera overworldPlayerCamera, battleCamera;
    public static GameController instance;

    [Header("System Controller Data")]
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleController battleController;

    public FieldMonsterBase currentFieldMonsterBase { get; private set; }

    [Header("UI Data")]
    [SerializeField] GameObject overWorldUIParent;

    [SerializeField] PauseMenu pauseMenu;
    OverworldUI overWorldUISystem;

    public OverworldUI OverWorldHUD => overWorldUISystem;

    [Header("Area Map Data")]

    [SerializeField] LevelDetails currentMainLevelInfo;
    [SerializeField] MapArea mapArea, defaultMapArea;

    public LevelDetails CurrentMainLevelInfo => currentMainLevelInfo;

    public GameState state;
    GameState stateBeforePause;


    private void Awake()
    {
        instance = this;

        EntityDataBase.Init();
        AttackDataBase.Init();
        ConditionDataBase.Init();

        overWorldUISystem = overWorldUIParent.GetComponent<OverworldUI>();
    }
    void Start()
    {
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
        MainInputActionController.instance.OnPauseTrigger += TriggerGamePause;

    }


    private void OnDisable()
    {
        MainInputActionController.instance.OnPauseTrigger -= TriggerGamePause;
    }

    public void TriggerGamePause()
    {
        switch (state)
        {
            case GameState.Battle:
                battleController.ReturnToMainBattleMenu();
                break;

            case GameState.Dialogue:
                DialogueManager.Instance.UpdateDialogue();
                break;

            case GameState.FreeRoam:

                PauseGame(true);
                pauseMenu.TogglePauseMenu(true);
                break;

            case GameState.Pause:
                if (pauseMenu.isPauseMenuActive)
                {
                    PauseGame(false);
                    pauseMenu.TogglePauseMenu(false);
                }
                break;

            default:
                break;
        }
    }

    public void SetIsDataLoaded()
    {
        isDataLoaded = true;
    }
    public void SetCurrentMapAreaToDefault()
    {
        if (defaultMapArea == null)
        {
            defaultMapArea = gameObject.GetComponent<MapArea>();
        }

        mapArea = defaultMapArea;
    }
    public void UpdateCurrentMapArea(MapArea _newMapArea)
    {
        mapArea = _newMapArea;
    }

    public void GameisBusy(bool _isBusy)
    {
        if (_isBusy)
        {
            stateBeforePause = state;
            state = GameState.Busy;
        }
        else
        {
            state = stateBeforePause;
        }
    }
    public void PauseGame(bool _isPaused)
    {
        if (_isPaused)
        {
            stateBeforePause = state;
            state = GameState.Pause;
        }
        else
        {
            state = stateBeforePause;
        }
    }
    public void EnableOrDisableOverworldHUD(bool _incomingBool)
    {
        overWorldUIParent.SetActive(_incomingBool);
    }
    public void StartCutsceneState()
    {
        state = GameState.CutScene;
    }

    #region Save and Load Data Functions
    public void SaveGame()
    {
        TriggerGamePause();
        SavingSystem.i.Save("saveFile1");
    }
    public void LoadGame()
    {

        TriggerGamePause();
        SavingSystem.i.Load("saveFile1");
    }
    public void LoadGame(string _fileName)
    {
        SavingSystem.i.Load($"{_fileName}");

        overWorldUISystem.UpdateHUDPlayerStats();
    }


    #endregion

    #region Battle Start and End Functions
    public void StartSpesificMonsterBattle(Entity _incomingMonsterEntity)
    {
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        battleCamera.Priority = 6;
        _incomingMonsterEntity.Init();
        battleController.StartBattle(_incomingMonsterEntity);

    }

    public void StartSpesificMonsterBattle(Entity _incomingMonsterEntity, FieldMonsterBase _incomingFieldMonsterBase)
    {
        currentFieldMonsterBase = _incomingFieldMonsterBase;

        EnableOrDisableOverworldHUD(false);
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        battleCamera.Priority = 6;
        _incomingMonsterEntity.Init();
        battleController.StartBattle(_incomingMonsterEntity);

    }

    public void StartRandomizedAreaBattle()
    {
        EnableOrDisableOverworldHUD(false);
        state = GameState.Battle;
        battleController.gameObject.SetActive(true);
        battleCamera.Priority = 6;
        Entity areaEnemy = mapArea.GetRandomAreaEnemy();

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
        battleCamera.Priority = 1;

        if (currentFieldMonsterBase != null) currentFieldMonsterBase = null;
    }
    #endregion
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

