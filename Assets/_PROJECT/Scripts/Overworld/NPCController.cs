using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue inputDialogue;
    [SerializeField] Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }
    public void Interact()
    {
        StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue));
    }
    public void Update()
    {
        character?.HandleUpdate();
    }


}


