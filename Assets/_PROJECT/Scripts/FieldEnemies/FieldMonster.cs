using System;
using System.Collections;
using UnityEngine;

public class FieldMonster : MonoBehaviour
{
    public enum MonsterState { IDLE, WANDER, RECOGNIZE };

    MonsterBase _base;

    [SerializeField] LayerMask solidObjectLayer, playerLayer;
    [SerializeField] float solidObjectDetectionRadius;

    [Header("Movement Data")]

    [SerializeField] Transform gridparentTransform;
    public float baseWanderingPeriod;
    public float moveSpeed;
    [SerializeField] float movementMagnitudeLimit;
    public bool isMoving;

    private float wanderingPeriod;

    Vector2 randVector;

    Animator animator;

    private void Awake()
    {
        wanderingPeriod = baseWanderingPeriod;
        randVector = Vector2.zero;

        animator = GetComponent<Animator>();
    }

    private void Wandering(int randomValue)
    {   
        switch (randomValue)
        {
            case 0:
                randVector = new Vector2(1, 0);
                break;
            case 1:
                randVector = new Vector2(-1, 0);
                break;
            case 2:
                randVector = new Vector2(0, 1);
                break;
            case 3:
                randVector = new Vector2(0, -1);
                break;
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            if (randVector != Vector2.zero)
            {

                Vector3 _targetPos = gridparentTransform.position;
                _targetPos.x += randVector.x;
                _targetPos.y += randVector.y;

                if (IsWanderable(_targetPos))
                {
                    StartCoroutine(Move(_targetPos));
                }
                else
                    wanderingPeriod = 0f; // Restart Wandering State if cannot move.
            }
        }
        wanderingPeriod -= Time.deltaTime;

        if(wanderingPeriod <= 0)
        {
            Wandering(UnityEngine.Random.Range(0, 4));
            wanderingPeriod = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
        }

        animator.SetBool("isMoving", isMoving);
    }
    IEnumerator Move(Vector3 _targetPosition)
    {
        if (randVector.x != 0) //temporal code for left-right sprite.
            animator.SetFloat("moveX", randVector.x);

        animator.SetFloat("moveY", randVector.y);
        isMoving = true;
        while ((_targetPosition - gridparentTransform.position).sqrMagnitude > movementMagnitudeLimit)
        {
            gridparentTransform.position = Vector3.MoveTowards(gridparentTransform.position, _targetPosition, moveSpeed * Time.deltaTime);
            randVector = Vector2.zero; // Stop after 1 tile movement

            yield return null;
        }
        gridparentTransform.position = _targetPosition;
        isMoving = false;
    }
    bool IsWanderable(Vector3 _targetPos)
    {

        if (Physics2D.OverlapCircle(_targetPos, solidObjectDetectionRadius, solidObjectLayer) != null)
        {
            return false;
        }
        if (Physics2D.OverlapCircle(_targetPos, solidObjectDetectionRadius, playerLayer) != null)
        {
            return false;
        }
        return true;
    }
} 

