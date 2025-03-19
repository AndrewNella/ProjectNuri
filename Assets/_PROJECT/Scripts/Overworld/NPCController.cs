using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour, Interactable
{
    [Header("Character Data")]
    [SerializeField] bool isInteractableByPlayer = false;
    [SerializeField] Dialogue inputDialogue;
    public Character character;

    public NPCState npcState { get; set; }
    [Header("Idle Settings")]
    [SerializeField] float maxIdleWaitTime;

    float idleTimer = 0;

    [Header("Patrol Settings")]
    [SerializeField] List<Vector2> patrolList;
    int currentPatrolIndex = 0;


    private void Awake()
    {
        character = GetComponent<Character>();
        idleTimer = 0;
    }

    public void Interact(Transform _initiator)
    {
        if (!isInteractableByPlayer) return;
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Dialogue;
            character.LookTowards(_initiator.position);
            StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue, () =>
            {
                idleTimer = 0;
                npcState = NPCState.Idle;
            }));
        }
    }
    public void Update()
    {

        if (npcState == NPCState.Idle)
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
        npcState = NPCState.Walking;

        var _oldPosition = transform.position;

        yield return character.Move(patrolList[currentPatrolIndex], character.gridparentTransform);

        if (transform.position != _oldPosition)
        {
            Debug.Log("Keep Walking to Patrol Point");
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolList.Count;
        }


        npcState = NPCState.Idle;

    }
    public enum NPCState
    {
        Idle,
        Walking,
        Wandering,
        Patrolling,
        MoveTowards,
        Dialogue
    }

}


