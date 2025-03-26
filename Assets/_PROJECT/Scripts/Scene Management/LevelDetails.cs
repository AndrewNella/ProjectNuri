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
    public void LoadAdditiveScene(AdditiveSceneTrigger _incomingScene)
    {
        SceneManager.LoadSceneAsync(listOfScenesConnectedToThisWorld[_incomingScene.thisSceneIndex], LoadSceneMode.Additive);
    }

    public void UnloadScene(AdditiveSceneTrigger _incomingScene)
    {
        if (_incomingScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(listOfScenesConnectedToThisWorld[_incomingScene.thisSceneIndex]);
        }
    }

}
