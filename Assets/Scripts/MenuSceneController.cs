using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Singleton controller for managing scene transitions
/// Handles loading gameplay scenes based on selected game mode
/// </summary>
public class MenuSceneController : MonoBehaviour
{
    private static MenuSceneController instance;
    public static MenuSceneController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MenuSceneController>();
                if (instance == null)
                {
                    GameObject go = new GameObject("MenuSceneController");
                    instance = go.AddComponent<MenuSceneController>();
                }
            }
            return instance;
        }
    }

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "lobby";
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [Header("Loading Settings")]
    [SerializeField] private bool useAsyncLoading = true;
    [SerializeField] private float minimumLoadingTime = 0.5f;

    private GameMode pendingGameMode;

    private void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Load the gameplay scene with the specified game mode
    /// </summary>
    public void LoadGameplayScene(GameMode mode)
    {
        pendingGameMode = mode;

        // Store the selected game mode in PlayerPrefs for the game scene to read
        PlayerPrefs.SetInt("SelectedGameMode", (int)mode);
        PlayerPrefs.Save();

        Debug.Log($"Loading gameplay scene with game mode: {mode}");

        if (useAsyncLoading)
        {
            StartCoroutine(LoadSceneAsync(gameplaySceneName));
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    /// <summary>
    /// Return to the main menu
    /// </summary>
    public void LoadMenuScene()
    {
        Debug.Log("Loading menu scene");

        if (useAsyncLoading)
        {
            StartCoroutine(LoadSceneAsync(menuSceneName));
        }
        else
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }

    /// <summary>
    /// Reload the current scene
    /// </summary>
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        float startTime = Time.time;

        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (asyncLoad == null)
        {
            Debug.LogError($"Failed to load scene: {sceneName}");
            yield break;
        }

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            // You can display a loading progress bar here
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // Debug.Log($"Loading progress: {progress * 100}%");

            yield return null;
        }

        // Ensure minimum loading time for smooth transition
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadingTime)
        {
            yield return new WaitForSeconds(minimumLoadingTime - elapsedTime);
        }

        Debug.Log($"Scene {sceneName} loaded successfully");
    }

    /// <summary>
    /// Get the currently selected game mode
    /// </summary>
    public GameMode GetSelectedGameMode()
    {
        return pendingGameMode;
    }

    /// <summary>
    /// Check if a scene exists in build settings
    /// </summary>
    public bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameFromPath == sceneName)
                return true;
        }
        return false;
    }
}
