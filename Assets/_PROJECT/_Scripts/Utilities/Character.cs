using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;


public class Character : MonoBehaviour
{
    [SerializeField] bool isPlayerCharacter = false;
    [SerializeField] Animator characterAnimator;
    public MovementBase movementControl;

    public Animator MainAnimator => characterAnimator;


    void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        Debug.Log(characterAnimator);
        movementControl = GetComponent<MovementBase>();

        if (movementControl != null)
            if (characterAnimator == null)
            {
                movementControl.InitializeMovement();
            }
            else
            {
                movementControl.InitializeMovement(characterAnimator);
            }
    }
    public void HandleUpdate()
    {
        if (movementControl != null)
        {
            characterAnimator.SetBool("isMoving", movementControl.isMoving);
        }
    }

}
