using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class MovementBase : MonoBehaviour
{
    public Transform gridparentTransform;
    Animator animator;
    public bool isMoving;

    public float moveSpeed;

    [SerializeField] float movementMagnitudeLimit, solidObjectDetectionRadius;

    public void InitializeMovement()
    {
        SetPositionAndSnapToTile(gridparentTransform.position);
    }
    public void InitializeMovement(Animator _incomingAnimator)
    {
        animator = _incomingAnimator;
        SetPositionAndSnapToTile(gridparentTransform.position);
    }




    public IEnumerator Move(Vector2 _moveVector, Transform _parentTransform, Action OnMoveOver = null)
    {
        if (GameController.instance.state == GameState.Pause || GameController.instance.state == GameState.Busy)
        {
            yield break;
        }
        animator?.SetFloat("moveX", Mathf.Clamp(_moveVector.x, -1, 1));
        animator?.SetFloat("moveY", Mathf.Clamp(_moveVector.y, -1, 1));

        Vector3 _targetPos = _parentTransform.position;
        _targetPos.x += _moveVector.x;
        _targetPos.y += _moveVector.y;

        if (!IsPathClear(_targetPos, _parentTransform))
        {
            yield break;
        }

        isMoving = true;
        while ((_targetPos - _parentTransform.position).sqrMagnitude > movementMagnitudeLimit)
        {
            _parentTransform.position = Vector3.MoveTowards(_parentTransform.position, _targetPos, moveSpeed * Time.deltaTime);

            yield return null;
        }
        _parentTransform.position = _targetPos;

        Vector3 _holdPosition = _parentTransform.position;
        _holdPosition.x = Mathf.Floor(_parentTransform.position.x) + 0.5f;
        _parentTransform.position = _holdPosition;

        // Debug.Log(isMoving);

        isMoving = false;

        OnMoveOver?.Invoke();
    }
    bool IsPathClear(Vector3 _targetVector, Transform _parentTransform)
    {
        var _difference = _targetVector - _parentTransform.position;
        var _direction = _difference.normalized;
        bool _BoxHit = Physics2D.BoxCast(_parentTransform.position + _direction, new Vector2(0.2f, 0.2f), 0f, _direction, _difference.magnitude - 1, GameLayers.Instance.SolidLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer);
        Debug.DrawLine(_parentTransform.position + _direction, _parentTransform.position + _direction * _difference.magnitude, Color.red, 4);
        if (_BoxHit)
        {
            // Debug.Log("Path is not clear");
            return false;
        }
        //The Path is clear
        return true;
    }

    public void SetPositionAndSnapToTile(Vector2 _pos)
    {
        _pos.x = Mathf.Floor(_pos.x) + 0.5f;
        _pos.y = Mathf.Floor(_pos.y) + 0.5f;
        gridparentTransform.position = _pos;
    }

    public void LookTowards(Vector3 _targetPosition)
    {
        float _xDiff = MathF.Floor(_targetPosition.x) - MathF.Floor(gridparentTransform.position.x);
        float _yDiff = MathF.Floor(_targetPosition.y) - MathF.Floor(gridparentTransform.position.y);

        if (_xDiff == 0 || _yDiff == 0)
        {
            animator?.SetFloat("moveX", Mathf.Clamp(_xDiff, -1, 1));
            animator?.SetFloat("moveY", Mathf.Clamp(_yDiff, -1, 1));
        }
        else
            Debug.LogError("Character Cannot Look Diagonally");
    }

    bool IsWalkable(Vector3 _targetPos)
    {

        if (Physics2D.OverlapCircle(_targetPos, solidObjectDetectionRadius, GameLayers.Instance.SolidLayer | GameLayers.Instance.InteractableLayer) != null)
        {
            return false;
        }
        return true;

    }
}
