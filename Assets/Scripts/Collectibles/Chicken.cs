using UnityEngine;

public class Chicken : MonoBehaviour, IGravitable
{
    [SerializeField] private Collider m_collider;
    private bool m_isInEnclosure = false;
    [SerializeField] private Vector3 m_gravityDirection;
    
    void Start()
    {
        m_collider = GetComponent<Collider>(); 
    }

    public void ApplyGravity()
    {
         
    }

    public void ChangeGravityDirection(Vector3 normal)
    {
        Gravity.Instance().SetGravityDirection(normal);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Enclosure")
        {
            if (!m_isInEnclosure)
            {
                m_isInEnclosure = true;    
            }
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            ChangeGravityDirection(-collision.gameObject.transform.up);
        }
    }

    void Update()
    {
        
    }
}
