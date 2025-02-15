using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerActionMap actionMap;
    public float moveSpeed;

    public bool isMoving;

    Vector2 inputVector;

    private void Awake()
    {
        inputVector = Vector2.zero;
        actionMap = new PlayerActionMap();
        actionMap.Enable();

        actionMap.PlayerControllerMap.Movement.performed += x => OnPlayerMoveInput(x.ReadValue<Vector2>());
        actionMap.PlayerControllerMap.Movement.canceled += x => OnPlayerMoveInput(x.ReadValue<Vector2>());

    }

    private void OnPlayerMoveInput(Vector2 _incomingVector2)
    {
        Debug.Log("Is Moving");
        inputVector = _incomingVector2;
    }

    void Update()
    {
        if (!isMoving)
        {
            //Removes Diagonal Movement
            if (inputVector.x != 0) inputVector.y = 0;


            if (inputVector != Vector2.zero)
            {
                Vector3 _targetPos = transform.position;
                _targetPos.x += inputVector.x;
                _targetPos.y += inputVector.y;

                StartCoroutine(Move(_targetPos));
            }
        }
    }

    IEnumerator Move(Vector3 _targetPosition)
    {
        isMoving = true;
        while ((_targetPosition - transform.position).sqrMagnitude > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _targetPosition;
        Debug.Log(isMoving);
        isMoving = false;
    }

}
