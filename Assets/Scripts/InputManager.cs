using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput inputActions;

    UnityEvent<Vector2> m_onCameraDelta = new UnityEvent<Vector2>();
    UnityEvent<Vector2> m_onMove = new UnityEvent<Vector2>();
    UnityEvent<bool> m_onJump = new UnityEvent<bool>();
    UnityEvent m_onAttack  = new UnityEvent();

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

        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;

        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Attack.canceled += OnAttack;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnCameraDelta(InputAction.CallbackContext context)
    {
        m_onCameraDelta.Invoke(context.ReadValue<Vector2>());
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        m_onMove.Invoke(context.ReadValue<Vector2>());  
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        float isJumping = context.ReadValue<float>();
        if (isJumping == 0)
        {
            m_onJump.Invoke(false);
            return;
        }
        m_onJump.Invoke(true);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() < 1) return;
        m_onAttack.Invoke();
    }

    public void BindOnCameraDelta(UnityAction<Vector2> action)
    {
        m_onCameraDelta.AddListener(action);
    }

    public void BindOnMove(UnityAction<Vector2> action)
    {
        m_onMove.AddListener(action);
    }

    public void BindOnJump(UnityAction<bool> action)
    {
        m_onJump.AddListener(action);
    }

    public void BindOnAttack(UnityAction action)
    {
        m_onAttack.AddListener(action);
    }
}
