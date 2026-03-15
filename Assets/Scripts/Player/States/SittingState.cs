using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Sitting state - player is sitting on the ground
    /// </summary>
    public class SittingState : PlayerState
    {
        public SittingState(PlayerController controller, Animator animator) : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            controller.EnterSitting();
            animator.SetBool("IsSitting", true);
            animator.SetLayerWeight(1, 1f);
            animator.SetFloat("Speed", 0f);
        }

        public override void Update()
        {
            Vector2 inputVector = controller.InputHandler.MoveInput;

            // Exit sitting on movement input
            if (inputVector.magnitude > 0.1f)
            {
                controller.ExitSitting();
                animator.SetBool("IsSitting", false);
                animator.SetLayerWeight(1, 0f);

                // Transition to appropriate movement state
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

            // Exit sitting on sit input (toggle)
            if (controller.InputHandler.SitPressed)
            {
                controller.ExitSitting();
                animator.SetBool("IsSitting", false);
                animator.SetLayerWeight(1, 0f);
                controller.InputHandler.ResetSitInput();
                controller.SetState(new IdleState(controller, animator));
                return;
            }

            // Check for jump input while sitting (stand up and jump)
            if (controller.InputHandler.JumpPressed && controller.IsGrounded)
            {
                controller.ExitSitting();
                animator.SetBool("IsSitting", false);
                animator.SetLayerWeight(1, 0f);
                controller.InputHandler.ResetJumpInput();
                controller.SetState(new JumpingState(controller, animator));
                return;
            }

            // Apply minimal gravity while sitting
            controller.ApplyGravity();
        }

        public override void OnExit()
        {
        }
    }
}

