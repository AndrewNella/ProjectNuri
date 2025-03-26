using System.Collections;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;


public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;

    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnpoint;

    Fader sceneFader;

    public Transform SpawnPoint => spawnpoint;
    public void OnPlayerTrigger(PlayerController _player)
    {
        GameController.instance.PauseGame(true);

        StartCoroutine(SwitchScene());
    }

    private void Start()
    {
        sceneFader = FindFirstObjectByType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        yield return sceneFader?.FadeRoutine(true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);


        Debug.Log("Swap Scenes");


        Portal _destinationPortal = FindObjectsByType<Portal>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault(x => x != this && x.destinationPortal == this.destinationPortal);
        PlayerController.instance.PlayerCharacter.SetPositionAndSnapToTile(_destinationPortal.spawnpoint.position);


        yield return sceneFader?.FadeRoutine(false);
        GameController.instance.PauseGame(false);

        Destroy(gameObject);
    }
}

public enum DestinationIdentifier { A, B, C, D, E }
