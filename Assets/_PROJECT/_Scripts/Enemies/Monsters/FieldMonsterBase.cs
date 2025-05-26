using System;
using System.Collections;
using UnityEngine;
using Kisei.Player;

/// <summary>
/// Base Controller for Monsters
/// </summary>
public class FieldMonsterBase : MonoBehaviour, ISavable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Entity fieldEntity;
    [SerializeField] GameObject exclamationSprite;
    [SerializeField] GameObject enemySprite;
    [SerializeField] GameObject triggerArea;

    public GameObject TriggerAreaObject => triggerArea;
    MonsterMovement fieldMonster = null;

    public Character character;
    public BattleResultType FieldBattleType => fieldBattleType;
    public Entity FieldEntity => fieldEntity;

    [Header("Battle Behaviour Settings")]
    [SerializeField] BattleResultType fieldBattleType;
    public bool isBattlingDisabled = false;
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
        fieldMonster = GetComponent<MonsterMovement>();
    }

    public void TriggerAttackFromThisEntity()
    {
        PlayerInstanceHUB.Instance.PlayerController.StopPlayerAnimator();
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
        var _difference = PlayerInstanceHUB.Instance.PlayerController.transform.position - enemySprite.transform.position;
        var _moveVector = _difference - _difference.normalized;
        _moveVector = new Vector2(Mathf.Round(_moveVector.x), Mathf.Round(_moveVector.y));

        yield return character.movementControl.Move(_moveVector, enemySprite.transform);

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
        StartCoroutine(WaitUntilStunTimerIsDone());
    }
    public void OnDefeated()
    {
        switch (fieldBattleType)
        {
            case BattleResultType.BattleOnceThenDead:
                StopAllCoroutines();
                fieldMonster?.StopAllCoroutines();
                Destroy(character.movementControl.gridparentTransform.gameObject);
                break;
            case BattleResultType.BattleOnceThenDisable:
                isBattlingDisabled = true;
                TriggerAreaObject.SetActive(false);

                break;
            case BattleResultType.StunnedAfterBattle:
                EscapeStun();
                break;
            default:
                break;
        }
    }

    public void OnWonBattle()
    {
        StopAllCoroutines();
        Destroy(character.movementControl.gridparentTransform.gameObject);
    }
    public object CaptureState()
    {
        Debug.Log("Monster State is Saved");
        return isBattlingDisabled;
    }

    public void RestoreState(object state)
    {
        isBattlingDisabled = (bool)state;
    }

    IEnumerator WaitUntilStunTimerIsDone()
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
    public enum BattleResultType
    {
        BattleOnceThenDead,
        BattleOnceThenDisable,
        StunnedAfterBattle,

    }
}
