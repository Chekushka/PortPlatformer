using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Base class for all player states. Defines the interface for state behavior.
    /// </summary>
    public abstract class PlayerState
    {
        protected PlayerController controller;
        protected Animator animator;

        public PlayerState(PlayerController controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
        }

        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void OnEnter()
        {
        }

        /// <summary>
        /// Called every frame while this state is active
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Called when exiting this state
        /// </summary>
        public virtual void OnExit()
        {
        }
    }
}

