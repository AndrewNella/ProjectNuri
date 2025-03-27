using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Portal System that changes the player's location.
/// </summary>

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnpoint;

    Fader sceneFader;

    public Transform SpawnPoint => spawnpoint;

    public void OnPlayerTrigger(PlayerController _player)
    {
        GameController.instance.PauseGame(true);
        PlayerController.instance.StopPlayerAnimator();
        StartCoroutine(TeleportPlayer());
    }

    private void Start()
    {
        sceneFader = FindFirstObjectByType<Fader>();
    }

    IEnumerator TeleportPlayer()
    {

        yield return sceneFader?.FadeRoutine(true);
        Debug.Log("Teleport Player");

        var _destinationPortal = FindObjectsByType<LocationPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(x => x != this && x.destinationPortal == this.destinationPortal);



        PlayerController.instance.PlayerCharacter.SetPositionAndSnapToTile(_destinationPortal.spawnpoint.position);


        yield return sceneFader?.FadeRoutine(false);
        GameController.instance.PauseGame(false);

    }

}
