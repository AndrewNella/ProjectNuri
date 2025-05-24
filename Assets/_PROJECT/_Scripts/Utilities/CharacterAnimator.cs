using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> movementSprites;
    [SerializeField] List<Sprite> idleSprites;

    //Parameters
    public float moveX { get; set; }
    public float moveY { get; set; }

    public bool isMoving { get; set; }

    //States
    CustomSpriteAnimator movementAnimation, idleAnimation;


    CustomSpriteAnimator currentAnimation;
    //References
    SpriteRenderer spriteRender;
    private void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        movementAnimation = new CustomSpriteAnimator(movementSprites, spriteRender);
        idleAnimation = new CustomSpriteAnimator(idleSprites, spriteRender);

        currentAnimation = movementAnimation;
    }

    private void Update()
    {
        
    }
}
