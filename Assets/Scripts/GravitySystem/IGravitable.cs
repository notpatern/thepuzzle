using UnityEngine;

public interface IGravitable
{
    public void ApplyGravity();
    public void ChangeGravityDirection(Vector3 gravityDirection);

}