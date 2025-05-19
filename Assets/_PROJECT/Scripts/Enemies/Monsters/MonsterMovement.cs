using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// System that controls the movement of monsters
/// </summary>
public enum MonsterState
{
    Idle,
    Wandering,
    Hunting,
    WalkingPatroling,
    IdlePatrolling
}
public enum MonsterPersonality
{
    Aggressive,
    Passive
}
public enum MonsterMovementDirection
{
    Up,
    Right,
    Down,
    Left
}


public class MonsterMovement : MonoBehaviour
{

    [SerializeField] FieldMonsterBase fieldbase;

    TargetScanner targetScanner;

    [Header("Movement Data")]

    [SerializeField] MonsterMovementDirection currentDirection;
    public bool isMoving;

    public bool rotatesClockwise = true;

    [SerializeField] float rotationSpeedForIdlePatrol;

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

    Coroutine currentMovementCoroutine;

    [Header("Monster Behaviour Settings")]
    //Enum References
    [SerializeField] MonsterState monsterState;
    [SerializeField] MonsterPersonality monsterPersonality;

    private void Awake()
    {
        StartCoroutine(DelayedLoadData());
    }

    IEnumerator DelayedLoadData()
    {
        yield return new WaitForEndOfFrame();
        fieldbase = GetComponent<FieldMonsterBase>();
        targetScanner = GetComponent<TargetScanner>();

        patrolIndex = 0;
        randVector = Vector2.zero;
        playerTransform = null;
        currentMovementCoroutine = null;

        //Check if the list exists, before we try to extract a value from it.
        if (patrolPositionList != null && patrolPositionList.Count < 0)
        {
            patrolTargetTransform = patrolPositionList[0];
        }

        monsterState = MonsterState.Wandering;
    }


    void Update()
    {
        //If the game is not yet loaded, then nothing must happen
        if (!GameController.instance.isDataLoaded && fieldbase == null)
        {
            Debug.Log("Monster cannot be activated");
            return;
        }

        //The monster will check for a few situations, and if any are true, then it cannot move.
        //   If the monster is stunned.     OR        If the game is not in Free Roam Mode.
        if (fieldbase != null && fieldbase.GetIsMonsterStunned() && GameController.instance != null && GameController.instance.state != GameState.FreeRoam)
        {
            //Return will simply stop the rest of the code from executing
            return;
        }

        // ScanForPlayer();


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
        }


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
                case MonsterState.WalkingPatroling:
                    UpdatePatrol();
                    currentMovementCoroutine = StartCoroutine(MoveInPatrolRoute());
                    break;
                case MonsterState.Idle:
                    break;
                case MonsterState.IdlePatrolling:
                    if (fieldbase.TriggerAreaObject == null) break;

                    currentMovementCoroutine = StartCoroutine(RotateTrigger());
                    break;
                default:
                    break;
            }
        }

        fieldbase.character.HandleUpdate();

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


    void CheckIfPlayerIsWithinRangeForBattle()
    {
        if (currentDistanceWithPlayer < minimumDistanceToTriggerBattle && !fieldbase.GetIsBattleDisabled())
        {
            if (GameController.instance.state != GameState.Battle)
            {
                fieldbase.TriggerAttackFromThisEntity();
            }
        }
    }

    IEnumerator RotateTrigger()
    {
        var _fieldTransform = fieldbase.TriggerAreaObject.gameObject.transform;
        Vector3 _newRotationVector;
        if (rotatesClockwise)
        {
            _newRotationVector = new Vector3(_fieldTransform.rotation.x, _fieldTransform.rotation.y, _fieldTransform.rotation.z - 90);
            currentDirection++;
        }
        else
        {
            _newRotationVector = new Vector3(_fieldTransform.rotation.x, _fieldTransform.rotation.y, _fieldTransform.rotation.z + 90);
            currentDirection--;
        }
        switch (currentDirection)
        {
            case MonsterMovementDirection.Up:
                fieldbase.character.MainAnimator.SetFloat("moveX", 0);
                fieldbase.character.MainAnimator.SetFloat("moveY", 1);
                break;
            case MonsterMovementDirection.Down:
                fieldbase.character.MainAnimator.SetFloat("moveX", 0);
                fieldbase.character.MainAnimator.SetFloat("moveY", -1);
                break;
            case MonsterMovementDirection.Left:
                fieldbase.character.MainAnimator.SetFloat("moveX", -1);
                fieldbase.character.MainAnimator.SetFloat("moveY", 0);
                break;
            case MonsterMovementDirection.Right:
                fieldbase.character.MainAnimator.SetFloat("moveX", 1);
                fieldbase.character.MainAnimator.SetFloat("moveY", 0);
                break;
        }

        yield return fieldbase.TriggerAreaObject.gameObject.transform.DORotate(_newRotationVector, rotationSpeedForIdlePatrol);
        yield return new WaitForSeconds(movementWaitTimer);

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
            StartCoroutine(fieldbase.character.Move(randVector, fieldbase.character.gridparentTransform, CheckIfPlayerIsWithinRangeForBattle));
        }
        else
        {
            StartCoroutine(fieldbase.character.Move(randVector, fieldbase.character.gridparentTransform));
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