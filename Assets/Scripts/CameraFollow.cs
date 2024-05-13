using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player's transform
    public Vector3 offset; // Offset from the player

    void LateUpdate()
    {
        if (target != null)
        {
            // Set the position of the camera to the player's position plus the offset
            transform.position = target.position + offset;
        }
        else
        {
            Debug.LogWarning("No target assigned to the camera follow script!");
        }
    }
}
