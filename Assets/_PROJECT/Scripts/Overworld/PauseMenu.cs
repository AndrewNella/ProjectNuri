using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour, UIEventSelection
{
    [SerializeField] CanvasGroup pauseCanvasGroup;
    [SerializeField] GameObject characterOverworldHUD;
    public bool isPauseMenuActive { get; private set; } = false;

    [Header("First Item For Each Menu")]
    [SerializeField] GameObject firstObject;

    private void Awake()
    {
        if (pauseCanvasGroup == null)
            pauseCanvasGroup = GetComponent<CanvasGroup>();

        if (characterOverworldHUD == null)
            characterOverworldHUD = FindAnyObjectByType<OverworldUI>().gameObject;



    }

    //Enter True to activate the pause menu, or false to disable it.
    public void TogglePauseMenu(bool _incomingBool)
    {
        isPauseMenuActive = _incomingBool;

        PlayerController.instance.SetIsInMenu(isPauseMenuActive);
        characterOverworldHUD.SetActive(!isPauseMenuActive);

        if (isPauseMenuActive)
            SetCurrentlySelectedObject(firstObject);
        else
            ClearCurrentlySelectedObject();



        pauseCanvasGroup.alpha = isPauseMenuActive ? 1 : 0;
        pauseCanvasGroup.interactable = isPauseMenuActive ? true : false;
        pauseCanvasGroup.blocksRaycasts = isPauseMenuActive ? true : false;

        // EventSystem.Set
    }


    //This stops the game.
    public void ExitGame()
    {
        Application.Quit();
    }


    #region Functions to select an object for gamepads to work
    public void SetCurrentlySelectedObject(GameObject _incomingGameObject)
    {
        EventSystem.current.SetSelectedGameObject(_incomingGameObject);
    }

    public void ClearCurrentlySelectedObject()
    {
        EventSystem.current.SetSelectedGameObject(null);

    }
    #endregion

   
}
