using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SOEvents;

public class MouseInfo : MonoBehaviour
{
    [field: SerializeField] public IntSOEvent MouseDownEvent {get; private set;}
    [field: SerializeField] public IntSOEvent MouseUpEvent {get; private set;}
    [field: SerializeField] public IntSOEvent ScrollEvent {get; private set;}

    [field: SerializeField] public bool LeftMouseHeldDown {get; private set;}
    [field: SerializeField] public float TimeLMButtonDown {get; private set;}
    [field: SerializeField] public bool RightMouseHeldDown {get; private set;}
    [field: SerializeField] public float TimeRMButtonDown {get; private set;}

    [field: SerializeField] public Vector2 MousePosScreen {get; private set;}
    [field: SerializeField] public Vector2 MousePosWorld {get; private set;}
    [field: SerializeField] public Vector2 DeltaMousePosScreen {get; private set;}
    
    [field: SerializeField] public bool MouseOverUI {get; private set;}


    private Vector2 mousePosTMinus1;


    public void OnLMB(InputValue value) {
        if (value.Get<float>() == 1) 
        {
            MouseDownEvent.Invoke(0);
            LeftMouseHeldDown = true;
        }
        if (value.Get<float>() == 0) 
        {
            MouseUpEvent.Invoke(0);
            LeftMouseHeldDown = false;
        }
    }

    public void OnRMB(InputValue value) {
        if (value.Get<float>() == 1) 
        {
            MouseDownEvent.Invoke(2);
            RightMouseHeldDown = true;
        }
        if (value.Get<float>() == 0) 
        {
            MouseUpEvent.Invoke(2);
            RightMouseHeldDown = false;
        }
    }

    public void OnMouseScroll(InputValue value) {
        switch (value.Get<Vector2>().y)
        {
            case > 0: ScrollEvent.Invoke(1); break;
            case < 0: ScrollEvent.Invoke(-1); break;
            default: ScrollEvent.Invoke(0); break;
        }
    }

    public void OnMousePosition(InputValue value) {
        MousePosScreen = value.Get<Vector2>();
        MousePosWorld =  Camera.main.ScreenToWorldPoint(MousePosScreen);
    }

    private void Update() {
        MouseOverUI = EventSystem.current.IsPointerOverGameObject();
        MousePosWorld =  Camera.main.ScreenToWorldPoint(MousePosScreen);

        if (LeftMouseHeldDown) TimeLMButtonDown += Time.deltaTime;
        else TimeLMButtonDown = 0;

        if (RightMouseHeldDown) TimeRMButtonDown += Time.deltaTime;
        else TimeRMButtonDown = 0;

        DeltaMousePosScreen = MousePosScreen - mousePosTMinus1;
        mousePosTMinus1 = MousePosScreen;

    }
}
