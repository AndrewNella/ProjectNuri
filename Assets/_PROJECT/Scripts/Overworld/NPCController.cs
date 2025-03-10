using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue inputDialogue;
    public void Interact()
    {
        StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue));
    }
}
