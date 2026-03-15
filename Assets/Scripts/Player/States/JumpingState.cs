using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Jumping state - player is in the air after jumping
    /// </summary>
    public class JumpingState : PlayerState
    {
        private bool justJumped = true;

        public JumpingState(PlayerController controller, Animator animator) : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            justJumped = true;
            controller.PerformJump();
        }

        public override void Update()
        {
            Vector2 inputVector = controller.InputHandler.MoveInput;

            // Allow air movement
            if (inputVector.magnitude > 0.1f)
            {
                controller.MoveCharacter(inputVector, controller.MoveSpeed);
            }

            // Check for additional jump input (for double jump)
            if (controller.InputHandler.JumpPressed && controller.CanJump)
            {
                controller.PerformJump();
                controller.InputHandler.ResetJumpInput();
                return;
            }

            // Update animator
            animator.SetFloat("YVelocity", controller.Velocity.y);
            animator.SetBool("IsGrounded", controller.IsGrounded);

            // Apply gravity
            controller.ApplyGravity();

            // Return to appropriate state when grounded
            if (controller.IsGrounded)
            {
                // Reset jump count when landing
                controller.ResetJumpCount();

                if (inputVector.magnitude > 0.1f)
                {
                    if (controller.InputHandler.IsSprinting)
                    {
                        controller.SetState(new SprintingState(controller, animator));
                    }
                    else
                    {
                        controller.SetState(new MovingState(controller, animator));
                    }
                }
                else
                {
                    controller.SetState(new IdleState(controller, animator));
                }
                return;
            }

            justJumped = false;
        }

        public override void OnExit()
        {
            // Ensure animator is updated when exiting jumping state
            animator.SetBool("IsGrounded", true);
        }
    }
}

