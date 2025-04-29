using UnityEngine;

public class DefeatMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup mainPauseCanvasGroup;
    public static DefeatMenu instance;

    private void Awake()
    {
        instance = this;
        if (mainPauseCanvasGroup == null) mainPauseCanvasGroup = GetComponent<CanvasGroup>();

    }
    void Start()
    {
        GameController.instance.playerIsDefeated += EnableDefeatMenu;

    }

    void EnableDefeatMenu()
    {
        PlayerController.instance.enablePlayerInputs = false;

        mainPauseCanvasGroup.alpha = 1;
        mainPauseCanvasGroup.blocksRaycasts = true;
        mainPauseCanvasGroup.interactable = true;
    }
    //This stops the game.
    public void ExitGame()
    {
        Application.Quit();
    }
}
