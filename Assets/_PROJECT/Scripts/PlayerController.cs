using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public LayerMask solidObjectLayer, dangerLayer, interactableLayer;
    [SerializeField] float solidObjectDetectionRadius;

    public event Action OnEncounter;

    [SerializeField] Entity mainPlayerEntity;

    [SerializeField] float randomEncounterChance;
    [SerializeField] bool isInMenu { get; set; }


    [Header("Movement Data")]

    [SerializeField] Transform gridparentTransform;
    public float moveSpeed;
    [SerializeField] float movementMagnitudeLimit;
    public bool isMoving;

    Vector2 inputVector;

    Animator animator;


    private void Awake()
    {
        instance = this;
        inputVector = Vector2.zero;


        animator = GetComponent<Animator>();

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
            var _facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            var _interactPosition = transform.position + _facingDir;

            // Debug.DrawLine(transform.position, _interactPosition, Color.red, 0.5f);

            var _collider = Physics2D.OverlapCircle(_interactPosition, 0.3f, interactableLayer);
            if (_collider != null)
            {
                _collider.GetComponent<Interactable>()?.Interact();
            }
        }
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            //Removes Diagonal Movement
            if (inputVector.x != 0) inputVector.y = 0;


            if (inputVector != Vector2.zero)
            {
                animator.SetFloat("moveX", inputVector.x);
                animator.SetFloat("moveY", inputVector.y);

                Vector3 _targetPos = gridparentTransform.position;
                _targetPos.x += inputVector.x;
                _targetPos.y += inputVector.y;

                if (IsWalkable(_targetPos))
                {

                    StartCoroutine(Move(_targetPos));
                }
            }


        }
        animator.SetBool("isMoving", isMoving);

    }

    IEnumerator Move(Vector3 _targetPosition)
    {
        isMoving = true;
        while ((_targetPosition - gridparentTransform.position).sqrMagnitude > movementMagnitudeLimit)
        {
            gridparentTransform.position = Vector3.MoveTowards(gridparentTransform.position, _targetPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }
        gridparentTransform.position = _targetPosition;

        Vector3 _holdPosition = gridparentTransform.position;
        _holdPosition.x = Mathf.Floor(gridparentTransform.position.x) + 0.5f;
        gridparentTransform.position = _holdPosition;

        Debug.Log(isMoving);
        isMoving = false;

        CheckForEncounter();
    }

    public void CheckForEncounter()
    {

        if (Physics2D.OverlapCircle(gridparentTransform.position, 0.2f, dangerLayer) != null && UnityEngine.Random.Range(1, 101) < randomEncounterChance)
        {
            TriggerEncounter();
        }
    }

    public void TriggerEncounter()
    {
        animator.SetBool("isMoving", false);

        OnEncounter();
    }

    bool IsWalkable(Vector3 _targetPos)
    {

        if (Physics2D.OverlapCircle(_targetPos, solidObjectDetectionRadius, solidObjectLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;

    }

}
