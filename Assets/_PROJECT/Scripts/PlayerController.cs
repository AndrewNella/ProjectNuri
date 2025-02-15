using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerActionMap actionMap;
    public float moveSpeed;

    public bool isMoving;

    Vector2 movementDirection;

    private void Awake()
    {
        movementDirection = Vector2.zero;
        actionMap = new PlayerActionMap();
        actionMap.Enable();

        actionMap.PlayerControllerMap.Movement.performed += x => OnPlayerMoveInput(x.ReadValue<Vector2>());
        actionMap.PlayerControllerMap.Movement.canceled -= x => OnPlayerMoveInput(x.ReadValue<Vector2>());

    }

    private void OnPlayerMoveInput(Vector2 _incomingVector2)
    {
        movementDirection = _incomingVector2;
    }

    void Update()
    {
        if (!isMoving)
        {
            if (movementDirection != Vector2.zero)
            {
                Vector3 _targetPos = transform.position;
                _targetPos.x += _targetPos.x;
                _targetPos.y += _targetPos.y;
            }
        }
    }

    IEnumerator Move(Vector3 _targetPosition)
    {
        while ((_targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _targetPosition;
    }

}
