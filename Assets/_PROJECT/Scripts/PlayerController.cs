using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public event Action OnEncounter;

    [SerializeField] Entity mainPlayerEntity;

    [SerializeField] float randomEncounterChance;
    [SerializeField] bool isInMenu { get; set; }


    [Header("Movement Data")]

    [SerializeField] Character character;


    Vector2 inputVector;



    private void Awake()
    {
        instance = this;
        inputVector = Vector2.zero;

        character = GetComponent<Character>();

        MainInputActionController.instance.actionMap.PlayerControllerMap.Movement.performed += x => OnPlayerMoveInput(x.ReadValue<Vector2>());
        MainInputActionController.instance.actionMap.PlayerControllerMap.Movement.canceled += x => OnPlayerMoveInput(x.ReadValue<Vector2>());


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
        MainInputActionController.instance.OnInteractTrigger += OnInteract;
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
                StartCoroutine(character.Move(inputVector, character.gridparentTransform, CheckForEncounter));
            }

            character.HandleUpdate();
        }
    }


    public void CheckForEncounter()
    {

        if (Physics2D.OverlapCircle(character.gridparentTransform.position, 0.2f, GameLayers.Instance.DangerLayer) != null && UnityEngine.Random.Range(1, 101) < randomEncounterChance)
        {
            TriggerEncounter();
        }
    }

    public void TriggerEncounter()
    {
        character.MainAnimator.SetBool("isMoving", false);

        OnEncounter();
    }

}
