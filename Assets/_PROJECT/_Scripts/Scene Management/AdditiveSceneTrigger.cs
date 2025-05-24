using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneTrigger : MonoBehaviour
{
    [SerializeField] LevelDetails levelDetails;

    [SerializeField] List<AdditiveSceneTrigger> connectedScenes;
    public bool isLoaded;

    [SerializeField] MapArea sceneMapArea;

    public int sceneIndex;

    public List<SavableEntity> savableEntitiesWithinthisScene;


    private void Awake()
    {
        if (sceneMapArea == null)
        {
            sceneMapArea = gameObject.GetComponent<MapArea>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player entered New Area");
            levelDetails.SetCurrentScene(this);

            //Load the Current Scene that was entered
            if (!isLoaded)
            {
                isLoaded = true;
                levelDetails.LoadAdditiveScene(this);
                levelDetails.SetCurrentScene(this);
            }

            //Load all the scenes that are attached to this one.
            foreach (var _scene in connectedScenes)
            {
                if (!_scene.isLoaded)
                {
                    _scene.isLoaded = true;
                    levelDetails.LoadAdditiveScene(_scene);
                }

            }

            //Unload all scenes that are not connected
            if (levelDetails.previouslyLoadedMainScene != null)
            {
                var _previouslyLoadedScenes = levelDetails.previouslyLoadedMainScene.connectedScenes;
                foreach (var _oldScene in _previouslyLoadedScenes)
                {
                    if (!connectedScenes.Contains(_oldScene) && _oldScene != this)
                    {
                        SaveSceneData();

                        levelDetails.UnloadScene(_oldScene);
                        _oldScene.isLoaded = false;
                    }
                }

            }



            //Assigns a new map area for the currently loaded Scene, if this area has a MapArea
            if (sceneMapArea == null) GameController.instance.SetCurrentMapAreaToDefault();
            else GameController.instance.UpdateCurrentMapArea(sceneMapArea);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (this == null || !gameObject.activeInHierarchy) return;

            StartCoroutine(DelayedSceneUnloader());

        }
    }

    IEnumerator DelayedSceneUnloader()
    {
        yield return new WaitForEndOfFrame();

        if (this == null || !gameObject.activeInHierarchy) yield break;

        var _currentLevelDetailMainScene = levelDetails.currentlyLoadedMainScene;


        // Looks at all the connected scenes, then sees if it any of these scenes are either the main scene, or in the main scene's connected scenes. 
        //If it is not either, then it is unloaded.
        foreach (AdditiveSceneTrigger _connectedSceneTrigger in connectedScenes)
        {
            if (_currentLevelDetailMainScene != _connectedSceneTrigger && !_currentLevelDetailMainScene.connectedScenes.Contains(_connectedSceneTrigger))
            {
                _connectedSceneTrigger.SaveSceneData();
                levelDetails.UnloadScene(_connectedSceneTrigger);
                _connectedSceneTrigger.isLoaded = false;
            }
        }

        //If this scene is not within the connected scenes, then it must also be unloaded.
        if (!_currentLevelDetailMainScene.connectedScenes.Contains(this))
        {
            SaveSceneData();
            levelDetails.UnloadScene(this);
            isLoaded = false;
        }
    }


    public List<SavableEntity> GetListOfEntitiesToSaveInScene()
    {
        List<SavableEntity> _savableEntitiesWithinScene = new List<SavableEntity>();
        Scene _targetScene = SceneManager.GetSceneByName(levelDetails.ListOfScenesConnectedToThisWorld[sceneIndex]);
        // Debug.Log($"Target Scene is {_targetScene.name}");

        SavableEntity[] AllEntitiesInGame = FindObjectsByType<SavableEntity>(FindObjectsSortMode.None);

        foreach (var savableEntity in AllEntitiesInGame)
        {
            if (savableEntity.gameObject.scene == _targetScene)
            {
                // Debug.Log($"New entity saved.");
                _savableEntitiesWithinScene.Add(savableEntity);
            }
        }

        // Debug.Log($"Entities saved: {_savableEntitiesWithinScene}");
        return _savableEntitiesWithinScene;
    }

    public void LoadSceneData()
    {
        // Debug.Log("Loading Scene is done. Level data is being loaded.");

        savableEntitiesWithinthisScene = GetListOfEntitiesToSaveInScene();
        SavingSystem.i.RestoreEntityStates(savableEntitiesWithinthisScene);
    }

    public void SaveSceneData()
    {

        savableEntitiesWithinthisScene = GetListOfEntitiesToSaveInScene();
        SavingSystem.i.CaptureEntityStates(savableEntitiesWithinthisScene);

    }
}
