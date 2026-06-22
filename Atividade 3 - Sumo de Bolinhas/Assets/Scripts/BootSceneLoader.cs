using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

public class BootSceneLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Load the BootSceneTarget from Resources
        var target = Resources.Load<BootSceneTarget>("BootSceneTarget");
        string targetName = "Splash";
        if (target != null && !string.IsNullOrEmpty(target.TargetSceneName))
            targetName = target.TargetSceneName;
        else
            Debug.LogWarning("BootSceneLoader: BootSceneTarget missing or empty; defaulting target to 'Splash'.");

        // If the target is the Boot scene itself, do nothing
        var currentScene = SceneManager.GetActiveScene();
        if (string.Equals(currentScene.name, targetName, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("BootSceneLoader: Target scene is the Boot scene itself. Nothing to load.");
            yield break;
        }

        // Try to find a manager object that centralizes SceneManager access.
        // We'll look for any MonoBehaviour whose type name is 'GameManager' or 'GameManagerCore'
        float timeout = 1.0f;
        float t = 0f;
        bool delegated = false;
        while (t < timeout)
        {
            var mbs = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in mbs)
            {
                if (mb == null) continue;
                var typeName = mb.GetType().Name;
                if (string.Equals(typeName, "GameManager", StringComparison.Ordinal) || string.Equals(typeName, "GameManagerCore", StringComparison.Ordinal))
                {
                    // Try to find a StartBootLoad method
                    var mi = mb.GetType().GetMethod("StartBootLoad", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (mi != null)
                    {
                        try
                        {
                            mi.Invoke(mb, new object[] { targetName });
                            Debug.Log($"BootSceneLoader: Delegated boot loading to '{typeName}'.");
                            delegated = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"BootSceneLoader: Failed invoking StartBootLoad on {typeName}: {ex}");
                        }
                    }
                }
            }
            if (delegated) break;
            yield return null;
            t += Time.deltaTime;
        }

        if (delegated)
        {
            yield break; // manager will perform loading/unloading
        }

        // Fallback: perform additive load ourselves and unload the Boot scene
        Debug.Log($"BootSceneLoader: No GameManager found; fallback loading '{targetName}' additively.");
        var loadOp = SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
        if (loadOp == null)
        {
            Debug.LogError($"BootSceneLoader: Failed to start loading scene '{targetName}'. Ensure it is added to Build Settings and the name is correct.");
            yield break;
        }
        while (!loadOp.isDone) yield return null;

        var loadedScene = SceneManager.GetSceneByName(targetName);
        if (loadedScene.IsValid()) SceneManager.SetActiveScene(loadedScene);

        var unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        if (unloadOp == null)
        {
            Debug.LogWarning("BootSceneLoader: Failed to unload Boot scene.");
            yield break;
        }
        while (!unloadOp.isDone) yield return null;

        Debug.Log($"BootSceneLoader: Loaded '{targetName}' and unloaded Boot scene.");
    }
}
