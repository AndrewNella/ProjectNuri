using System.Collections;
using UnityEngine;

public class FieldMonsterBase : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Entity fieldEntity;
    [SerializeField] GameObject exclamationSprite;
    [SerializeField] GameObject enemySprite;
    Character character;

    public Entity FieldEntity => fieldEntity;
    public Character Character => character;

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
        GameController.instance.StartSpesificMonsterBattle(fieldEntity);

    }
    IEnumerator LongBattleIntro()
    {
        GameController.instance.StartCutsceneState();
        exclamationSprite.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamationSprite.SetActive(true);


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
        GameController.instance.StartSpesificMonsterBattle(fieldEntity);
    }

    public void OnDefeated()
    {
        Destroy(this.gameObject);
    }
}
