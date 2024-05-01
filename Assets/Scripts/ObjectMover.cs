using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public Transform startPoint; // The starting point
    public Transform endPoint;   // The ending point
    public float speed = 2.0f;   // The speed of movement

    private float startTime;      // Time when the movement started
    private float journeyLength;  // Total distance between the start and end points

    void Start()
    {
        // Calculate the journey length between the start and end points
        journeyLength = Vector3.Distance(startPoint.position, endPoint.position);

        // Start moving the object
        StartMoving();
    }

    void Update()
    {
        // Calculate the time since the movement started
        float distCovered = (Time.time - startTime) * speed;

        // Calculate the fraction of the journey completed
        float fracJourney = distCovered / journeyLength;

        // Move the object using Lerp
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, fracJourney);

        // Calculate the direction of movement
        Vector3 direction = (endPoint.position - startPoint.position).normalized;

        // Rotate the object to face the direction of movement
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // If the object reaches the end point, restart the movement
        if (fracJourney >= 1.0f)
        {
            StartMoving();
        }
    }

    void StartMoving()
    {
        // Set the start time to the current time when movement begins
        startTime = Time.time;

        // Swap the start and end points for the next movement
        Transform temp = startPoint;
        startPoint = endPoint;
        endPoint = temp;
    }
}
