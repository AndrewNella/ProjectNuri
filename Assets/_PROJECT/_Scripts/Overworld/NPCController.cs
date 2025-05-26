using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour
{
    public event Action<Transform> OnInteractTrigger;
    InteractionBase interaction;
    [Header("Character Data")]
    [SerializeField] bool isInteractableByPlayer = false;
    [SerializeField] Dialogue inputDialogue;
    [SerializeField] Character character;

    public Character NPCCharacter => character;

    public NPCState npcState;
    [Header("Idle Settings")]
    [SerializeField] float maxIdleWaitTime;

    public float idleTimer = 0;

    [Header("Patrol Settings")]
    [SerializeField] List<Vector2> patrolList;
    int currentPatrolIndex = 0;

    public List<Vector2> PatrolPointList => patrolList;
    public int PatrolIndex => currentPatrolIndex;
    private void Awake()
    {
        character = GetComponent<Character>();
        idleTimer = 0;

        interaction = GetComponent<InteractionBase>();


    }
    private void Start()
    {
        if (npcState == NPCState.Patrolling)
        {
            StartCoroutine(WalkToPatrolPoint());
        }
    }
    public void Interact(Transform _initiator)
    {
        if (!isInteractableByPlayer) return;

        OnInteractTrigger!.Invoke(_initiator);

        // switch (npcState)
        // {

        //     case NPCState.WalkWaiting:
        //         npcState = NPCState.Dialogue;
        //         NPCCharacter.movementControl.LookTowards(_initiator.position);
        //         StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue, () =>
        //         {
        //             idleTimer = 0;
        //             npcState = NPCState.WalkWaiting;
        //         }));
        //         break;
        //     case NPCState.Idle:
        //         npcState = NPCState.Dialogue;

        //         Debug.Log($"Incoming Transform's position is {_initiator.position}");
        //         NPCCharacter.movementControl.LookTowards(_initiator.position);

        //         StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue));
        //         break;
        //     default:
        //         break;
        // }
    }

    public void SetPatrolIndex(int _incomingInt)
    {
        currentPatrolIndex = _incomingInt;
    }
    public void Update()
    {

        if (npcState == NPCState.WalkWaiting)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > maxIdleWaitTime)
            {
                idleTimer = 0f;
                if (patrolList.Count > 0)
                {
                    StartCoroutine(WalkToPatrolPoint());
                }

            }
        }
        character?.HandleUpdate();
    }

    public IEnumerator WalkToPatrolPoint()
    {
        if (character == null)
        {
            Debug.LogError("Character component is missing. Cannot walk to patrol point.");
            yield break;
        }

        npcState = NPCState.Walking;

        var _oldPosition = transform.position;

        yield return character.movementControl.Move(patrolList[currentPatrolIndex], character.movementControl.gridparentTransform);

        if (transform.position != _oldPosition)
        {
            Debug.Log("Keep Walking to Patrol Point");
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolList.Count;
        }


        npcState = NPCState.WalkWaiting;

    }
    public enum NPCState
    {
        Idle,
        WalkWaiting,
        Walking,
        Wandering,
        Patrolling,
        MoveTowards,
        Dialogue
    }

}


