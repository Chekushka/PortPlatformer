using Pause;
using Player.States;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IPausable
    {
        #region Settings

        [Header("Movement Settings")]
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
        [Range(-2, 0)] [SerializeField] private float characterFallMultiplier = -4f;

        [Header("Sit Settings")]
        [SerializeField] private float sitHeight = 0.5f;
    
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;
    
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Debug Info (Read-Only)")]
        [SerializeField] private bool debugIsGrounded;
        [SerializeField] private int debugJumpCount;
        [SerializeField] private float debugVerticalVelocity;
        [SerializeField] private bool debugIsSitting;
        [SerializeField] private string debugCurrentState;

        #endregion

        #region Components

        private CharacterController m_Controller;
        private PlayerInputHandler m_InputHandler;
        private Animator m_Animator;

        #endregion

        #region State Machine

        private PlayerState m_CurrentState;
        private PlayerState m_PreviousState;

        #endregion

        #region Movement State

        private Vector3 m_Velocity;
        private int m_JumpCount = 0;
        private bool m_IsPaused = false;
        private bool m_IsSitting = false;

        #endregion

        #region Public Properties

        public float MoveSpeed => moveSpeed;
        public float SprintSpeed => sprintSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float RotationMultiplier => rotationMultiplier;
        public float Gravity => gravity;
        public float JumpHeight => jumpHeight;
        public int MaxJumps => maxJumps;
        public float JumpFlipBoost => jumpFlipBoost;
        public float CharacterFallMultiplier => characterFallMultiplier;

        public PlayerInputHandler InputHandler => m_InputHandler;
        public Vector3 Velocity => m_Velocity;
        public bool IsGrounded => IsCharacterGrounded();
        public bool IsSitting => m_IsSitting;
        public bool CanJump => m_JumpCount < maxJumps;

        #endregion
    
        private void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            m_InputHandler = GetComponent<PlayerInputHandler>();
            m_Animator = GetComponent<Animator>();

            // Initialize state machine with Idle state
            m_CurrentState = new IdleState(this, m_Animator);
            m_CurrentState.OnEnter();
        }
    
        private void Update()
        {
            if (m_IsPaused)
                return;

            // Update debug fields
            debugIsGrounded = IsGrounded;
            debugJumpCount = m_JumpCount;
            debugVerticalVelocity = m_Velocity.y;
            debugIsSitting = m_IsSitting;
            debugCurrentState = m_CurrentState?.GetType().Name ?? "None";

            // Update current state
            if (m_CurrentState != null)
            {
                m_CurrentState.Update();
            }
        }

        #region State Machine Methods

        /// <summary>
        /// Sets the current state and handles transitions
        /// </summary>
        public void SetState(PlayerState newState)
        {
            if (newState == m_CurrentState)
                return;

            m_PreviousState = m_CurrentState;
            m_CurrentState?.OnExit();
            m_CurrentState = newState;
            m_CurrentState?.OnEnter();
        }

        #endregion

        #region Movement Methods

        /// <summary>
        /// Moves the character in a given direction with the specified speed
        /// </summary>
        public void MoveCharacter(Vector2 inputVector, float speed)
        {
            // Calculate movement direction relative to camera
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * inputVector.y + cameraRight * inputVector.x;

            // Handle rotation and movement
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion newRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationMultiplier);

                m_Controller.Move(transform.forward * (speed * Time.deltaTime));
            }
        }

        /// <summary>
        /// Applies gravity to the character
        /// </summary>
        public void ApplyGravity()
        {
            if (IsGrounded && m_Velocity.y < 0)
            {
                m_Velocity.y = -2f;
            }

            m_Velocity.y += gravity * Time.deltaTime;
            m_Controller.Move(m_Velocity * Time.deltaTime);
        }

        /// <summary>
        /// Performs a jump action
        /// </summary>
        public void PerformJump()
        {
            if (IsGrounded || m_JumpCount < maxJumps)
            {
                m_Velocity.y = Mathf.Sqrt(jumpHeight * characterFallMultiplier * gravity);

                // Apply sprint boost when jumping from sprinting state
                if (m_InputHandler.IsSprinting)
                {
                    Vector3 forwardBoost = transform.forward * jumpFlipBoost;
                    m_Controller.Move(forwardBoost * Time.deltaTime);
                    
                    if (!IsGrounded)
                    {
                        m_Animator.SetTrigger("Flip");
                    }
                    else
                    {
                        m_Animator.SetTrigger("Jump");
                    }
                }
                else
                {
                    m_Animator.SetTrigger("Jump");
                }

                m_JumpCount++;
            }
        }

        /// <summary>
        /// Resets the jump count when landing
        /// </summary>
        public void ResetJumpCount()
        {
            m_JumpCount = 0;
        }

        /// <summary>
        /// Sets the vertical velocity (for landing damping)
        /// </summary>
        public void SetVerticalVelocity(float yVelocity)
        {
            m_Velocity.y = yVelocity;
        }

        /// <summary>
        /// Enters the sitting state
        /// </summary>
        public void EnterSitting()
        {
            m_IsSitting = true;
        }

        /// <summary>
        /// Exits the sitting state
        /// </summary>
        public void ExitSitting()
        {
            m_IsSitting = false;
        }

        #endregion

        #region Ground Check

        private bool IsCharacterGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }

        #endregion

        #region Pause System
        
        private void OnEnable()
        {
            PauseManager.I.Register(this);
        }

        private void OnDisable()
        {
            PauseManager.I.Unregister(this);
        }
    
        public void Pause() => m_IsPaused = true;
        public void Resume() => m_IsPaused = false;
        
        #endregion
    }
}