using System.Collections.Generic;
using UnityEngine;

public class CustomSpriteAnimator
{
    [SerializeField] SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public CustomSpriteAnimator(List<Sprite> _frames, SpriteRenderer _spriteRender, float _frameRate = 0.16f)
    {
        frames = _frames;
        spriteRenderer = _spriteRender;
        frameRate = _frameRate;
    }

    public void StartAnimation()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[0];
    }

    public void UpdateHandler()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }
}
