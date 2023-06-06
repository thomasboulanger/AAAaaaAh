//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/

using UnityEngine;
using UnityEngine.Events;

//[System.Serializable] public class CustomGameEvent : UnityEvent<Component, object>{}
//[System.Serializable] public class CustomGameEvent : UnityEvent<Component, object, object>{}
[System.Serializable] public class CustomGameEvent : UnityEvent<Component, object, object, object>{}

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public CustomGameEvent response;
    private void OnEnable() => gameEvent.RegisterListener(this);
    private void OnDisable() => gameEvent.UnregisterListener(this);

    public void OnEventRaised(Component sender, object data1,object data2 ,object data3) => response.Invoke(sender,data1, data2, data3);
}