using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDetails : MonoBehaviour
{
    [Header("Connected World Data")]
    [SerializeField] List<string> listOfScenesConnectedToThisWorld = new List<string>();


    public AdditiveSceneTrigger currentlyLoadedMainScene;
    public AdditiveSceneTrigger previouslyLoadedMainScene;

    public List<string> ListOfScenesConnectedToThisWorld => listOfScenesConnectedToThisWorld;



    public void SetCurrentScene(AdditiveSceneTrigger _incomingScene)
    {
        previouslyLoadedMainScene = currentlyLoadedMainScene;
        currentlyLoadedMainScene = _incomingScene;

    }
    public void SetPreviousScene(AdditiveSceneTrigger _incomingScene)
    {
        previouslyLoadedMainScene = _incomingScene;

    }


    public void LoadAdditiveScene(AdditiveSceneTrigger _incomingScene)
    {
        var _operation = SceneManager.LoadSceneAsync(listOfScenesConnectedToThisWorld[_incomingScene.sceneIndex], LoadSceneMode.Additive);

        // Find anything that can be saved within the scene
        _operation.completed += (AsyncOperation op) =>
        {
            _incomingScene.LoadSceneData();
        };
    }

    public void UnloadScene(AdditiveSceneTrigger _incomingScene)
    {
        if (_incomingScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(listOfScenesConnectedToThisWorld[_incomingScene.sceneIndex]);
        }
    }

    public void OverRideUnloadScene(AdditiveSceneTrigger _incomingScene)
    {

        SceneManager.UnloadSceneAsync(listOfScenesConnectedToThisWorld[_incomingScene.sceneIndex]);

    }



}
