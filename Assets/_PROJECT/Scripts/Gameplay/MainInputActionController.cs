using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MainInputActionController : MonoBehaviour
{

    public static MainInputActionController instance;

    public event Action OnPauseTrigger;


    public PlayerActionMap actionMap;

    void Awake()
    {
        instance = this;
        actionMap = new PlayerActionMap();
        actionMap.Enable();

        MainInputActionController.instance.actionMap.PlayerControllerMap.Pause.performed += OnPauseInput;
        MainInputActionController.instance.actionMap.PlayerControllerMap.Pause.canceled -= OnPauseInput;
    }

    private void OnPauseInput(InputAction.CallbackContext context)
    {
        OnPauseTrigger();
    }


}
