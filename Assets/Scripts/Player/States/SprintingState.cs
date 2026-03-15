using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Sprinting state - player is running at increased speed
    /// </summary>
    public class SprintingState : PlayerState
    {
        private float currentSpeed;

        public SprintingState(PlayerController controller, Animator animator) : base(controller, animator)
        {
            currentSpeed = 0f;
        }

        public override void OnEnter()
        {
            currentSpeed = 0f;
            animator.SetBool("IsGrounded", true);
            // Ensure jump count is reset when entering sprinting state
            controller.ResetJumpCount();
        }

        public override void Update()
        {
            Vector2 inputVector = controller.InputHandler.MoveInput;

            // Exit sitting state if player moves
            if (controller.IsSitting)
            {
                controller.ExitSitting();
            }

            // Check if sprint input is released or no movement
            if (!controller.InputHandler.IsSprinting || inputVector.magnitude <= 0.1f)
            {
                if (inputVector.magnitude > 0.1f)
                {
                    controller.SetState(new MovingState(controller, animator));
                }
                else
                {
                    controller.SetState(new IdleState(controller, animator));
                }
                return;
            }

            // Check for sit input
            if (controller.InputHandler.SitPressed && controller.IsGrounded)
            {
                controller.SetState(new SittingState(controller, animator));
                controller.InputHandler.ResetSitInput();
                return;
            }

            // Check for jump input
            if (controller.InputHandler.JumpPressed)
            {
                controller.SetState(new JumpingState(controller, animator));
                controller.InputHandler.ResetJumpInput();
                return;
            }

            // Update speed with acceleration/deceleration
            float targetSpeed = controller.SprintSpeed;
            float speedSmooth = (targetSpeed > currentSpeed) ? controller.Acceleration : controller.Deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedSmooth * Time.deltaTime);

            // Handle rotation and movement
            controller.MoveCharacter(inputVector, currentSpeed);

            // Update animator
            animator.SetFloat("Speed", currentSpeed);
            animator.SetFloat("XInput", inputVector.x);

            // Apply gravity
            controller.ApplyGravity();
        }

        public override void OnExit()
        {
            currentSpeed = 0f;
        }
    }
}

