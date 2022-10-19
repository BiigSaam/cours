using UnityEngine;
using UnityEngine.Events;

// https://github.com/UnityTechnologies/open-project-1/blob/devlogs/2-scriptable-objects/UOP1_Project/Assets/Scripts/Events/IntEventListener.cs

public class GameEventListener : MonoBehaviour
{
    // Event to register
    public GameEvent Event;

    // Function to call when the Event is invoked
    public UnityEvent Response;

    private void OnEnable()
    { Event.RegisterListener(this); }

    private void OnDisable()
    { Event.UnregisterListener(this); }

    public void OnEventRaised()
    { Response.Invoke(); }
}