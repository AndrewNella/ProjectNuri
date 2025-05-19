#region Using
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kisei.Utility;
#endregion
namespace PhantomPixelStudio.Utility
{
    /// <summary>
    ///     Generic singleton Class. Extend this class to make singleton component.
    ///     Example:
    ///     "public class Foo : GenericSingleton<Foo>"
    ///
    ///     . To get the instance of Foo class, use <code>Foo.instance</code>
    ///     Override <code>Init()</code> method instead of using <code>Awake()</code>
    ///     from this class.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        [SerializeField, Tooltip("If set to true, the gameobject will deactive on Awake")]
        private bool _deactivateOnLoad;

        [SerializeField, Tooltip("If set to true, the singleton will be marked as \"don't destroy on load\"")]
        private bool _dontDestroyOnLoad;

        private bool _isInitialized;

        public static T Instance
        {
            get
            {
                // Instance required for the first time, we look for it
                if (_instance != null)
                {
                    return _instance;
                }

                var instances = Resources.FindObjectsOfTypeAll<T>();
                if (instances == null || instances.Length == 0)
                {
                    return null;
                }

                _instance = instances.FirstOrDefault(i => i.gameObject.scene.buildIndex != -1);

                if (_instance == null)
                {
                    _instance = instances.FirstOrDefault();
                }

                if (Application.isPlaying)
                {
                    _instance?.Init();
                }

                return _instance;
            }
        }

        // If no other monobehaviour request the instance in an awake function
        // executing before this one, no need to search the object.
        protected virtual void Awake()
        {
            if (_instance == null || !_instance || !_instance.gameObject)
            {
                _instance = (T)this;
            }
            else if (_instance != this)
            {
                Debug.LogError($"Another instance of {GetType()} already exist! Destroying self...");
                Destroy(this);
                return;
            }

            _instance.Init();
        }

        /// <summary>
        ///     This function is called when the instance is used the first time
        ///     Put all the initializations you need here, as you would do in Awake
        /// </summary>
        public void Init()
        {
            if (_isInitialized)
            {
                return;
            }

            if (_dontDestroyOnLoad)
            {
                if (transform.parent != null)
                    transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            if (_deactivateOnLoad)
            {
                gameObject.SetActive(false);
            }

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            InternalInit();
            _isInitialized = true;
        }

        /// <summary>
        /// Gets the existing instance or creates a new instance if one doesn't exist.
        /// </summary>
        /// <returns>The singleton instance of type T</returns>
        public static T GetOrCreateInstance()
        {
            if (Instance != null)
                return Instance;

            var gameObject = new GameObject(typeof(T).Name);
            var instance = gameObject.AddComponent<T>();
            gameObject.LogCaution("Creating new Singleton Instance of " + typeof(T).Name);

            //it's automatically initialized in the Awake method
            return instance;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            // Sanity
            if (!Instance/* || gameObject == null*/)
            {
                SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
                _instance = null;
                return;
            }

            if (_dontDestroyOnLoad)
            {
                return;
            }

            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            _instance = null;
        }



        /// Make sure the instance isn't referenced anymore when the user quit, just in case.
        private void OnApplicationQuit()
        {
            _instance = null;
        }

        private void OnDestroy()
        {
            // Clear static listener OnDestroy
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;

            StopAllCoroutines();
            InternalOnDestroy();
            if (_instance != this)
            {
                return;
            }

            _instance = null;
            _isInitialized = false;
        }

        protected virtual void InternalInit() { }

        protected virtual void InternalOnDestroy() { }
    }
}
