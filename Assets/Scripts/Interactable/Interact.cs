using UnityEngine;

[System.Serializable]
public class Interact
{
    [SerializeField] float m_interactDistance;
    private Transform m_transform; 
    
    private Interact(Transform transform)    
    {
        m_transform = transform; 
    }
    
    public void Update()
    {
        Physics.Raycast(m_transform.position, m_transform.forward, out RaycastHit hit, m_interactDistance);
        if (hit.collider)
        {
            IInteractable interactable = (IInteractable)hit.collider.GetComponent<IInteractable>();
            interactable?.Interact(m_transform.position, m_transform);
        }
    }
}