using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput inputActions;

    UnityEvent<Vector2> m_onCameraDelta = new UnityEvent<Vector2>();
    UnityEvent<Vector2> m_onMove = new UnityEvent<Vector2>();

    private void Awake()
    {
        inputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Look.performed += OnCameraDelta;  
        inputActions.Player.Look.canceled += OnCameraDelta;  

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnCameraDelta(InputAction.CallbackContext context)
    {
        m_onCameraDelta.Invoke(context.ReadValue<Vector2>());
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        m_onMove.Invoke(context.ReadValue<Vector2>());  
    }

    public void BindOnCameraDelta(UnityAction<Vector2> action)
    {
        m_onCameraDelta.AddListener(action);
    }

    public void BindOnMove(UnityAction<Vector2> action)
    {
        m_onMove.AddListener(action);
    }
}
