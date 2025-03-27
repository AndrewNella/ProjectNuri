using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MainInputActionController : MonoBehaviour
{

    public static MainInputActionController instance;

    public event Action OnPauseTrigger;
    public event Action OnInteractTrigger;


    public PlayerActionMap actionMap;

    void Awake()
    {
        instance = this;
        actionMap = new PlayerActionMap();
        actionMap.Enable();

        actionMap.PlayerControllerMap.Pause.performed += OnPauseInput;
        actionMap.PlayerControllerMap.Pause.canceled -= OnPauseInput;
        
        actionMap.PlayerControllerMap.Interact.performed += OnInteractInput;
        actionMap.PlayerControllerMap.Interact.canceled -= OnInteractInput;


    }

    private void OnPauseInput(InputAction.CallbackContext context)
    {
        OnPauseTrigger();
    }
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        OnInteractTrigger();
    }


}
