using UnityEngine;

public class MapLocationConnector : MonoBehaviour
{
    public GameObject gameObject1; // Reference to the first GameObject
    public GameObject gameObject2; // Reference to the second GameObject

    public LineRenderer line; // Line Renderer

    void Start()
    {
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = 2;
    }

    void Update()
    {
        // Check if the GameObjects are not null
        if (gameObject1 != null && gameObject2 != null)
        {
            // Update position of the two vertex of the Line Renderer
            line.SetPosition(0, gameObject1.transform.position);
            line.SetPosition(1, gameObject2.transform.position);
        }
    }
}