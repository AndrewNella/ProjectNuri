using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public enum MonsterState
{
    Idle,
    Wandering,
    Hunting,
    Patroling
}
public enum MonsterPersonality
{
    Aggressive,
    Passive
}
public enum MonsterMovementDirection
{
    Left,
    Right,
    Up,
    Down
}


public class FieldMonster : MonoBehaviour
{

    [SerializeField] FieldMonsterBase fieldbase;
    [SerializeField] BattleController battleController;

    TargetScanner targetScanner;

    [Header("Movement Data")]

    [SerializeField] Transform gridparentTransform;
    public bool isMoving;

    [SerializeField] float minimumDistanceToTriggerBattle;
    [SerializeField] float minimumDistanceToPatrolTarget;
    [SerializeField] float movementWaitTimer;

    [SerializeField] List<Transform> patrolPositionList;

    int patrolIndex;

    Vector2 randVector;
    Transform playerTransform;
    Transform patrolTargetTransform;
    float currentDistanceWithPlayer;
    float currentDistanceWithPatrolTarget;

    Animator animator;

    Coroutine currentMovementCoroutine;

    [Header("Monster Behaviour Settings")]
    //Enum References
    [SerializeField] MonsterState monsterState;
    [SerializeField] MonsterPersonality monsterPersonality;

    private void Awake()
    {
        fieldbase = GetComponent<FieldMonsterBase>();

        patrolIndex = 0;
        randVector = Vector2.zero;
        playerTransform = null;
        currentMovementCoroutine = null;

        //Check if the list exists, before we try to extract a value from it.
        if (patrolPositionList != null && patrolPositionList.Count < 0)
        {
            patrolTargetTransform = patrolPositionList[0];
        }

        animator = GetComponent<Animator>();
        targetScanner = GetComponent<TargetScanner>();
        monsterState = MonsterState.Wandering;
    }



    private void Defeated()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        //If the monster is stunned, then no movement actions will happen.
        if (!fieldbase.GetIsMonsterStunned())
        {
            ScanForPlayer();
            if (currentMovementCoroutine == null)
            {
                switch (monsterState)
                {
                    case MonsterState.Wandering:
                        currentMovementCoroutine = StartCoroutine(MoveInRandomDirection());
                        break;
                    case MonsterState.Hunting:
                        currentMovementCoroutine = StartCoroutine(MoveTowardsPlayer());
                        break;
                    case MonsterState.Patroling:
                        currentMovementCoroutine = StartCoroutine(MoveInPatrolRoute());
                        break;
                    default:
                        break;
                }
            }


            //If the player does exist.
            if (playerTransform != null)
            {
                currentDistanceWithPlayer = (playerTransform.position - transform.position).sqrMagnitude;
                if (monsterPersonality == MonsterPersonality.Aggressive && monsterState != MonsterState.Hunting)
                {
                    monsterState = MonsterState.Hunting;
                }
            }

            //If the Player does not Exist.
            if (playerTransform == null)
            {
                currentDistanceWithPlayer = 100;
                monsterState = MonsterState.Wandering;
            }

            if (monsterState == MonsterState.Patroling)
            {
                UpdatePatrol();
            }
            fieldbase.Character.HandleUpdate();

            animator.SetBool("isMoving", fieldbase.Character.isMoving);
        }
    }

    void UpdatePatrol()
    {
        if (currentDistanceWithPatrolTarget < minimumDistanceToPatrolTarget)
        {
            patrolIndex += 1;
            if (patrolIndex >= patrolPositionList.Count)
            {
                patrolIndex = 0;
            }
            patrolTargetTransform = patrolPositionList[patrolIndex];
        }
    }
    void ScanForPlayer()
    {
        playerTransform = targetScanner.nearestTarget;

    }

    void CheckIfPlayerIsWithinRangeForBattle()
    {
        if (currentDistanceWithPlayer < minimumDistanceToTriggerBattle && !fieldbase.GetIsBattleDisabled())
        {
            if (GameController.instance.state != GameState.Battle)
            {
                fieldbase.TriggerAttackFromThisEntity();
                return;
            }
        }
    }

    IEnumerator MoveInRandomDirection()
    {
        MoveInSpesificDirection((MonsterMovementDirection)(UnityEngine.Random.Range(0, Enum.GetNames(typeof(MonsterMovementDirection)).Length)));
        yield return new WaitForSeconds(movementWaitTimer);
        currentMovementCoroutine = null;

    }
    IEnumerator MoveInPatrolRoute()
    {
        Vector3 targetDir = patrolTargetTransform.position - transform.position;
        targetDir = targetDir.normalized;
        if ((Math.Abs(targetDir.x) >= Math.Abs(targetDir.y)) && targetDir.x >= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Left);
        }
        if ((Math.Abs(targetDir.x) >= Math.Abs(targetDir.y)) && targetDir.x <= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Right);
        }
        if ((Math.Abs(targetDir.x) < Math.Abs(targetDir.y)) && targetDir.y >= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Up);
        }
        if ((Math.Abs(targetDir.x) < Math.Abs(targetDir.y)) && targetDir.y <= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Down);
        }

        yield return new WaitForSeconds(movementWaitTimer);
        currentMovementCoroutine = null;



    }
    void MoveInSpesificDirection(Enum _dir)
    {
        switch (_dir)
        {
            case MonsterMovementDirection.Left:
                randVector = new Vector2(1, 0);
                break;
            case MonsterMovementDirection.Right:
                randVector = new Vector2(-1, 0);
                break;
            case MonsterMovementDirection.Up:
                randVector = new Vector2(0, 1);
                break;
            case MonsterMovementDirection.Down:
                randVector = new Vector2(0, -1);
                break;
        }

        //If the monster is aggressive, it will battle the player.
        if (monsterPersonality == MonsterPersonality.Aggressive)
        {
            StartCoroutine(fieldbase.Character.Move(randVector, gridparentTransform, CheckIfPlayerIsWithinRangeForBattle));
        }
        else
        {
            StartCoroutine(fieldbase.Character.Move(randVector, gridparentTransform));
        }


    }
    IEnumerator MoveTowardsPlayer()
    {
        Vector3 dir = playerTransform.position - transform.position;
        dir = dir.normalized;

        if ((Math.Abs(dir.x) >= Math.Abs(dir.y)) && dir.x >= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Left);
        }
        if ((Math.Abs(dir.x) >= Math.Abs(dir.y)) && dir.x <= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Right);
        }
        if ((Math.Abs(dir.x) < Math.Abs(dir.y)) && dir.y >= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Up);
        }
        if ((Math.Abs(dir.x) < Math.Abs(dir.y)) && dir.y <= 0)
        {
            MoveInSpesificDirection(MonsterMovementDirection.Down);
        }
        yield return new WaitForSeconds(movementWaitTimer);
        currentMovementCoroutine = null;
    }
}