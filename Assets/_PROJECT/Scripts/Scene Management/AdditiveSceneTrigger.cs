using System.Collections.Generic;
using UnityEngine;

public class AdditiveSceneTrigger : MonoBehaviour
{
    [SerializeField] LevelDetails levelDetails;

    [SerializeField] List<AdditiveSceneTrigger> connectedScenes;
    public bool isLoaded;

    public int thisSceneIndex;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
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
        }
    }
}
