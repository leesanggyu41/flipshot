using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float walkSpeed = 4f; // Speed of the player
    public float maxVelocityChange = 10f; // Maximum change in velocity per frame

    private Vector2 inputVector;
    private Rigidbody rb; // Reference to the Rigidbody component
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the GameObject
    }

    // Update is called once per frame
    void Update()
    {
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // Get input from the player
        inputVector.Normalize(); // Normalize the input vector to ensure consistent movement spee
    }
    void FixedUpdate()
    {
        Vector3 velocityChange = CalculateMovement(walkSpeed); // Calculate the change in velocity based on input
        rb.AddForce(velocityChange, ForceMode.VelocityChange); // Apply the calculated force to the Rigidbody
    }
    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(inputVector.x, 0, inputVector.y); // Create a target velocity vector based on input
        targetVelocity = transform.TransformDirection(targetVelocity); // Transform the target velocity to world space
        targetVelocity *= _speed; // Scale the target velocity by the walk speed

        Vector3 velocity = rb.linearVelocity; // Get the current velocity of the Rigidbody
        if (inputVector.magnitude > 0.5f) // If the change in velocity is greater than the maximum allowed
        {
            Vector3 velocityChange = targetVelocity - velocity; // Calculate the change in velocity
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocityChange); //= Limit the change in velocity to the maximum allowed
            return velocityChange;
        }
        return Vector3.zero;
    }
}
