using UnityEngine;

public class Gravity
{
    private Gravity() {}
    
    private static Gravity s_instance;

    public static Gravity Instance()
    {
        if (s_instance == null)
        {
            s_instance = new Gravity(); 
        }
        
        return s_instance;  
    }

    private float m_gravity = 10;
    public float GetGravity()
    {
        return m_gravity;
    }
    
    public void SetGravity(float gravity)
    {
        m_gravity = gravity;
    }
    
    private Vector3 m_gravityDirection = Vector3.down;
    public Vector3 GetGravityDirection()
    {
        return m_gravityDirection;
    }

    public void SetGravityDirection(Vector3 gravityDirection)
    {
        m_gravityDirection = gravityDirection;   
    }
}