using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TMP_Text dialogueText;

    int currentDialogueLine;

    Dialogue dialogue;
    [SerializeField] float dialogueLetterWaiterTimer;

    public event Action OnShowDialogue, OnCloseDialogue;

    bool isTyping;
    private void Awake()
    {
        Instance = this;
    }
    public void EnableDialogueBox()
    {
        currentDialogueLine = 0;
        dialogueBox.SetActive(true);
        MainInputActionController.instance.OnInteractTrigger += UpdateDialogue;
        MainInputActionController.instance.OnPauseTrigger += UpdateDialogue;
    }
    public void DisableDialogueBox()
    {
        currentDialogueLine = 0;
        dialogueBox.SetActive(false);
        MainInputActionController.instance.OnInteractTrigger -= UpdateDialogue;
        MainInputActionController.instance.OnPauseTrigger -= UpdateDialogue;
        OnCloseDialogue?.Invoke();

    }

    void UpdateDialogue()
    {
        if (!isTyping)
        {

            ++currentDialogueLine;
            if (currentDialogueLine < dialogue.Lines.Count)
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentDialogueLine]));
            }
            else
            {
                DisableDialogueBox();
            }
        }
    }


    public IEnumerator ShowDialogue(Dialogue _dialogue)
    {

        yield return new WaitForEndOfFrame();
        OnShowDialogue.Invoke();

        dialogue = _dialogue;
        EnableDialogueBox();

        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    public void HandleUpdate()
    {

    }

    public IEnumerator TypeDialogue(string _line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (var letter in _line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueLetterWaiterTimer);
        }
        isTyping = false;
    }
}
