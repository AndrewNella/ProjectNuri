using UnityEngine;

public class DialogueInteraction : InteractionBase
{
    [SerializeField] Dialogue inputDialogue;
    public override void Interact(Transform _incomingTransform)
    {

        var _movementControls = npcController.NPCCharacter.movementControl; ;
        // Debug.Log($"NPC Controller's state is {npcController.npcState}");
        switch (npcController.npcState)
        {

            case NPCController.NPCState.WalkWaiting:
                npcController.npcState = NPCController.NPCState.Dialogue;

                if (_movementControls != null)
                {
                    _movementControls.LookTowards(_incomingTransform.position);
                }


                StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue, () =>
                {
                    npcController.idleTimer = 0;
                    npcController.npcState = NPCController.NPCState.WalkWaiting;
                }));
                break;
            case NPCController.NPCState.Idle:
                npcController.npcState = NPCController.NPCState.Dialogue;


                if (_movementControls != null)
                {
                    _movementControls.LookTowards(_incomingTransform.position);
                }

                StartCoroutine(DialogueManager.Instance.ShowDialogue(inputDialogue));
                break;
            default:
                Debug.LogError("No correct State Was detected for Dialogue");
                break;
        }
    }
}
