using System.Collections.Generic;
using UnityEngine;

public class AdditiveSceneTrigger : MonoBehaviour
{
    [SerializeField] LevelDetails levelDetails;

    [SerializeField] List<AdditiveSceneTrigger> connectedScenes;
    public bool isLoaded;

    [SerializeField] MapArea sceneMapArea;

    public int thisSceneIndex;

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
                levelDetails.LoadAdditiveScene(this);
                levelDetails.SetCurrentScene(this);

                isLoaded = true;
            }

            //Load all the scenes that are attached to this one.
            foreach (var _scene in connectedScenes)
            {
                if (!_scene.isLoaded)
                {
                    levelDetails.LoadAdditiveScene(_scene);
                    _scene.isLoaded = true;
                }

            }

            //Unload all scenes that are not connected
            if (levelDetails.previouslyLoadedMainScene != null)
            {
                var _previouslyLoadedScenes = levelDetails.previouslyLoadedMainScene.connectedScenes;
                foreach (var _scene in _previouslyLoadedScenes)
                {
                    if (!connectedScenes.Contains(_scene) && _scene != this)
                    {
                        levelDetails.UnloadScene(_scene);
                        _scene.isLoaded = false;
                    }
                }
            }

            //Assigns a new map area for the currently loaded Scene, if this area has a MapArea
            if (sceneMapArea == null) GameController.instance.SetCurrentMapAreaToDefault();
            else GameController.instance.UpdateCurrentMapArea(sceneMapArea);

        }
    }
}
