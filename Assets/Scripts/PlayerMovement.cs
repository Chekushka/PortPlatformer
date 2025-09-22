using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Public variables to adjust in the Inspector
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 2f;
    
    public float acceleration = 15f; 
    public float deceleration = 20f;

    public float rotationMultiplier = 10f;
    [Range(-2,0)]
    public float characterFallMultiplier = -4f;
    
    public float gravity = -9.81f;
    
    public Camera mainCamera;

    // References to other components
    private CharacterController controller;
    private PlayerInputHandler inputHandler;

    // Internal state variables
    private float currentSpeed;
    private Vector3 velocity;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Get the PlayerInputHandler component on this same GameObject
        inputHandler = GetComponent<PlayerInputHandler>();
    }
    
    void Update()
    {
        // Handle gravity
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }
        
        // Read input from the handler
        Vector2 inputVector = inputHandler.MoveInput;
        bool isSprinting = inputHandler.IsSprinting;
        
        // Calculate the movement direction relative to the camera
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize(); 
        
        Vector3 cameraRight = mainCamera.transform.right;
        
        Vector3 moveDirection = cameraForward * inputVector.y + cameraRight * inputVector.x;
        
        // Determine the target speed based on input
        float targetSpeed = inputVector.magnitude > 0.1f ? moveSpeed : 0f;
        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
        }

        // Smoothly move towards the target speed
        float speedSmooth = (targetSpeed > currentSpeed) ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedSmooth * Time.deltaTime);

        // Handle jump
        if (inputHandler.JumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * characterFallMultiplier * gravity);
            inputHandler.ResetJump();
        }
        
        // Handle rotation and movement
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationMultiplier);

            controller.Move(transform.forward * (currentSpeed * Time.deltaTime));
        }
        
        // Apply gravity and final move
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}