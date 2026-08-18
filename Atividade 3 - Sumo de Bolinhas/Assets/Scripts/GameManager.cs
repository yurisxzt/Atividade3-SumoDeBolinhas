using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
#endif

/// <summary>
/// GameManager Singleton:
/// - Centraliza as trocas de cena
/// - Mantém o estado do jogo
/// - Gerencia os inputs
/// - Controla as quedas dos jogadores
/// - Faz respawn nas duas primeiras quedas
/// - Finaliza a partida na terceira queda
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Iniciando,
        MenuPrincipal,
        Gameplay
    }

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
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

    // =========================================================
    // SISTEMA DE QUEDAS
    // =========================================================

    [Header("Partida - Quedas")]
    [SerializeField]
    private int quedasParaVencer = 3;

    private int player1Quedas = 0;
    private int player2Quedas = 0;

    public int Player1Quedas => player1Quedas;
    public int Player2Quedas => player2Quedas;

    private bool partidaFinalizada = false;

    // =========================================================
    // AWAKE
    // =========================================================

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

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Debug.Log(
            $"GameManager started. State={State}"
        );

        EnsureSingleAudioListener();
    }

    // =========================================================
    // ENABLE / DISABLE
    // =========================================================

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        var active =
            SceneManager.GetActiveScene();

        MapSceneToState(active.name);

        EnsureSingleAudioListener();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // =========================================================
    // CENA CARREGADA
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        MapSceneToState(scene.name);

        EnsureSingleAudioListener();

        ApplyGameplayCamera(scene.name);
    }

    // =========================================================
    // CAMERA DA GAMEPLAY
    // =========================================================

    private void ApplyGameplayCamera(
        string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        bool isGameplayScene =
            sceneName.ToLowerInvariant().Contains("sample") ||
            sceneName.ToLowerInvariant().Contains("game");

        if (!isGameplayScene)
            return;

        Camera mainCamera =
            Camera.main;

        if (mainCamera == null)
            return;

        mainCamera.transform.position =
            new Vector3(
                0f,
                16.5f,
                -20.5f
            );

        mainCamera.transform.rotation =
            Quaternion.Euler(
                50f,
                0f,
                0f
            );

        mainCamera.fieldOfView = 58f;

        mainCamera.orthographic = false;
    }

    // =========================================================
    // AUDIO LISTENER
    // =========================================================

    private void EnsureSingleAudioListener()
    {
        AudioListener[] listeners =
            FindObjectsOfType<AudioListener>(true);

        if (listeners == null ||
            listeners.Length == 0)
        {
            var listenerObject =
                new GameObject("AudioListener");

            listenerObject.AddComponent<AudioListener>();

            return;
        }

        var activeScene =
            SceneManager.GetActiveScene();

        AudioListener preferred = null;

        for (int i = 0;
             i < listeners.Length;
             i++)
        {
            var listener =
                listeners[i];

            if (listener == null)
                continue;

            if (listener.gameObject.scene ==
                activeScene)
            {
                preferred = listener;
                break;
            }
        }

        if (preferred == null)
            preferred = listeners[0];

        for (int i = 0;
             i < listeners.Length;
             i++)
        {
            var listener =
                listeners[i];

            if (listener == null)
                continue;

            listener.enabled =
                listener == preferred;
        }
    }

    // =========================================================
    // ESTADO DA CENA
    // =========================================================

    private void MapSceneToState(
        string sceneName)
    {
        switch (sceneName)
        {
            case "_Boot":

                SetState(
                    GameState.Iniciando
                );

                break;

            case "MenuPrincipal":

                SetState(
                    GameState.MenuPrincipal
                );

                break;

            case "SampleScene":

                SetState(
                    GameState.Gameplay
                );

                break;
        }
    }

    // =========================================================
    // TROCA DE CENA
    // =========================================================

    public bool RequestSceneChange(
        string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning(
                "GameManager: já está carregando uma cena."
            );

            return false;
        }

        if (State ==
            GameState.Iniciando)
        {
            Debug.LogWarning(
                $"GameManager: mudança para '{sceneName}' negada — estado atual: {State}"
            );

            return false;
        }

        StartCoroutine(
            LoadSceneCoroutine(
                sceneName,
                LoadSceneMode.Single
            )
        );

        return true;
    }

    // =========================================================
    // TROCA FORÇADA
    // =========================================================

    public void ForceSceneChange(
        string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning(
                "GameManager: já está carregando uma cena."
            );

            return;
        }

        StartCoroutine(
            LoadSceneCoroutine(
                sceneName,
                LoadSceneMode.Single
            )
        );
    }

    // =========================================================
    // ALTERAR ESTADO
    // =========================================================

    public void SetState(
        GameState newState)
    {
        if (State == newState)
            return;

        State = newState;

        Debug.Log(
            $"GameManager: estado alterado para {State}"
        );

        OnStateChanged?.Invoke(State);
    }

    // =========================================================
    // CARREGAMENTO DE CENA
    // =========================================================

    private IEnumerator LoadSceneCoroutine(
        string sceneName,
        LoadSceneMode mode)
    {
        _isLoadingScene = true;

        Debug.Log(
            $"GameManager: carregando cena '{sceneName}' (mode={mode})..."
        );

        AsyncOperation op =
            SceneManager.LoadSceneAsync(
                sceneName,
                mode
            );

        if (op == null)
        {
            Debug.LogError(
                $"GameManager: cena '{sceneName}' não encontrada ou LoadSceneAsync retornou null."
            );

            _isLoadingScene = false;

            yield break;
        }

        while (!op.isDone)
            yield return null;

        yield return null;

        EnsureSingleAudioListener();

        try
        {
            AllocateInputToPlayer();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(
                $"GameManager: falha ao alocar inputs: {ex}"
            );
        }

        if (
            sceneName != null &&
            (
                sceneName.ToLowerInvariant().Contains("sample") ||
                sceneName.ToLowerInvariant().Contains("game")
            )
        )
        {
            yield return StartCoroutine(
                EnsureGuiLoaded()
            );

            SetState(
                GameState.Gameplay
            );
        }
        else
        {
            yield return StartCoroutine(
                EnsureGuiUnloaded()
            );
        }

        _isLoadingScene = false;

        Debug.Log(
            $"GameManager: cena '{sceneName}' carregada."
        );
    }

    // =========================================================
    // BOOT
    // =========================================================

    public void StartBootLoad(
        string targetSceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning(
                "GameManager: já está carregando uma cena."
            );

            return;
        }

        StartCoroutine(
            LoadSceneFromBootCoroutine(
                targetSceneName
            )
        );
    }

    private IEnumerator LoadSceneFromBootCoroutine(
        string targetSceneName)
    {
        _isLoadingScene = true;

        var currentScene =
            SceneManager.GetActiveScene();

        Debug.Log(
            $"GameManager: Boot sequence — carregando '{targetSceneName}' additivamente..."
        );

        var loadOp =
            SceneManager.LoadSceneAsync(
                targetSceneName,
                LoadSceneMode.Additive
            );

        if (loadOp == null)
        {
            Debug.LogError(
                $"GameManager: falha ao iniciar carregamento de '{targetSceneName}'."
            );

            _isLoadingScene = false;

            yield break;
        }

        while (!loadOp.isDone)
            yield return null;

        var loadedScene =
            SceneManager.GetSceneByName(
                targetSceneName
            );

        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(
                loadedScene
            );
        }

        EnsureSingleAudioListener();

        var unloadOp =
            SceneManager.UnloadSceneAsync(
                currentScene
            );

        if (unloadOp != null)
        {
            while (!unloadOp.isDone)
                yield return null;
        }

        yield return null;

        AllocateInputToPlayer();

        if (
            targetSceneName != null &&
            (
                targetSceneName.ToLowerInvariant().Contains("sample") ||
                targetSceneName.ToLowerInvariant().Contains("game")
            )
        )
        {
            yield return StartCoroutine(
                EnsureGuiLoaded()
            );

            SetState(
                GameState.Gameplay
            );
        }
        else if (
            targetSceneName != null &&
            targetSceneName.ToLowerInvariant().Contains("menu")
        )
        {
            yield return StartCoroutine(
                EnsureGuiUnloaded()
            );

            SetState(
                GameState.MenuPrincipal
            );
        }

        _isLoadingScene = false;

        Debug.Log(
            $"GameManager: Boot sequence complete. Loaded '{targetSceneName}'."
        );
    }

    // =========================================================
    // VERIFICAR CENA
    // =========================================================

    private bool IsSceneLoaded(
        string name)
    {
        for (
            int i = 0;
            i < SceneManager.sceneCount;
            ++i
        )
        {
            var sc =
                SceneManager.GetSceneAt(i);

            if (
                !sc.IsValid() ||
                !sc.isLoaded
            )
                continue;

            if (
                string.Equals(
                    sc.name,
                    name,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return true;
            }
        }

        return false;
    }

    // =========================================================
    // GUI
    // =========================================================

    private IEnumerator EnsureGuiLoaded()
    {
        const string guiName = "GUI";

        if (IsSceneLoaded(guiName))
            yield break;

        var op =
            SceneManager.LoadSceneAsync(
                guiName,
                LoadSceneMode.Additive
            );

        if (op == null)
        {
            Debug.LogWarning(
                "GameManager: GUI scene not found or not in Build Settings."
            );

            yield break;
        }

        while (!op.isDone)
            yield return null;

        yield return null;
    }

    private IEnumerator EnsureGuiUnloaded()
    {
        const string guiName = "GUI";

        if (!IsSceneLoaded(guiName))
            yield break;

        var sc =
            SceneManager.GetSceneByName(
                guiName
            );

        if (!sc.IsValid())
            yield break;

        var op =
            SceneManager.UnloadSceneAsync(sc);

        if (op == null)
            yield break;

        while (!op.isDone)
            yield return null;

        yield return null;
    }

    // =========================================================
    // INPUT SYSTEM
    // =========================================================

    private void AllocateInputToPlayer()
    {
#if ENABLE_INPUT_SYSTEM

        var devices =
            InputSystem.devices;

        if (devices.Count == 0)
        {
            Debug.Log(
                "GameManager: nenhum dispositivo do Input System detectado."
            );

            return;
        }

        var allPlayers =
            PlayerInput.all;

        PlayerInput player = null;

        if (allPlayers.Count > 0)
            player = allPlayers[0];

        if (player == null)
        {
            Debug.Log(
                "GameManager: nenhum PlayerInput encontrado na cena."
            );

            return;
        }

        var firstDevice =
            devices[0];

        try
        {
            var user =
                player.user;

            if (!user.valid)
            {
                user =
                    InputUser.PerformPairingWithDevice(
                        firstDevice
                    );
            }

            try
            {
                player.SwitchCurrentControlScheme(
                    player.currentControlScheme,
                    firstDevice
                );
            }
            catch (Exception)
            {
                InputUser.PerformPairingWithDevice(
                    firstDevice,
                    user
                );
            }

            Debug.Log(
                $"GameManager: device '{firstDevice}' alocado ao PlayerInput '{player.gameObject.name}'."
            );
        }
        catch (Exception ex)
        {
            Debug.LogWarning(
                $"GameManager: erro ao alocar dispositivo: {ex}"
            );
        }

#else

        Debug.Log(
            "GameManager: Input System não habilitado — pulando alocação de inputs."
        );

#endif
    }

    // =========================================================
    // VENCEDOR
    // =========================================================

    public void RegisterWinner(
        string playerName,
        string ballName)
    {
        MatchData.WinnerName =
            playerName;

        MatchData.WinnerBall =
            ballName;

        ForceSceneChange(
            "VictoryScene"
        );
    }

    // =========================================================
    // QUANDO UMA BOLINHA CAI
    // =========================================================

    public void PlayerLost(
        TwoBallController loser)
    {
        if (loser == null)
            return;

        // Se a partida já terminou,
        // ignora novas quedas.
        if (partidaFinalizada)
            return;

        int loserNumber = 0;

        // Identifica o jogador
        if (
            loser.gameObject.name ==
            "Jogador 1"
        )
        {
            loserNumber = 1;

            player1Quedas++;
        }
        else if (
            loser.gameObject.name ==
            "Jogador 2"
        )
        {
            loserNumber = 2;

            player2Quedas++;
        }
        else
        {
            Debug.LogWarning(
                "Não foi possível identificar o jogador que caiu: " +
                loser.gameObject.name
            );

            return;
        }

        Debug.Log(
            "Jogador " +
            loserNumber +
            " caiu! " +
            "P1: " +
            player1Quedas +
            " | P2: " +
            player2Quedas
        );

        // =====================================================
        // TERCEIRA QUEDA
        // =====================================================

        if (
            (
                loserNumber == 1 &&
                player1Quedas >= quedasParaVencer
            )
            ||
            (
                loserNumber == 2 &&
                player2Quedas >= quedasParaVencer
            )
        )
        {
            TwoBallController winner;

            if (loserNumber == 1)
            {
                winner =
                    FindPlayerByName(
                        "Jogador 2"
                    );
            }
            else
            {
                winner =
                    FindPlayerByName(
                        "Jogador 1"
                    );
            }

            if (winner != null)
            {
                SetWinner(winner);
            }

            return;
        }

        // =====================================================
        // PRIMEIRA OU SEGUNDA QUEDA
        // =====================================================

        RespawnPlayer(loser);
    }

    // =========================================================
    // ENCONTRAR JOGADOR
    // =========================================================

    private TwoBallController FindPlayerByName(
        string playerName)
    {
        TwoBallController[] players =
            FindObjectsByType<TwoBallController>(
                FindObjectsSortMode.None
            );

        foreach (
            var player in players
        )
        {
            if (
                player != null &&
                player.gameObject.name ==
                playerName
            )
            {
                return player;
            }
        }

        return null;
    }

    // =========================================================
    // RESPAWN
    // =========================================================

    private void RespawnPlayer(
        TwoBallController player)
    {
        if (player == null)
            return;

        Vector3 respawnPosition;

        if (
            player.gameObject.name ==
            "Jogador 1"
        )
        {
            respawnPosition =
                new Vector3(
                    -3f,
                    1f,
                    0f
                );
        }
        else
        {
            respawnPosition =
                new Vector3(
                    3f,
                    1f,
                    0f
                );
        }

        Rigidbody rb =
            player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;
        }

        player.transform.position =
            respawnPosition;

        Debug.Log(
            player.gameObject.name +
            " respawnou na arena."
        );
    }

    // =========================================================
    // DEFINIR VENCEDOR
    // =========================================================

    private void SetWinner(
        TwoBallController winner)
    {
        if (winner == null)
            return;

        partidaFinalizada = true;

        BolinhaController ball =
            winner.GetComponent<BolinhaController>();

        string ballName =
            "Desconhecida";

        if (
            ball != null &&
            ball.Data != null
        )
        {
            ballName =
                ball.Data.ballName;
        }

        MatchData.WinnerName =
            winner.gameObject.name;

        MatchData.WinnerBall =
            ballName;

        Debug.Log(
            "PARTIDA FINALIZADA! " +
            winner.gameObject.name +
            " venceu com a bolinha " +
            ballName
        );

        ForceSceneChange(
            "VictoryScene"
        );
    }
}