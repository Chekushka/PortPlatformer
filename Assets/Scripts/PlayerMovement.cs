using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour , IPausable
{
    [Header("Movement Settings")]
    // Public variables to adjust in the Inspector
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float acceleration = 15f; 
    [SerializeField] private float deceleration = 20f;
    [Range(0.1f, 10)] [SerializeField] private float rotationMultiplier = 10f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpFlipBoost = 5f;
    [Range(-2,0)] [SerializeField] private float characterFallMultiplier = -4f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // An empty GameObject placed at the character's feet
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer; // Make sure your ground object has this layer
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    // References to other components
    private CharacterController m_Controller;
    private PlayerInputHandler m_InputHandler;
    private Animator m_Animator;

    // Internal state variables
    private float m_CurrentSpeed;
    private Vector3 m_Velocity;
    private int m_JumpCount = 0;
    private bool m_IsPaused = false;
    
    private void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<PlayerInputHandler>();
        m_Animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (m_IsPaused)
        {
            return; // Stop executing the rest of the Update method
        }
        
        // Handle gravity
        bool isGrounded = IsCharacterGrounded();
        if (isGrounded && m_Velocity.y < 0)
        {
            m_JumpCount = 0; // Reset jump count when grounded
            m_Velocity.y = -2f; 
        }
        
        // Read input from the handler
        Vector2 inputVector = m_InputHandler.MoveInput;
        bool isSprinting = m_InputHandler.IsSprinting;
        
        // Calculate the movement direction relative to the camera
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        Vector3 moveDirection = cameraForward * inputVector.y + cameraRight * inputVector.x;
        
        // Determine the target speed based on input
        float targetSpeed = inputVector.magnitude > 0.1f ? moveSpeed : 0f;
        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
        }

        // Smoothly move towards the target speed
        float speedSmooth = (targetSpeed > m_CurrentSpeed) ? acceleration : deceleration;
        m_CurrentSpeed = Mathf.MoveTowards(m_CurrentSpeed, targetSpeed, speedSmooth * Time.deltaTime);

        // Handle jump
        if (m_InputHandler.JumpPressed)
        {
            // First jump or subsequent jumps
            if (isGrounded || m_JumpCount < maxJumps)
            {
                m_Velocity.y = Mathf.Sqrt(jumpHeight * characterFallMultiplier * gravity);

                if (!isGrounded && m_InputHandler.IsSprinting)
                {
                    Vector3 forwardBoost = transform.forward * jumpFlipBoost;
                    m_Controller.Move(forwardBoost * Time.deltaTime);
                    m_Animator.SetTrigger("Flip");
                }
                else
                {
                    m_Animator.SetTrigger("Jump");
                }
                
                m_JumpCount++;
                m_InputHandler.ResetJumpInput(); 
            }
        }
        
        // Handle rotation and movement
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationMultiplier);

            m_Controller.Move(transform.forward * (m_CurrentSpeed * Time.deltaTime));
        }
    
        // Animate the character
        m_Animator.SetFloat("Speed", m_CurrentSpeed);
        m_Animator.SetBool("IsGrounded", isGrounded);
        m_Animator.SetFloat("YVelocity", m_Velocity.y);
        m_Animator.SetFloat("XInput", m_InputHandler.MoveInput.x);
        
        // Apply gravity and final move
        m_Velocity.y += gravity * Time.deltaTime;
        m_Controller.Move(m_Velocity * Time.deltaTime);
    }
    
    private void OnEnable()
    {
        PauseManager.I.Register(this);
    }

    // Unregister when this object is disabled
    private void OnDisable()
    {
        PauseManager.I.Unregister(this);
    }
    
    public void Pause() => m_IsPaused = true;
    public void Resume() => m_IsPaused = false;
    
    private bool IsCharacterGrounded()
    {
        // Check for a collision with the ground layer using a sphere at the feet
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}