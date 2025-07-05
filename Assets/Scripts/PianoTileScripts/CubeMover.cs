using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private float moveSpeed = -30f; // Speed of cube, set it in Inspector
    
    [Header("Destruction")]
    [Tooltip("Z position at which the cube will be destroyed")]
    public float destructionZPosition = -7f;
    
    [Tooltip("Show debug messages when cube is destroyed")]
    public bool showDestructionDebug = false;

    void Start()
    {
        // Ensure the moving cube has the correct tag
        if (!gameObject.CompareTag("MovingCube"))
        {
            gameObject.tag = "MovingCube";
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        
        // Check if cube has moved beyond the destruction point
        if (transform.position.z <= destructionZPosition)
        {
            if (showDestructionDebug)
            {
                Debug.Log($"CubeMover: Destroying cube at position {transform.position}");
            }
            
            // Notify score manager that cube was missed
            if (PTScoreManager.Instance != null)
            {
                PTScoreManager.Instance.OnCubeMissed();
            }
            
            Destroy(gameObject);
        }
    }
}
