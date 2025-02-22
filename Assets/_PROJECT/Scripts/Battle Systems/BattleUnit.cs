using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] EntityBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Entity entity { get; set; }
    public void Setup()
    {
        entity = new Entity(_base, level);
        if (isPlayerUnit)
        {

            //Functionality that checks if you are the player or an enemy
            GetComponent<Image>().sprite = entity.Base.FrontSprite;
        }
        else
        {

            GetComponent<Image>().sprite = entity.Base.BackSprite;
        }
    }
}
