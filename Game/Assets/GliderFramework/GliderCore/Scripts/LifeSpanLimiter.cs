using UnityEngine;
using SOEvents;

public class LifeSpanLimiter : MonoBehaviour
{
    // This should usually be despawnEvent that returns the object to the Object Pool.
    [SerializeField] public GameObjectFloatSOEvent onLifeEndEvent;
    [SerializeField] bool autoStart = true;
    public float startLifespan;
    public float spanRemaining;

    [SerializeField] float despawnFloatValue = 0;

    public bool Running {get; private set;}

    private void Start() 
    {
        if (autoStart) StartCountdown(startLifespan);   
    }

    // Use this method to activate and make this class actually do something.
    public void StartCountdown(float startLifespanValue)
    {
        startLifespan = startLifespanValue;
        spanRemaining = startLifespan;
        Running = true;
    }

    private void FixedUpdate() 
    {
        if (!Running) return;

        spanRemaining -= Time.fixedDeltaTime;

        if (spanRemaining <= 0) 
        {
            Running = false;
            onLifeEndEvent.Invoke(gameObject, despawnFloatValue);
        }
    }

    private void OnDisable() => Running = false;
    private void OnEnable() {
        if (autoStart) StartCountdown(startLifespan);
    }

}
