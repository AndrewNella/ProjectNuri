using System;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class InteractionBase : MonoBehaviour, Interactable
{
    public bool isInteractableByPlayer = false;
    public NPCController npcController;
    private void Awake()
    {
        npcController = GetComponent<NPCController>();
        npcController.OnInteractTrigger += Interact;

    }

    public virtual void Interact(Transform _incomingTransform)
    {
        Debug.Log("Interaction is called from the Base");
    }
}
