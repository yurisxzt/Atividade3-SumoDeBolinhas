using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
#endif

/// <summary>
/// GameManager singleton:
/// - Centraliza todo acesso ao SceneManager (apenas esta classe deve trocar cenas)
/// - Mantém o estado do jogo (enum simplificado)
/// - Aloca inputs para o jogador (suporte ao Input System é opcional/condicional)
/// Coloque este script em uma cena inicial (por exemplo a cena _Boot) ou ele será criado automaticamente no primeiro carregamento.
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Iniciando,
        MenuPrincipal,
        Gameplay,
        // adicione mais estados se necessário
    }

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // try to find existing in scene (use FindFirstObjectByType to avoid deprecated API)
#if UNITY_2022_2_OR_NEWER
                _instance = FindFirstObjectByType<GameManager>();
#else
                _instance = FindObjectOfType<GameManager>();
#endif
                if (_instance == null)
                {
                    var go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Debug")]
    [SerializeField]
    private GameState initialState = GameState.Iniciando;

    public GameState State { get; private set; }

    public event Action<GameState> OnStateChanged;

    private bool _isLoadingScene;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        State = initialState;
    }

    private void Start()
    {
        Debug.Log($"GameManager started. State={State}");
        EnsureSingleAudioListener();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // set initial state based on currently active scene
        var active = SceneManager.GetActiveScene();
        MapSceneToState(active.name);
        EnsureSingleAudioListener();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MapSceneToState(scene.name);
        EnsureSingleAudioListener();
    }

    private void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        if (listeners == null || listeners.Length == 0)
        {
            var listenerObject = new GameObject("AudioListener");
            listenerObject.AddComponent<AudioListener>();
            return;
        }

        var activeScene = SceneManager.GetActiveScene();
        AudioListener preferred = null;

        for (int i = 0; i < listeners.Length; i++)
        {
            var listener = listeners[i];
            if (listener == null) continue;
            if (listener.gameObject.scene == activeScene)
            {
                preferred = listener;
                break;
            }
        }

        if (preferred == null)
            preferred = listeners[0];

        for (int i = 0; i < listeners.Length; i++)
        {
            var listener = listeners[i];
            if (listener == null) continue;
            listener.enabled = listener == preferred;
        }
    }

    private void MapSceneToState(string sceneName)
    {
        // Map specific scene names to game states
        switch (sceneName)
        {
            case "_Boot":
                SetState(GameState.Iniciando);
                break;
            case "MenuPrincipal":
                SetState(GameState.MenuPrincipal);
                break;
            case "SampleScene":
                SetState(GameState.Gameplay);
                break;
            // unlisted scenes keep the current state (e.g., Splash)
        }
    }

    /// <summary>
    /// Solicita uma mudança de cena. Apenas o GameManager chama SceneManager.
    /// Retorna true se a mudança foi iniciada.
    /// </summary>
    public bool RequestSceneChange(string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("GameManager: já está carregando uma cena.");
            return false;
        }

        // método simples de autorização: não permite troca quando estamos iniciando
        if (State == GameState.Iniciando)
        {
            Debug.LogWarning($"GameManager: mudança para '{sceneName}' negada — estado atual: {State}");
            return false;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Single));
        return true;
    }

    /// <summary>
    /// Força a troca de cena independentemente do estado.
    /// </summary>
    public void ForceSceneChange(string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("GameManager: já está carregando uma cena.");
            return;
        }
        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Single));
    }

    /// <summary>
    /// Troca o estado do jogo e notifica ouvintes.
    /// </summary>
    public void SetState(GameState newState)
    {
        if (State == newState) return;
        State = newState;
        Debug.Log($"GameManager: estado alterado para {State}");
        OnStateChanged?.Invoke(State);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode)
    {
        _isLoadingScene = true;

        Debug.Log($"GameManager: carregando cena '{sceneName}' (mode={mode})...");
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        if (op == null)
        {
            Debug.LogError($"GameManager: cena '{sceneName}' não encontrada ou LoadSceneAsync retornou null.");
            _isLoadingScene = false;
            yield break;
        }

        // aguarda término
        while (!op.isDone)
            yield return null;

        // pequena espera para garantir que objetos Awake/OnEnable já rodaram
        yield return null;
        EnsureSingleAudioListener();

        // após carregar, alocar entradas para jogador(s)
        try
        {
            AllocateInputToPlayer();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"GameManager: falha ao alocar inputs: {ex}");
        }

        // If we just entered a Gameplay scene, ensure the GUI scene is loaded additively.
        if (sceneName != null && (sceneName.ToLowerInvariant().Contains("sample") || sceneName.ToLowerInvariant().Contains("game")))
        {
            yield return StartCoroutine(EnsureGuiLoaded());
            SetState(GameState.Gameplay);
        }
        else
        {
            // leaving gameplay: unload GUI if present
            yield return StartCoroutine(EnsureGuiUnloaded());
        }

        _isLoadingScene = false;
        Debug.Log($"GameManager: cena '{sceneName}' carregada.");
    }

    /// <summary>
    /// Called from a Boot scene to load the real target scene additively and then unload the Boot scene.
    /// This ensures only GameManager touches SceneManager.
    /// </summary>
    public void StartBootLoad(string targetSceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("GameManager: já está carregando uma cena.");
            return;
        }
        StartCoroutine(LoadSceneFromBootCoroutine(targetSceneName));
    }

    private IEnumerator LoadSceneFromBootCoroutine(string targetSceneName)
    {
        _isLoadingScene = true;

        var currentScene = SceneManager.GetActiveScene();
        Debug.Log($"GameManager: Boot sequence — carregando '{targetSceneName}' additivamente...");
        var loadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        if (loadOp == null)
        {
            Debug.LogError($"GameManager: falha ao iniciar carregamento de '{targetSceneName}'.");
            _isLoadingScene = false;
            yield break;
        }

        while (!loadOp.isDone)
            yield return null;

        var loadedScene = SceneManager.GetSceneByName(targetSceneName);
        if (loadedScene.IsValid())
            SceneManager.SetActiveScene(loadedScene);

        EnsureSingleAudioListener();

        // unload the boot scene
        var unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        if (unloadOp != null)
        {
            while (!unloadOp.isDone)
                yield return null;
        }

        // allow inputs to be allocated in the newly loaded scene
        yield return null;
        AllocateInputToPlayer();

        // If the boot target is a gameplay scene, ensure GUI loaded
        if (targetSceneName != null && (targetSceneName.ToLowerInvariant().Contains("sample") || targetSceneName.ToLowerInvariant().Contains("game")))
        {
            yield return StartCoroutine(EnsureGuiLoaded());
            SetState(GameState.Gameplay);
        }
        else if (targetSceneName != null && targetSceneName.ToLowerInvariant().Contains("menu"))
        {
            // ensure GUI is not loaded when entering menu
            yield return StartCoroutine(EnsureGuiUnloaded());
            SetState(GameState.MenuPrincipal);
        }
        _isLoadingScene = false;
        Debug.Log($"GameManager: Boot sequence complete. Loaded '{targetSceneName}'.");
    }

    private bool IsSceneLoaded(string name)
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            var sc = SceneManager.GetSceneAt(i);
            if (!sc.IsValid() || !sc.isLoaded) continue;
            if (string.Equals(sc.name, name, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    private IEnumerator EnsureGuiLoaded()
    {
        const string guiName = "GUI";
        if (IsSceneLoaded(guiName)) yield break;
        var op = SceneManager.LoadSceneAsync(guiName, LoadSceneMode.Additive);
        if (op == null) { Debug.LogWarning("GameManager: GUI scene not found or not in Build Settings."); yield break; }
        while (!op.isDone) yield return null;
        yield return null;
    }

    private IEnumerator EnsureGuiUnloaded()
    {
        const string guiName = "GUI";
        if (!IsSceneLoaded(guiName)) yield break;
        var sc = SceneManager.GetSceneByName(guiName);
        if (!sc.IsValid()) yield break;
        var op = SceneManager.UnloadSceneAsync(sc);
        if (op == null) yield break;
        while (!op.isDone) yield return null;
        yield return null;
    }

    /// <summary>
    /// Aloca o primeiro dispositivo disponível ao primeiro PlayerInput encontrado.
    /// Esse código só é compilado se o Input System estiver disponível (símbolo ENABLE_INPUT_SYSTEM).
    /// Implementação simples adequada para single-player.
    /// </summary>
    private void AllocateInputToPlayer()
    {
#if ENABLE_INPUT_SYSTEM
        var devices = InputSystem.devices;
        if (devices.Count == 0)
        {
            Debug.Log("GameManager: nenhum dispositivo do Input System detectado.");
            return;
        }

        // Use PlayerInput.all to avoid obsolete FindObjectOfType usage
        var allPlayers = PlayerInput.all;
        PlayerInput player = null;
        if (allPlayers.Count > 0)
            player = allPlayers[0];

        if (player == null)
        {
            Debug.Log("GameManager: nenhum PlayerInput encontrado na cena.");
            return;
        }

        var firstDevice = devices[0];

        try
        {
            var user = player.user;
            if (!user.valid)
            {
                user = InputUser.PerformPairingWithDevice(firstDevice);
            }

            try
            {
                player.SwitchCurrentControlScheme(player.currentControlScheme, firstDevice);
            }
            catch (Exception)
            {
                InputUser.PerformPairingWithDevice(firstDevice, user);
            }

            Debug.Log($"GameManager: device '{firstDevice}' alocado ao PlayerInput '{player.gameObject.name}'.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"GameManager: erro ao alocar dispositivo: {ex}");
        }
#else
        Debug.Log("GameManager: Input System não habilitado — pulando alocação de inputs.");
#endif
    }
}









