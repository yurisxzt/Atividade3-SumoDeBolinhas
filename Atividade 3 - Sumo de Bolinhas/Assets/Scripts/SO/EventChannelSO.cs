using UnityEngine;

public abstract class EventChannelBase : ScriptableObject { }

[CreateAssetMenu(menuName = "EventChannels/CoinCollected")]
public class CoinEventChannel : EventChannelBase
{
    public System.Action<string> listeners;
    public void Raise(string id) { listeners?.Invoke(id); }
}

[CreateAssetMenu(menuName = "EventChannels/CheckpointReached")]
public class VoidEventChannel : EventChannelBase
{
    public System.Action listeners;
    public void Raise() { listeners?.Invoke(); }
}
