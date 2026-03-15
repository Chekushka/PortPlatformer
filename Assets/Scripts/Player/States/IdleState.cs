using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Idle state - player is standing still on the ground
    /// </summary>
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController controller, Animator animator) : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", true);
            // Ensure jump count is reset when entering idle
            controller.ResetJumpCount();
        }

        public override void Update()
        {
            // Check for sit input
            if (controller.InputHandler.SitPressed && controller.IsGrounded)
            {
                controller.SetState(new SittingState(controller, animator));
                controller.InputHandler.ResetSitInput();
                return;
            }

            // Check for movement input
            Vector2 inputVector = controller.InputHandler.MoveInput;
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
                return;
            }

            // Check for jump input while idle
            if (controller.InputHandler.JumpPressed)
            {
                controller.SetState(new JumpingState(controller, animator));
                controller.InputHandler.ResetJumpInput();
                return;
            }

            // Apply gravity
            controller.ApplyGravity();
        }
    }
}

