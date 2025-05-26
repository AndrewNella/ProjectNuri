using System;
using System.Collections;
using System.ComponentModel;
using Kisei.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ISavable
{

    public bool enablePlayerInputs = true;
    [SerializeField] Entity mainPlayerEntity;
    public Entity PlayerEntity => mainPlayerEntity;



    [SerializeField] float randomEncounterChance;
    [SerializeField] bool isInMenu { get; set; }


    [Header("Movement Data")]

    [SerializeField] GameObject headPlayerControllerParent;
    Character character;
    Vector2 inputVector;


    private void Awake()
    {
        inputVector = Vector2.zero;

        character = PlayerInstanceHUB.Instance.PlayerCharacter;
    }
    private void Start()
    {
        MainInputActionController.instance.actionMap.PlayerControllerMap.Movement.performed += x => OnPlayerMoveInput(x.ReadValue<Vector2>());
        MainInputActionController.instance.actionMap.PlayerControllerMap.Movement.canceled += x => OnPlayerMoveInput(x.ReadValue<Vector2>());

        MainInputActionController.instance.OnInteractTrigger += OnInteract;


        mainPlayerEntity.Init();
    }
    public void SetIsInMenu(bool _incomingBool)
    {
        isInMenu = _incomingBool;
    }

    public bool GetIsInMenu()
    {
        return isInMenu;
    }
    void OnEnable()
    {
    }

    void OnDisable()
    {
        MainInputActionController.instance.OnInteractTrigger -= OnInteract;
    }

    public Entity GetPlayerEntity()
    {
        return mainPlayerEntity;
    }

    private void OnPlayerMoveInput(Vector2 _incomingVector2)
    {
        inputVector = _incomingVector2;
    }

    void OnInteract()
    {
        if (!enablePlayerInputs) return;

        if (!isInMenu)
        {
            Debug.Log("Interact");
            var _facingDir = new Vector3(character.MainAnimator.GetFloat("moveX"), character.MainAnimator.GetFloat("moveY"));
            var _interactPosition = transform.position + _facingDir;

            // Debug.DrawLine(transform.position, _interactPosition, Color.red, 0.5f);

            var _collider = Physics2D.OverlapCircle(_interactPosition, 0.3f, GameLayers.Instance.InteractableLayer);
            if (_collider != null)
            {
                _collider.GetComponent<Interactable>()?.Interact(character.movementControl.gridparentTransform);
            }
        }
    }

    public void HandleUpdate()
    {
        if (enablePlayerInputs)
        {
            if (!character.movementControl.isMoving)
            {
                //Removes Diagonal Movement
                if (inputVector.x != 0) inputVector.y = 0;


                if (inputVector != Vector2.zero)
                {
                    StartCoroutine(character.movementControl.Move(inputVector, character.movementControl.gridparentTransform, OnMoveOver));
                }

                character.HandleUpdate();
            }
        }
    }
    void OnMoveOver()
    {
        var _colliders = Physics2D.OverlapCircleAll(character.movementControl.gridparentTransform.position, 0.2f, GameLayers.Instance.TriggerableLayer);
        foreach (var _collider in _colliders)
        {
            var _trigger = _collider.GetComponent<IPlayerTriggerable>();
            if (_trigger != null)
            {
                _trigger.OnPlayerTrigger(this);
                break;
            }
        }
    }

    public void StopPlayerAnimator()
    {

        character.MainAnimator.SetBool("isMoving", false);
    }

    public object CaptureState()
    {
        var _saveData = new PlayerSaveData()
        {
            savedPlayerPosition = new float[] { character.movementControl.gridparentTransform.position.x, character.movementControl.gridparentTransform.position.y },
            savedPlayerEntity = mainPlayerEntity.GetSaveData()

        };

        return _saveData;
    }

    public void RestoreState(object state)
    {
        var _saveData = (PlayerSaveData)state;

        // Restore Position
        var _position = _saveData.savedPlayerPosition;

        character.movementControl.gridparentTransform.position = new Vector3(_position[0], _position[1]);

        //Restore Player Data
        mainPlayerEntity = new Entity(_saveData.savedPlayerEntity);

        //Extra Functionality

        if (GameController.instance.OverWorldHUD != null)
        {
            GameController.instance.OverWorldHUD.UpdateHUDPlayerStats();
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float[] savedPlayerPosition;

    public EntitySaveData savedPlayerEntity;
}
