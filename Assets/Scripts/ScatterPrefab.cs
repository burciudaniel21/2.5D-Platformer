using UnityEngine;

public class ScatterPrefab : MonoBehaviour
{
    public GameObject prefabToScatter; // Assign the prefab you want to scatter in the Unity Editor
    public int numberOfPrefabs = 10; // Number of prefabs to scatter
    public float minScale = 0.5f; // Minimum scale for the prefabs
    public float maxScale = 2.0f; // Maximum scale for the prefabs
    public float yScale = 0.5f;
    public bool spawnVertically = true; // Choose whether to spawn vertically or horizontally
    public Vector3 offset = Vector3.zero; // Offset to apply to the prefabs' position
    public float yOffsetAboveParent = 1.0f; // Offset to ensure objects are spawned above the parent

    void Start()
    {
        ScatterPrefabs();
    }

    void ScatterPrefabs()
    {
        if (prefabToScatter == null)
        {
            Debug.LogError("Prefab to scatter is not assigned!");
            return;
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;

        Vector3 parentBoundsMin = transform.TransformPoint(bounds.min);
        Vector3 parentBoundsMax = transform.TransformPoint(bounds.max);

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            // Generate a random point within the bounds of the parent object
            Vector3 randomPoint = new Vector3(
                Random.Range(parentBoundsMin.x, parentBoundsMax.x),
                Random.Range(parentBoundsMin.y, parentBoundsMax.y),
                Random.Range(parentBoundsMin.z, parentBoundsMax.z)
            );

            // Add yOffsetAboveParent to ensure objects are spawned above the parent
            randomPoint.y += yOffsetAboveParent;

            // Convert the random point to local space
            Vector3 localRandomPoint = transform.InverseTransformPoint(randomPoint);

            // Randomly scale the prefab within the defined bounds
            float randomScale = Random.Range(minScale, maxScale);
            Vector3 scale = new Vector3(randomScale, yScale, randomScale);

            // Instantiate the prefab at the calculated position with random scale
            GameObject newPrefab = Instantiate(prefabToScatter, randomPoint, Quaternion.identity);

            // Set the parent of the instantiated prefab to this object
            newPrefab.transform.parent = transform;

            // Ensure the spawned prefab stays within the bounds of the parent
            Vector3 clampedPosition = newPrefab.transform.localPosition;

            clampedPosition.x = Mathf.Clamp(clampedPosition.x, bounds.min.x, bounds.max.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, bounds.min.y, bounds.max.y);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, bounds.min.z, bounds.max.z);

            newPrefab.transform.localPosition = clampedPosition;

            // Set the local scale of the prefab
            newPrefab.transform.localScale = scale;
        }
    }
}
