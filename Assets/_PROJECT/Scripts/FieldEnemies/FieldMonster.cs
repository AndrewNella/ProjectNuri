using System;
using System.Collections;
using UnityEngine;

public class FieldMonster : MonoBehaviour
{

    [SerializeField] LayerMask solidObjectLayer, playerLayer;
    [SerializeField] float solidObjectDetectionRadius;

    [SerializeField] Entity fieldEntity;

    [SerializeField] BattleController battleController;
    [SerializeField] FieldEnemyController fieldEnemyController;

    TargetScanner targetScanner;

    [Header("Movement Data")]

    [SerializeField] Transform gridparentTransform;
    public float baseWanderingPeriod;
    public float moveSpeed;
    [SerializeField] float movementMagnitudeLimit;

    public bool isBattlePhase;
    public bool isMoving;
    public bool isAggressive;

    private float movingPeroid;

    Vector2 randVector;
    Transform playerTransform;
    float distanceWithPlayer;

    Animator animator;
    GameObject player;

    private void Awake()
    {
        movingPeroid = baseWanderingPeriod;
        randVector = Vector2.zero;
        playerTransform = null;

        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        targetScanner = GetComponent<TargetScanner>();
    }

    private void Moving(int randomValue)
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

    private void Defeated()
    {
        gameObject.SetActive(false);
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

                if (IsWanderable(_targetPos) && fieldEnemyController.isBattle == false)
                {
                    StartCoroutine(Move(_targetPos));
                }
                else
                    movingPeroid = 0f; // Restart Wandering State if cannot move.
            }
        }
        movingPeroid -= Time.deltaTime;

        playerTransform = targetScanner.nearestTarget;
        if (playerTransform != null)
        {
            distanceWithPlayer = (playerTransform.position - transform.position).sqrMagnitude;
        }
        else
        {
            distanceWithPlayer = 100;
        }
        if (distanceWithPlayer < 0.1)
        {
            if(fieldEnemyController.isBattle == false)
            {
                GameController.instance.StartSpesificMonsterBattle(fieldEntity);
                fieldEnemyController.isBattle = true;
            }
        }

        if (movingPeroid <= 0)
        {
            if(isAggressive == false)
            {
                Moving(UnityEngine.Random.Range(0, 4));
                movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
            }
            else
            {
                if (playerTransform != null)
                {

                    Vector3 dir = playerTransform.position - transform.position;
                    dir = dir.normalized;
                    Debug.Log(dir);
                    if ((Math.Abs(dir.x) >= Math.Abs(dir.y)) && dir.x >= 0)
                    {
                        Moving(0);
                        movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
                    }
                    if ((Math.Abs(dir.x) >= Math.Abs(dir.y)) && dir.x <= 0)
                    {
                        Moving(1);
                        movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
                    }
                    if ((Math.Abs(dir.x) < Math.Abs(dir.y)) && dir.y >= 0)
                    {
                        Moving(2);
                        movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
                    }
                    if ((Math.Abs(dir.x) < Math.Abs(dir.y)) && dir.y <= 0)
                    {
                        Moving(3);
                        movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
                    }
                }
                else
                {
                    Moving(UnityEngine.Random.Range(0, 4));
                    movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
                }
            }
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
        return true;
    }
} 

