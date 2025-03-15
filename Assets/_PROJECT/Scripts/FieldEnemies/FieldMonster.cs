using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState 
{
    Wandering, 
    Attacking,
    Patroling
}
public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

public enum Property
{
    Meek,
    Aggressive,
    Patrol
}
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

    public Property property;

    [SerializeField] List<Transform> patrolPositionList;

    private int patrolIndex;
    private float movingPeroid;

    Vector2 randVector;
    Transform playerTransform;
    Transform targetTransform;
    float distanceWithPlayer;
    float distanceWithTargetPosition;

    Animator animator;
    GameObject player;

    MonsterState monsterState;
    Direction direction;

    private void Awake()
    {
        movingPeroid = baseWanderingPeriod;
        patrolIndex = 0;
        randVector = Vector2.zero;
        playerTransform = null;
        targetTransform = patrolPositionList[0];

        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        targetScanner = GetComponent<TargetScanner>();
        monsterState = MonsterState.Wandering;
    }

    private void Moving(Enum dir)
    {   
        switch (dir)
        {
            case Direction.Left:
                randVector = new Vector2(1, 0);
                break;
            case Direction.Right:
                randVector = new Vector2(-1, 0);
                break;
            case Direction.Up:
                randVector = new Vector2(0, 1);
                break;
            case Direction.Down:
                randVector = new Vector2(0, -1);
                break;
        }
        movingPeroid = baseWanderingPeriod + UnityEngine.Random.Range(0f, 0.2f * baseWanderingPeriod);
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
            if(property == Property.Aggressive)
            {
                monsterState = MonsterState.Attacking;
            }
        }
        if (playerTransform == null)
        {
            distanceWithPlayer = 100;
            monsterState = MonsterState.Wandering;
        }

        if (property == Property.Patrol)
        {
            monsterState = MonsterState.Patroling;
        }
        if (targetTransform != null)
        {
            distanceWithTargetPosition = (targetTransform.position - transform.position).sqrMagnitude;
        }

        if (distanceWithPlayer < 0.1)
        {
            if(fieldEnemyController.isBattle == false)
            {
                GameController.instance.StartSpesificMonsterBattle(fieldEntity);
                fieldEnemyController.isBattle = true;
            }
        }
        if(distanceWithTargetPosition < 0.1)
        {
            patrolIndex += 1;
            if(patrolIndex >= patrolPositionList.Count)
            {
                patrolIndex = 0;
            }
            targetTransform = patrolPositionList[patrolIndex];
        }

        if (movingPeroid <= 0)
        {
            switch(monsterState)
            {
                case MonsterState.Wandering:
                    Moving((Direction)(UnityEngine.Random.Range(0, Enum.GetNames(typeof(Direction)).Length)));
                    break;

                case MonsterState.Attacking:
                    Vector3 dir = playerTransform.position - transform.position;
                    dir = dir.normalized;
                    if ((Math.Abs(dir.x) >= Math.Abs(dir.y)) && dir.x >= 0)
                    {
                        Moving(Direction.Left);
                    }
                    if ((Math.Abs(dir.x) >= Math.Abs(dir.y)) && dir.x <= 0)
                    {
                        Moving(Direction.Right);
                    }
                    if ((Math.Abs(dir.x) < Math.Abs(dir.y)) && dir.y >= 0)
                    {
                        Moving(Direction.Up);
                    }
                    if ((Math.Abs(dir.x) < Math.Abs(dir.y)) && dir.y <= 0)
                    {
                        Moving(Direction.Down);
                    }
                    break;

                case MonsterState.Patroling:
                    Vector3 targetDir = targetTransform.position - transform.position;
                    targetDir = targetDir.normalized;
                    if ((Math.Abs(targetDir.x) >= Math.Abs(targetDir.y)) && targetDir.x >= 0)
                    {
                        Moving(Direction.Left);
                    }
                    if ((Math.Abs(targetDir.x) >= Math.Abs(targetDir.y)) && targetDir.x <= 0)
                    {
                        Moving(Direction.Right);
                    }
                    if ((Math.Abs(targetDir.x) < Math.Abs(targetDir.y)) && targetDir.y >= 0)
                    {
                        Moving(Direction.Up);
                    }
                    if ((Math.Abs(targetDir.x) < Math.Abs(targetDir.y)) && targetDir.y <= 0)
                    {
                        Moving(Direction.Down);
                    }
                    break;
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

