using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
#endif

/// <summary>
/// GameManagerCore: core implementation of a GameManager singleton. If a user-defined
/// GameManager.cs exists, it will be a separate class; this file provides the behavior
/// requested in the task under a different name to avoid collisions.
/// Place this component on a GameObject named 'GameManager' in the _Boot scene.
/// </summary>
public class GameManagerCore : MonoBehaviour
{
    public enum GameState { Iniciando, MenuPrincipal, Gameplay }

    private static GameManagerCore _instance;
    public static GameManagerCore Instance => _instance;

    [Header("Optional PlayerInput holder (assign GameObject with PlayerInput)")]
    public MonoBehaviour playerInputHolder;
    [Header("Scene names")]
    public string guiSceneName = "GUI";
    public string bootSceneName = "_Boot";

    private GameState _state = GameState.Iniciando;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManagerCore: Awake - instance created. State=" + _state);
    }

    private void Start()
    {
        SetState(GameState.Iniciando);
#if ENABLE_INPUT_SYSTEM
        AllocateInputToPlayer();
#else
        Debug.Log("GameManagerCore: Input System not enabled or unavailable.");
#endif
    }

    private void SetState(GameState newState)
    {
        if (_state == newState) return;
        Debug.Log($"GameManagerCore: State change {_state} -> {newState}");
        _state = newState;
        // react to certain states: load/unload GUI when entering/exiting Gameplay
        if (_state == GameState.Gameplay)
        {
            // load GUI additively if not already loaded
            if (!IsSceneLoaded(guiSceneName))
                StartCoroutine(LoadGuiAdditive());
        }
        else
        {
            // if leaving gameplay, unload GUI if loaded
            if (IsSceneLoaded(guiSceneName))
                StartCoroutine(UnloadGui());
        }
    }

    public void ForceSceneChange(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameManagerCore.ForceSceneChange: empty sceneName");
            return;
        }
        StartCoroutine(DoForceSceneChange(sceneName));
    }

    private IEnumerator DoForceSceneChange(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (op == null)
        {
            Debug.LogError($"GameManagerCore: Failed to start loading scene '{sceneName}'. Ensure it's in Build Settings.");
            yield break;
        }
        while (!op.isDone) yield return null;

        if (sceneName.ToLowerInvariant().Contains("menu")) SetState(GameState.MenuPrincipal);
        else if (sceneName.ToLowerInvariant().Contains("sample") || sceneName.ToLowerInvariant().Contains("game")) SetState(GameState.Gameplay);
        else Debug.Log($"GameManagerCore: Scene '{sceneName}' loaded. State remains {_state}.");
    }

    public void RequestSceneChange(string sceneName) => ForceSceneChange(sceneName);

    public void StartBootLoad(string targetSceneName)
    {
        if (string.IsNullOrEmpty(targetSceneName)) { Debug.LogError("GameManagerCore.StartBootLoad: empty target"); return; }
        StartCoroutine(BootLoadCoroutine(targetSceneName));
    }

    private IEnumerator BootLoadCoroutine(string targetSceneName)
    {
        Debug.Log($"GameManagerCore: Boot loading target '{targetSceneName}'");
        var loadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        if (loadOp == null) { Debug.LogError($"GameManagerCore: Failed to start loading scene '{targetSceneName}'."); yield break; }
        while (!loadOp.isDone) yield return null;

        var loadedScene = SceneManager.GetSceneByName(targetSceneName);
        if (loadedScene.IsValid()) SceneManager.SetActiveScene(loadedScene);

        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            var sc = SceneManager.GetSceneAt(i);
            if (!sc.IsValid() || !sc.isLoaded) continue;
            if (string.Equals(sc.name, targetSceneName, System.StringComparison.OrdinalIgnoreCase)) continue;

            string filename = System.IO.Path.GetFileNameWithoutExtension(sc.path);
            bool isBootName = string.Equals(sc.name, "_Boot", System.StringComparison.OrdinalIgnoreCase) || string.Equals(filename, "_Boot", System.StringComparison.OrdinalIgnoreCase);
            bool containsGM = SceneContainsGameManager(sc);
            if (isBootName || containsGM)
            {
                Debug.Log($"GameManagerCore: Unloading boot scene '{sc.name}' (containsGM={containsGM})");
                SceneManager.UnloadSceneAsync(sc);
            }
        }

        if (targetSceneName.ToLowerInvariant().Contains("menu")) SetState(GameState.MenuPrincipal);
        else SetState(GameState.Gameplay);
    }

    private IEnumerator LoadGuiAdditive()
    {
        if (string.IsNullOrEmpty(guiSceneName)) yield break;
        if (IsSceneLoaded(guiSceneName)) yield break;
        Debug.Log($"GameManagerCore: Loading GUI scene '{guiSceneName}' additively.");
        var op = SceneManager.LoadSceneAsync(guiSceneName, LoadSceneMode.Additive);
        if (op == null) { Debug.LogWarning($"GameManagerCore: Failed to start loading GUI scene '{guiSceneName}'."); yield break; }
        while (!op.isDone) yield return null;
        Debug.Log("GameManagerCore: GUI scene loaded.");
    }

    private IEnumerator UnloadGui()
    {
        if (string.IsNullOrEmpty(guiSceneName)) yield break;
        if (!IsSceneLoaded(guiSceneName)) yield break;
        Debug.Log($"GameManagerCore: Unloading GUI scene '{guiSceneName}'.");
        var op = SceneManager.UnloadSceneAsync(guiSceneName);
        if (op == null) { Debug.LogWarning($"GameManagerCore: Failed to start unloading GUI scene '{guiSceneName}'."); yield break; }
        while (!op.isDone) yield return null;
        Debug.Log("GameManagerCore: GUI scene unloaded.");
    }

    private bool SceneContainsGameManager(Scene sc)
    {
        if (!sc.IsValid() || !sc.isLoaded) return false;
        try
        {
            var roots = sc.GetRootGameObjects();
            foreach (var go in roots)
            {
                if (go == null) continue;
                if (go.GetComponent<GameManagerCore>() != null) return true;
                if (go.GetComponentInChildren<GameManagerCore>(true) != null) return true;
            }
        }
        catch { }
        return false;
    }

    private bool IsSceneLoaded(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        var sc = SceneManager.GetSceneByName(name);
        return sc.IsValid() && sc.isLoaded;
    }

#if ENABLE_INPUT_SYSTEM
    private void AllocateInputToPlayer()
    {
        if (playerInputHolder == null) { Debug.Log("GameManagerCore: No playerInputHolder assigned."); return; }
        var pi = playerInputHolder.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (pi == null) { Debug.Log("GameManagerCore: playerInputHolder has no PlayerInput."); return; }
        var devices = InputSystem.devices;
        if (devices.Count == 0) { Debug.Log("GameManagerCore: No devices available."); return; }
        var device = devices[0];
        try { InputUser.PerformPairingWithDevice(device, pi.user); Debug.Log($"GameManagerCore: Paired '{device.displayName}' to PlayerInput."); }
        catch (System.Exception ex) { Debug.LogWarning($"GameManagerCore: Failed to pair device: {ex}"); }
    }
#endif
}