using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    // Public variables to adjust in the Inspector
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float acceleration = 15f; 
    public float deceleration = 20f;
    [Range(0.1f, 10)] public float rotationMultiplier = 10f;
    public float gravity = -9.81f;
    
    [Header("Jump Settings")]
    public int maxJumps = 2;
    public float jumpHeight = 2f;
    public float jumpFlipBoost = 5f;
    [Range(-2,0)] public float characterFallMultiplier = -4f;
    
    [Header("Ground Check")]
    public Transform groundCheck; // An empty GameObject placed at the character's feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer; // Make sure your ground object has this layer
    
    public Camera mainCamera;

    // References to other components
    private CharacterController controller;
    private PlayerInputHandler inputHandler;
    private Animator animator;

    // Internal state variables
    private float currentSpeed;
    private Vector3 velocity;
    private int jumpCount = 0;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Get the PlayerInputHandler component on this same GameObject
        inputHandler = GetComponent<PlayerInputHandler>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Handle gravity
        bool isGrounded = IsCharacterGrounded();
        if (isGrounded && velocity.y < 0)
        {
            jumpCount = 0; // Reset jump count when grounded
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
        if (inputHandler.JumpPressed)
        {
            // First jump or subsequent jumps
            if (isGrounded || jumpCount < maxJumps)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (!isGrounded && inputHandler.IsSprinting)
                {
                    Vector3 forwardBoost = transform.forward * jumpFlipBoost;
                    controller.Move(forwardBoost * Time.deltaTime);
                    animator.SetTrigger("Flip");
                }
                else
                {
                    animator.SetTrigger("Jump");
                }
                
                jumpCount++;
                inputHandler.ResetJumpInput(); 
            }
        }
        
        // Handle rotation and movement
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationMultiplier);

            controller.Move(transform.forward * (currentSpeed * Time.deltaTime));
        }
    
        // Animate the character
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", velocity.y);
        animator.SetFloat("XInput", inputHandler.MoveInput.x);
        
        // Apply gravity and final move
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    private bool IsCharacterGrounded()
    {
        // Check for a collision with the ground layer using a sphere at the feet
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}