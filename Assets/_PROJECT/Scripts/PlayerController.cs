using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    PlayerActionMap actionMap;

    [SerializeField] LayerMask solidObjectLayer, dangerLayer;
    [SerializeField] float solidObjectDetectionRadius;


    public event Action OnEncounter;

    [SerializeField] Entity mainPlayerEntity;

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
        actionMap = new PlayerActionMap();
        actionMap.Enable();

        animator = GetComponent<Animator>();

        actionMap.PlayerControllerMap.Movement.performed += x => OnPlayerMoveInput(x.ReadValue<Vector2>());
        actionMap.PlayerControllerMap.Movement.canceled += x => OnPlayerMoveInput(x.ReadValue<Vector2>());

        actionMap.PlayerControllerMap.Pause.performed += OnPauseInput;
        actionMap.PlayerControllerMap.Pause.canceled -= OnPauseInput;

        mainPlayerEntity.Init();

    }

    void OnPauseInput(InputAction.CallbackContext _incomingInput)
    {

    }
    public Entity GetPlayerEntity()
    {
        return mainPlayerEntity;
    }

    private void OnPlayerMoveInput(Vector2 _incomingVector2)
    {

        inputVector = _incomingVector2;
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
        if (Physics2D.OverlapCircle(gridparentTransform.position, 0.2f, dangerLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);

                OnEncounter();
            }
        }
    }

    bool IsWalkable(Vector3 _targetPos)
    {

        if (Physics2D.OverlapCircle(_targetPos, solidObjectDetectionRadius, solidObjectLayer) != null)
        {
            return false;
        }
        return true;

    }

}
