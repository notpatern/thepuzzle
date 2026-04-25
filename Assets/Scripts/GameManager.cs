using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager m_inputManager;
    [SerializeField] private Player m_player;

    void Start()
    {
        BindInputEvents();
    } 

    void BindInputEvents()
    {
        m_inputManager.BindOnCameraDelta(m_player.GetCamera().UpdateCameraAngles);
        m_inputManager.BindOnMove(m_player.GetPlayerMovement().OnMoveInputs);
        m_inputManager.BindOnJump(m_player.GetPlayerMovement().OnJumpInput);
        m_inputManager.BindOnAttack(m_player.GetPlayerInteract().OnAttackInput);
    }   
}
