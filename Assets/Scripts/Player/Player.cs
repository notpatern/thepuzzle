using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_cameraObject;
    private PlayerCamera m_camera;

    private PlayerMovement m_playerMovement;

    void Awake()
    {
        m_camera = m_cameraObject.GetComponent<PlayerCamera>();
    }

    private void Start()
    {
        m_playerMovement.Start();
    }

    public PlayerCamera GetCamera()
    {
        return m_camera;
    }

    private void Update()
    {
        m_playerMovement.Update();
    }

    private void FixedUpdate()
    {
        m_playerMovement.FixedUpdate();
    }
}

