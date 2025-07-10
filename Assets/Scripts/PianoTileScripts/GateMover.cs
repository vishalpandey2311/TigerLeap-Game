using UnityEngine;

public class GateMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed of gate movement")]
    public float moveSpeed = -30f;
    
    [Header("Destruction")]
    [Tooltip("Z position at which the gate will be destroyed")]
    public float destructionZPosition = -7f;
    
    [Tooltip("Show debug messages when gate is destroyed")]
    public bool showDestructionDebug = false;

    void Start()
    {
        // Ensure the gate has the correct tag for identification
        if (!gameObject.CompareTag("DecorativeGate"))
        {
            gameObject.tag = "DecorativeGate";
        }
    }

    void Update()
    {
        // Move towards negative Z direction
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        
        // Check if gate has moved beyond the destruction point
        if (transform.position.z <= destructionZPosition)
        {
            if (showDestructionDebug)
            {
                Debug.Log($"GateMover: Destroying gate at position {transform.position}");
            }
            
            Destroy(gameObject);
        }
    }
}