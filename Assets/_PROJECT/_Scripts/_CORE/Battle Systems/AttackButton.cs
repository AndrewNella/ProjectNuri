using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    public TMP_Text connectedTextBox;

    bool isCurrentlySelected = false;
    // public GameObject current;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (connectedTextBox == null)
        {
            connectedTextBox = GetComponentInChildren<TMP_Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject && !isCurrentlySelected)
        {
            isCurrentlySelected = true;
            // BattleMenuControl.instance.typeText.text = $"Attack Type - ";
        }
        if (EventSystem.current.currentSelectedGameObject != this.gameObject)
        {
            isCurrentlySelected = false;

        }

    }
}
