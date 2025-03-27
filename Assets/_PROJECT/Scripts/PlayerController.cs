using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;


    [SerializeField] Entity mainPlayerEntity;

    public Entity PlayerEntity => mainPlayerEntity;

    [SerializeField] float randomEncounterChance;
    [SerializeField] bool isInMenu { get; set; }


    [Header("Movement Data")]


    [SerializeField] Character character;

    public Character PlayerCharacter => character;


    Vector2 inputVector;



    private void Awake()
    {
        instance = this;
        inputVector = Vector2.zero;

        character = GetComponent<Character>();
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
        if (!isInMenu)
        {
            Debug.Log("Interact");
            var _facingDir = new Vector3(character.MainAnimator.GetFloat("moveX"), character.MainAnimator.GetFloat("moveY"));
            var _interactPosition = transform.position + _facingDir;

            // Debug.DrawLine(transform.position, _interactPosition, Color.red, 0.5f);

            var _collider = Physics2D.OverlapCircle(_interactPosition, 0.3f, GameLayers.Instance.InteractableLayer);
            if (_collider != null)
            {
                _collider.GetComponent<Interactable>()?.Interact(character.gridparentTransform);
            }
        }
    }

    public void HandleUpdate()
    {
        if (!character.isMoving)
        {
            //Removes Diagonal Movement
            if (inputVector.x != 0) inputVector.y = 0;


            if (inputVector != Vector2.zero)
            {
                StartCoroutine(character.Move(inputVector, character.gridparentTransform, OnMoveOver));
            }

            character.HandleUpdate();
        }
    }
    void OnMoveOver()
    {
        var _colliders = Physics2D.OverlapCircleAll(character.gridparentTransform.position, 0.2f, GameLayers.Instance.TriggerableLayer);

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

}
