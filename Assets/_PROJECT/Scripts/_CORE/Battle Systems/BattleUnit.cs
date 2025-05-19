using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] bool isPlayerUnit;

    [SerializeField] BattleHUD hud;

    public BattleHUD HUD
    {
        get { return hud; }
    }

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    [SerializeField] float animationTimer;
    [SerializeField] float startPositionX;

    // [SerializeField] Animator animator;

    Image image;
    Vector3 originalPos;
    public Entity entity { get; set; }



    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        if (hud == null)
        {
            hud = this.gameObject.GetComponentInChildren<BattleHUD>();
        }
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
        hud.SetData(_incomingEntity);
        // PlayEnterAnimation();
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
