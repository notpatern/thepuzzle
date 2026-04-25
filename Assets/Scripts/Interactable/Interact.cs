using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Interact
{
    [SerializeField] float m_interactDistance;
    private Transform m_transform;
    private bool m_isHolding = false;
    private GameObject m_heldObject;

    public void Start(Transform transform)
    {
        m_transform = transform;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.forward * m_interactDistance);
    }

    public void OnAttackInput()
    {
        if (!m_isHolding)
        {
            Physics.Raycast(m_transform.position, m_transform.forward, out RaycastHit hit, m_interactDistance);
            if (hit.transform)
            {
                IInteractable interactable = hit.transform.parent.GetComponentInChildren<IInteractable>();
                if (interactable != null)
                {
                    m_heldObject = hit.transform.transform.parent.gameObject;
                    interactable.Interact(m_transform.position, m_transform);
                    m_isHolding = true;
                }
            }
        }
        else
        {
            m_isHolding = false;
            m_heldObject.transform.SetParent(m_heldObject.transform);
            m_heldObject = null;
        }
    }
}