using UnityEngine;

[System.Serializable]
public class PlayerMovement
{
    private Transform playerTransform;
    [SerializeField] private float m_alignTurnRate;
    [SerializeField] private LayerMask m_walkable;
    [SerializeField] private LayerMask m_ground;

    private Vector3 m_velocity = new Vector3();
    private Vector2 m_movementInputs = new Vector2();
    private Vector3 m_wishDirection = new Vector3();
    private bool m_isGrounded = false;

    private float m_movementSpeed = 0f;
    private float m_airMoveSpeed = 0f;
    private float m_airCap = 0f;

    public void Start()
    {

    }

    public void Update()
    {
        BuildWishDirection();
        CheckGround();
        CheckWalkable();
    }

    public void FixedUpdate()
    {

    }

    private void AlignToWalkableSurface(Vector3 surfaceNormal)
    {
        Quaternion desiredRotation = Quaternion.FromToRotation(playerTransform.up, surfaceNormal);
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, desiredRotation, Time.deltaTime * m_alignTurnRate);
    }

    public void SetPlayerTransform(Transform transform)
    {
        playerTransform = transform;
    }

    public void OnMoveInputs(Vector2 input)
    {
        m_movementInputs = input;
    }

    private void BuildWishDirection()
    {
        m_wishDirection = new Vector3(m_movementInputs.x, 0f, m_movementInputs.y);
        m_wishDirection = Vector3.ProjectOnPlane(m_wishDirection, playerTransform.up).normalized;
    }

    private void CheckGround()
    {
        RaycastHit hit;
        if (!Physics.SphereCast(playerTransform.position, 0.5f, -playerTransform.up, out hit, 1.1f, m_ground))
        {
            m_isGrounded = false;
            return;
        }
        m_isGrounded = true;
    }

    private void CheckWalkable()
    {
        Vector3[] capsulePoints = GetCapsulePoints();
        RaycastHit[] hits = Physics.CapsuleCastAll(capsulePoints[0], capsulePoints[1], 1f, playerTransform.up, 0f, m_walkable);

        if (hits.Length == 0) 
        {
            return;
        }

        float distance;
        float oldDistance = 100f;
        Vector3 walkableNormal;

        foreach (RaycastHit hit in hits)
        {
            distance = hit.distance;
            if (distance < oldDistance)
            {
                walkableNormal = hit.normal;
            }
            oldDistance = distance;
        }
    }
    
    private Vector3[] GetCapsulePoints()
    {
        Vector3[] points = { new Vector3(), new Vector3() };

        points[0] = playerTransform.position + playerTransform.position + playerTransform.up * -1f;
        points[1] = points[0] + playerTransform.up * 2f;
            
        return points;
    }

    private void DecomposeVelocity()
    {

    }

    private void ApplyFriction()
    {

    }

    private void ApplyGravity()
    {

    }

    private void Accelerate()
    {

    }

    private void ClipVelocity()
    {

    }

    private void Depentrate()
    {

    }

    private void Move()
    {
            
    }
}
