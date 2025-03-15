using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue inputDialogue;
    [SerializeField] Character character;

    NPCState npcState;

    float idleTimer;

    private void Awake()
    {
        character = GetComponent<Character>();
        idleTimer = 0;
    }
    public void Interact()
    {
        StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue));
    }
    public void Update()
    {
        if (npcState == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > 2)
            {
                idleTimer = 0f;
                // StartCoroutine(character.Move());
            }
        }
        character?.HandleUpdate();
    }

    public enum NPCState
    {
        Idle,
        Walking

    }
}


