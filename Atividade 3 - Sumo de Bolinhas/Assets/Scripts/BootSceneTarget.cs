using UnityEngine;

[CreateAssetMenu(menuName = "Bootstrap/BootSceneTarget")]
public class BootSceneTarget : ScriptableObject
{
    // Full asset path (e.g. "Assets/Scenes/Level1.unity"). Written by the editor before entering Play.
    public string TargetScenePath;

    // Scene name (filename without extension). Used at runtime to load the scene by name.
    [Tooltip("Scene name (filename without extension) that the Boot scene will redirect to. Defaults to 'Splash'.")]
    public string TargetSceneName = "Splash";
}


