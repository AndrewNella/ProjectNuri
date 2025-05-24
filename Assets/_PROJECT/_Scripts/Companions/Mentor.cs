using UnityEngine;

public class Mentor
{
    [SerializeField] MentorBase BaseContainer;
    [SerializeField] int LevelContainer;
    public MentorBase Base { get { return BaseContainer; } }
    public int Level { get { return LevelContainer; } }

    public float rescueChance { get; set; }
    public float trustLevel { get; set; }

}
