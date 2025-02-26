using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] bool isPlayerUnit;

    [SerializeField] float animationTimer;
    [SerializeField] float startPositionX;

    [SerializeField] Animator animator;

    Image image;
    Vector3 originalPos;
    public Entity entity { get; set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
    }
    public void Setup(Entity _incomingEntity)
    {
        entity = _incomingEntity;
        if (isPlayerUnit)
        {

            //Functionality that checks if you are the player or an enemy
            image.sprite = entity.Base.BackSprite;
        }
        else
        {

            image.sprite = entity.Base.FrontSprite;
        }
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (!isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(startPositionX, originalPos.y);
            image.transform.DOLocalMove(originalPos, animationTimer);
            // animator.StopPlayback();
        }
    }
}
