using UnityEngine;

[System.Serializable]
public class PlayerMovement
{
    private Transform playerTransform;
    private Transform m_cameraTransform;
    private Player m_owner; 

    [SerializeField] private float m_alignTurnRate;
    [SerializeField] private LayerMask m_walkable;
    [SerializeField] private CapsuleCollider m_capsuleCollider;
    [SerializeField] private CapsuleCollider m_walkableCollider;

    [SerializeField] private float m_groundAccel = 10f;
    [SerializeField] private float m_groundSpeedCap = 7f;
    [SerializeField] private float m_friction = 6f;
    [SerializeField] private float m_airAccel = 100f;
    [SerializeField] private float m_airCap = 0.7f;
    [SerializeField] private float m_gravity = 20f;
    [SerializeField] private float m_jumpForce = 7f;

    private Vector3 m_currentWalkableNormal = Vector3.zero;

    private Vector3 m_velocity = Vector3.zero;
    private Vector2 m_movementInputs = Vector2.zero;
    private Vector3 m_wishDirection = Vector3.zero;
    private bool m_isGrounded = false;
    private bool m_jumpQueued = false;

    private Vector3 m_verticalVelocity = Vector3.zero;
    private Vector3 m_horizontalVelocity = Vector3.zero;

    public void Start()
    {
    }

    public void Update()
    {
        if (m_currentWalkableNormal == Vector3.zero)
        {
            m_currentWalkableNormal = playerTransform.up;
        }
        BuildWishDirection();
        CheckGround();
        CheckWalkable();
        
        DecomposeVelocity();

        if (m_isGrounded)
        {
            ApplyFriction();
            Accelerate(m_groundSpeedCap, m_groundAccel);
            m_verticalVelocity = Vector3.zero;

            if (m_jumpQueued)
            {
                Jump();
            }
        }
        else
        {
            ApplyGravity();
            Accelerate(m_airCap, m_airAccel);
        }

        m_velocity = m_horizontalVelocity + m_verticalVelocity;
    }

    private void UpdateCameraData()
    {
        m_owner.GetCamera().lastPlayerPosition = m_owner.GetCamera().currentPlayerPosition;
        m_owner.GetCamera().currentPlayerPosition = playerTransform.position;
    }

    public void FixedUpdate()
    {

        Move();
        Depenetrate();
    }

    public void SetOwner(Player player)
    {
        m_owner = player;
    }

    public void SetPlayerTransform(Transform transform)
    {
        playerTransform = transform;
    }

    public void SetCameraTransform(Transform cameraTransform)
    {
        m_cameraTransform = cameraTransform;
    }

    public void OnMoveInputs(Vector2 input)
    {
        m_movementInputs = input;
    }

    public void OnJumpInput(bool jumpQueued)
    {
        m_jumpQueued = jumpQueued;
    }

    private void BuildWishDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(m_cameraTransform.forward, playerTransform.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(m_cameraTransform.right, playerTransform.up).normalized;
        m_wishDirection = (camForward * m_movementInputs.y + camRight * m_movementInputs.x).normalized;
    }

    private void CheckGround()
    {
        RaycastHit hit; // might need this later
        if (!Physics.Raycast(playerTransform.position, -m_currentWalkableNormal, out hit, 1.05f))
        {
            m_isGrounded = false;
            return;
        }
        m_isGrounded = true;
        UselessGameFeelMethodThatIWantedToImplementBecauseItsFun(hit);
    }

    private void UselessGameFeelMethodThatIWantedToImplementBecauseItsFun(RaycastHit hit)
    {
        if ((m_walkable.value & (1 << hit.transform.gameObject.layer)) == 0)
        {
            if (Vector3.Dot(hit.normal, playerTransform.up) > 0.98f)
            {
                m_currentWalkableNormal = hit.normal;
                AlignToSurface(hit.normal);
            }
        }
    }

