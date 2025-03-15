using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

//The character Script MUST have access to an animator
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField] Animator animator;
    public bool isMoving;
    public float moveSpeed;
    [SerializeField] float movementMagnitudeLimit, solidObjectDetectionRadius;


    void Awake()
    {
        if (TryGetComponent<Animator>(out Animator _foundAnimator))
        {
            animator = _foundAnimator;

        }

    }

    public Animator MainAnimator => animator;
    public IEnumerator Move(Vector2 _moveVector, Action OnMoveOver = null)
    {

        animator?.SetFloat("moveX", _moveVector.x);
        animator?.SetFloat("moveY", _moveVector.y);

        Vector3 _targetPos = transform.position;
        _targetPos.x += _moveVector.x;
        _targetPos.y += _moveVector.y;

        if (!IsWalkable(_targetPos))
        {
            yield break;
        }

        isMoving = true;
        while ((_targetPos - transform.position).sqrMagnitude > movementMagnitudeLimit)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, moveSpeed * Time.deltaTime);

            yield return null;
        }
        transform.position = _targetPos;

        Vector3 _holdPosition = transform.position;
        _holdPosition.x = Mathf.Floor(transform.position.x) + 0.5f;
        transform.position = _holdPosition;

        Debug.Log(isMoving);
        isMoving = false;

        OnMoveOver?.Invoke();
    }
    public IEnumerator Move(Vector2 _moveVector, Transform _parentTransform, Action OnMoveOver = null)
    {

        animator?.SetFloat("moveX", Mathf.Clamp(_moveVector.x, -1, 1));
        animator?.SetFloat("moveY", Mathf.Clamp(_moveVector.y, -1, 1));

        Vector3 _targetPos = _parentTransform.position;
        _targetPos.x += _moveVector.x;
        _targetPos.y += _moveVector.y;

        if (!IsWalkable(_targetPos))
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

        Debug.Log(isMoving);

        isMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.SetBool("isMoving", isMoving);
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
