using UnityEngine;

[CreateAssetMenu(fileName = "MentorBase", menuName = "Scriptable Objects/MentorBase")]
public class MentorBase : ScriptableObject
{
    [SerializeField] string mentorName;

    [TextArea]
    [SerializeField] string mentorDescription;

    [SerializeField] Sprite mentorSprite;

    [SerializeField] float mentorTrustLevel;

    [SerializeField] float mentorRescueChance;

    public string Name => mentorName;

    public string Description => mentorDescription;

    public Sprite MentorSprite => mentorSprite;

    public float MentorRescueChance => mentorRescueChance;
}