    private void CheckWalkable()
    {
        Vector3[] points = GetCapsuleColliderPoints(m_walkableCollider);

        Collider[] overlaps = OverlapCapsule(points[0], points[1], m_walkableCollider.radius);

        Vector3 normal = Vector3.zero;
        float bestAlignment = float.MinValue;

        foreach (Collider col in overlaps)
        {
            if (Physics.ComputePenetration(
                m_walkableCollider,
                playerTransform.position,
                playerTransform.rotation,
                col,
                col.transform.position,
                col.transform.rotation,
                out Vector3 direction,
                out float distance))
            {
                float alignment = Vector3.Dot(direction, playerTransform.up);
                if (alignment > bestAlignment)
                {
                    if ((m_walkable.value & (1 << col.gameObject.layer)) == 0)
                    {
                        if (alignment < 0.8f)
                        {
                            continue;
                        }
                        normal = Vector3.zero;
                        break;
                    }
                    bestAlignment = alignment;
                    normal = direction;
                }
            }
        }

        if (normal != Vector3.zero)
        {
            m_currentWalkableNormal = normal;
        }

        if (playerTransform.up == m_currentWalkableNormal)
        {
            return;
        }

        AlignToSurface(m_currentWalkableNormal);
    }

    private void AlignToSurface(Vector3 surfaceNormal)
    {
        Quaternion desiredRotation = Quaternion.FromToRotation(playerTransform.up, surfaceNormal) * playerTransform.rotation;
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, desiredRotation, Time.deltaTime * m_alignTurnRate);

        SnapToNormal(surfaceNormal, 0.1f);
    }

    private void SnapToNormal(Vector3 normal, float snapThresholdDegree)
    {
        if (Vector3.Angle(playerTransform.up, normal) < snapThresholdDegree)
        {
            playerTransform.rotation = Quaternion.LookRotation(playerTransform.forward, normal);
        }
    }

    private Vector3[] GetPointsFromGeneralData(float height, float radius)
    {
        float innerHalfHeight = (height * 0.5f) - radius;
        Vector3 center = playerTransform.position;

        return new Vector3[]
        {
            center - playerTransform.up * innerHalfHeight,
            center + playerTransform.up * innerHalfHeight
        };
    }

    private Vector3[] GetCapsuleColliderPoints(CapsuleCollider capsuleCollider)
    {
        return GetPointsFromGeneralData(capsuleCollider.height, capsuleCollider.radius);
    }

    private void DecomposeVelocity()
    {
        m_verticalVelocity = playerTransform.up * Vector3.Dot(m_velocity, playerTransform.up);
        m_horizontalVelocity = m_velocity - m_verticalVelocity;
    }

    private void ApplyFriction()
    {
        float speed = m_horizontalVelocity.magnitude;
        if (speed < 0.01f)
        {
            m_horizontalVelocity = Vector3.zero;
            return;
        }
        float drop = speed * m_friction * Time.deltaTime;
        m_horizontalVelocity *= Mathf.Max(speed - drop, 0f) / speed;
    }

    private void ApplyGravity()
    {
        m_verticalVelocity -= playerTransform.up * m_gravity * Time.deltaTime;
    }

    private void Accelerate(float speedCap, float accel)
    {
        float currentSpeed = Vector3.Dot(m_horizontalVelocity, m_wishDirection);
        float addSpeed = Mathf.Clamp(speedCap - currentSpeed, 0f, accel * Time.deltaTime);
        m_horizontalVelocity += m_wishDirection * addSpeed;
    }

    private void Jump()
    {
        m_verticalVelocity = playerTransform.up * m_jumpForce;
    }

    private void ClipVelocity(Vector3 normal)
    {
        float dot = Vector3.Dot(m_velocity, normal);
        if (dot >= 0f)
        {
            return;
        }
        m_velocity -= normal * dot;
    }

    private Collider[] OverlapCapsule(Vector3 point1, Vector3 point2, float radius)
    {
        return Physics.OverlapCapsule(
            point1,
            point2,
            radius
        );
    }

    private void Depenetrate()
    {
        Vector3[] playerPoints = GetCapsuleColliderPoints(m_capsuleCollider);

        Collider[] overlaps = OverlapCapsule(playerPoints[0], playerPoints[1], m_capsuleCollider.radius);

        foreach (Collider col in overlaps)
        {
            if (col.gameObject.layer == 0)
            {
                continue;
            }
            if (Physics.ComputePenetration(
                    m_capsuleCollider,
                    playerTransform.position,
                    playerTransform.rotation,
                    col,
                    col.transform.position,
                    col.transform.rotation,
                    out Vector3 direction,
                    out float distance))
            {
                playerTransform.position += direction * distance;
                ClipVelocity(direction);
            }
        }
    }

    private void Move()
    {
        playerTransform.position += m_velocity * Time.fixedDeltaTime;
    }
}