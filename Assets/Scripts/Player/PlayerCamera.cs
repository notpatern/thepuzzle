using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [HideInInspector] public Vector3 lastPlayerPosition = Vector3.zero;
    [HideInInspector] public Vector3 currentPlayerPosition = Vector3.zero;
    
    [SerializeField] private Transform m_cameraHolder;
    [SerializeField] private Transform m_orientation;
    [SerializeField] private Transform m_player;

    [SerializeField] private float m_pitchMin = -89f;
    [SerializeField] private float m_pitchMax = 89f;

    private Vector3 m_cameraForward;
    private float m_currentPitch;
    private Quaternion m_lastPlayerRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_cameraForward = m_cameraHolder.forward;
        m_currentPitch = 0f;
        m_lastPlayerRotation = m_player.rotation;
    }

    public void UpdateCameraAngles(Vector2 delta)
    {
        //float framePosition = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        //m_cameraHolder.position = Vector3.Lerp(lastPlayerPosition, currentPlayerPosition, framePosition);
        
        if (m_player.rotation != m_lastPlayerRotation)
        {
            RebaseOntoNewUp();
            m_lastPlayerRotation = m_player.rotation;
        }

        float yawDelta = delta.x * 50f * Time.deltaTime;
        float pitchDelta = delta.y * 50f * Time.deltaTime;

        m_cameraForward = Quaternion.AngleAxis(yawDelta, m_player.up) * m_cameraForward;

        float newPitch = Mathf.Clamp(m_currentPitch + pitchDelta, m_pitchMin, m_pitchMax);
        float appliedDelta = newPitch - m_currentPitch;
        m_currentPitch = newPitch;

        Vector3 cameraRight = Vector3.Cross(m_cameraForward, m_player.up).normalized;
        m_cameraForward = Quaternion.AngleAxis(appliedDelta, cameraRight) * m_cameraForward;
        m_cameraForward = m_cameraForward.normalized;

        m_cameraHolder.rotation = Quaternion.LookRotation(m_cameraForward, m_player.up);

        Vector3 flatForward = Vector3.ProjectOnPlane(m_cameraForward, m_player.up).normalized;
        m_orientation.rotation = Quaternion.LookRotation(flatForward, m_player.up);
    }

    private void RebaseOntoNewUp()
    {
        Quaternion playerDelta = m_player.rotation * Quaternion.Inverse(m_lastPlayerRotation);

        m_cameraForward = playerDelta * m_cameraForward;
        m_cameraForward = m_cameraForward.normalized;

        Vector3 flatForward = Vector3.ProjectOnPlane(m_cameraForward, m_player.up).normalized;
        m_currentPitch = Vector3.SignedAngle(flatForward, m_cameraForward, Vector3.Cross(flatForward, m_player.up));
        m_currentPitch = Mathf.Clamp(m_currentPitch, m_pitchMin, m_pitchMax);
    }
}