using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_cameraObject;
    private PlayerCamera m_camera;

    [SerializeField] private PlayerMovement m_playerMovement;
    [SerializeField] private Interact m_interact;

    void Awake()
    {
        m_camera = m_cameraObject.GetComponent<PlayerCamera>();
    }   

    private void Start()
    {
        m_playerMovement.Start();
        m_playerMovement.SetPlayerTransform(transform);
        m_playerMovement.SetCameraTransform(m_cameraObject);
        m_playerMovement.SetOwner(this);
        m_interact.Start(m_cameraObject);
    }

    public PlayerCamera GetCamera()
    {
        return m_camera;
    }

    public PlayerMovement GetPlayerMovement()
    {
        return m_playerMovement;
    }

    public Interact GetPlayerInteract()
    {
        return m_interact;
    }

    public void OnDrawGizmos()
    {
        m_interact.OnDrawGizmos();
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