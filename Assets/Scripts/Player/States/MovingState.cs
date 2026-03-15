using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Moving state - player is walking
    /// </summary>
    public class MovingState : PlayerState
    {
        private float currentSpeed;

        public MovingState(PlayerController controller, Animator animator) : base(controller, animator)
        {
            currentSpeed = 0f;
        }

        public override void OnEnter()
        {
            currentSpeed = 0f;
            animator.SetBool("IsGrounded", true);
            // Ensure jump count is reset when entering moving state
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

            // Check for sprint input
            if (controller.InputHandler.IsSprinting)
            {
                controller.SetState(new SprintingState(controller, animator));
                return;
            }

            // Check if still moving
            if (inputVector.magnitude <= 0.1f)
            {
                controller.SetState(new IdleState(controller, animator));
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
            float targetSpeed = controller.MoveSpeed;
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

