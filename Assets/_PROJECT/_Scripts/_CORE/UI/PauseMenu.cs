using System.Collections.Generic;
using Kisei.Player;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour, UIEventSelection
{
    [SerializeField] CanvasGroup mainPauseCanvasGroup, currentCanvasGroup;


    [SerializeField] GameObject characterOverworldHUD, inventoryMenu;

    public GameObject OverworldHUD => characterOverworldHUD;


    public bool isPauseMenuActive { get; private set; } = false;

    [Header("Pause Menu Pages")]
    [SerializeField] List<CanvasGroup> listOfPauseMenuPages = new List<CanvasGroup>();
    [SerializeField] List<GameObject> listOfPauseMenuPagesFirstItem = new List<GameObject>();

    private void Awake()
    {
        currentCanvasGroup = listOfPauseMenuPages[0];
        if (mainPauseCanvasGroup == null)
            mainPauseCanvasGroup = GetComponent<CanvasGroup>();

        if (characterOverworldHUD == null)
            characterOverworldHUD = FindAnyObjectByType<OverworldUI>().gameObject;



    }

    public void SwapPages(CanvasGroup _incomingCanvasGroup)
    {
        if (currentCanvasGroup == _incomingCanvasGroup) return;

        if (listOfPauseMenuPages.Contains(_incomingCanvasGroup))
        {
            //Deactive the old page
            currentCanvasGroup.alpha = 0;
            currentCanvasGroup.interactable = false;
            currentCanvasGroup.blocksRaycasts = false;

            //Activate the new page
            _incomingCanvasGroup.alpha = 1;
            _incomingCanvasGroup.interactable = true;
            _incomingCanvasGroup.blocksRaycasts = true;

            //Update the current page
            currentCanvasGroup = _incomingCanvasGroup;

            //Set the current selected object
            int _listIndex = listOfPauseMenuPages.FindIndex(x => x == _incomingCanvasGroup);
            Debug.Log(listOfPauseMenuPagesFirstItem[_listIndex]);
            SetCurrentlySelectedObject(listOfPauseMenuPagesFirstItem[_listIndex]);

        }

    }


    //Enter True to activate the pause menu, or false to disable it.
    public void TogglePauseMenu(bool _incomingBool)
    {
        isPauseMenuActive = _incomingBool;

        PlayerInstanceHUB.Instance.PlayerController.SetIsInMenu(isPauseMenuActive);
        characterOverworldHUD.SetActive(!isPauseMenuActive);

        if (isPauseMenuActive)
        {
            SwapPages(listOfPauseMenuPages[0]);
            SetCurrentlySelectedObject(listOfPauseMenuPagesFirstItem[0]);
            // currentCanvasGroup = listOfPauseMenuPages[0];
        }
        // else
        // ClearCurrentlySelectedObject();



        mainPauseCanvasGroup.alpha = isPauseMenuActive ? 1 : 0;
        mainPauseCanvasGroup.interactable = isPauseMenuActive ? true : false;
        mainPauseCanvasGroup.blocksRaycasts = isPauseMenuActive ? true : false;

        if (!_incomingBool && currentCanvasGroup != listOfPauseMenuPages[0])
        {
            SwapPages(listOfPauseMenuPages[0]);
        }
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
