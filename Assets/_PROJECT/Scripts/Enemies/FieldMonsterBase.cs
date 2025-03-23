using System;
using System.Collections;
using UnityEngine;

public class FieldMonsterBase : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Entity fieldEntity;
    [SerializeField] GameObject exclamationSprite;
    [SerializeField] GameObject enemySprite;
    [SerializeField] GameObject TriggerArea;

    Character character;
    public FieldMonsterType FieldBattleType => fieldBattleType;
    public Entity FieldEntity => fieldEntity;
    public Character Character => character;

    [Header("Battle Behaviour Settings")]
    [SerializeField] FieldMonsterType fieldBattleType;
    bool isBattlingDisabled = false;
    bool isMonsterStunned = false;
    [SerializeField] float stunTimeWhenDefeated;

    public bool GetIsMonsterStunned()
    {
        return isMonsterStunned;
    }
    public bool GetIsBattleDisabled()
    {
        return isBattlingDisabled;
    }
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void TriggerAttackFromThisEntity()
    {
        PlayerController.instance.StopPlayerAnimator();
        if (exclamationSprite == null)
        {
            QuickBattleIntro();
        }
        else
        {
            StartCoroutine(LongBattleIntro());
        }
    }

    void QuickBattleIntro()
    {
        GameController.instance.StartSpesificMonsterBattle(fieldEntity, this);

    }
    IEnumerator LongBattleIntro()
    {
        GameController.instance.StartCutsceneState();
        exclamationSprite.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamationSprite.SetActive(false);


        //Move Towards player
        var _difference = PlayerController.instance.transform.position - enemySprite.transform.position;
        var _moveVector = _difference - _difference.normalized;
        _moveVector = new Vector2(Mathf.Round(_moveVector.x), Mathf.Round(_moveVector.y));

        yield return character.Move(_moveVector, enemySprite.transform);

        //Show Dialogue
        if (dialogue != null)
        {
            DialogueManager.Instance.ShowDialogue(dialogue);
        }
        GameController.instance.StartSpesificMonsterBattle(fieldEntity, this);
    }

    public void EscapeStun()
    {
        isBattlingDisabled = true;
        isMonsterStunned = true;
        StartCoroutine(WaitForBattleStunTimer());
    }
    public void OnDefeated()
    {
        switch (fieldBattleType)
        {
            case FieldMonsterType.BattleOnceThenDead:
                Destroy(this.gameObject);
                break;
            case FieldMonsterType.BattleOnceThenDisable:
                isBattlingDisabled = true;
                TriggerArea.SetActive(false);

                break;
            case FieldMonsterType.StunnedAfterBattle:
                EscapeStun();
                break;
            default:
                break;
        }
    }
    IEnumerator WaitForBattleStunTimer()
    {
        Debug.Log("Monster is stunned");
        SpriteRenderer _spriteHolder = null;
        if (enemySprite.TryGetComponent<SpriteRenderer>(out SpriteRenderer _sprite))
        {
            _spriteHolder = _sprite;
            Color oldColour = _spriteHolder.color;
            _spriteHolder.color = new Color(oldColour.r, oldColour.g, oldColour.b, 0.6f);
        }
        yield return new WaitForSeconds(stunTimeWhenDefeated);

        if (_spriteHolder != null)
        {
            Color oldColour = _spriteHolder.color;
            _spriteHolder.color = new Color(oldColour.r, oldColour.g, oldColour.b, 1f);
        }
        isBattlingDisabled = false;
        isMonsterStunned = false;
        Debug.Log("Monster is no longer stunned");

    }
    public enum FieldMonsterType
    {
        BattleOnceThenDead,
        BattleOnceThenDisable,
        StunnedAfterBattle,

    }
}
