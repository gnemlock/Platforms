/* Written by Matthew Francis Keating */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platforms
{
    using Dimensions = Platforms.Utility.PlayerControlDimensions;
    using Tags = Platforms.Utility.PlayerControlTags;

    #if UNITY_EDITOR
    using Tooltips = Platforms.Utility.PlayerControlTooltips;
    #endif

    /// <summary>Control for a 2D player.</summary>
    [RequireComponent(typeof(Rigidbody2D))] public class PlayerControl : MonoBehaviour
    {
        //TODO: Cap running velocity
        //TODO: Test running velocity cap so it does not interfere if player has a higher base velocity;
        //TODO: Enable grounding for jump
        //TODO: Enable option for multi jump
        //TODO: Enable interaction for climbing
        //TODO: Determine InputManger source; if default, enable key down events from InputManager instead of Input.GetKeyDown
        //TODO: Documentation
        //TODO: RUN< JUMP< CLIMB
        //TODO: Placeholder images to identify action

        [SerializeField][Tooltip(Tooltips.runForce)] private float runForce = 200f;
        [SerializeField][Tooltip(Tooltips.jumpForce)] private float jumpForce = 1000f;
        [SerializeField][Tooltip(Tooltips.maxRunVelocity)] private float maxRunVelocity = 200f;
    
        private Rigidbody2D rigidbody;
        private BoxCollider2D collider;
        /// <summary>The distance from the center of the collider to the ground.</summary>
        private float distanceToGround;
        private bool jumping = false;

        /// <summary>Determines whether this <see cref="Platforms.PlayerControl"/> is currently 
        /// grounded.</summary>
        bool isGrounded
        {
            get
            {
                // Perform a linecase between the current transform position, and the postion 
                // where the ground should be, checking only against the ground layer.
                // Return the results as a not null check.
                return Physics2D.Linecast((Vector2)transform.position, 
                    Vector2.down * (distanceToGround + Dimensions.groundBuffer), 
                    1 << Dimensions.groundLayerIndex);
            }
        }
    
        private void Awake()
        {
            // Set a reference to the local Rigidbody2D and BoxCollider2D components.
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<BoxCollider2D>();
        }
    
        private void Start()
        {
            // Set the distanceToGround as the lower y bound of the players collider.
            distanceToGround = collider.bounds.extents.y;
        }
    
        // Fixed Update runs in synch with the physics system
        private void FixedUpdate()
        {
            ApplyHorizontalInput();
            ApplyVerticalInput();
        }

        private void Update()
        {
            if(Input.GetButtonDown(Tags.jumpInput) && isGrounded)
            {
                jumping = true;
            }
        }
    
        void ApplyHorizontalInput()
        {
            // If we are not set to only run when grounded, or are otherwise grounded, 
            // use the horizontal axis and movement speed to determine our current horizontal
            // movement, and apply fixed time scaling to it.
            float runInput = Input.GetAxis(Tags.horizontalInput) * runForce;
            
            // If the horizontal movement does not have an empty x value,
            if (runInput != 0f)
            {
                // Move the player towards the horizontalMovement position by its rigidbody.
                rigidbody.AddForce(Vector2.right * runInput);
            }

            if(Mathf.Abs(rigidbody.velocity.x) > maxRunVelocity)
            {
                rigidbody.velocity = new Vector2(Mathf.Sign(rigidbody.velocity.x) * maxRunVelocity, 
                    rigidbody.velocity.y);
            }
        }
    
        void ApplyVerticalInput()
        {
            if(jumping)
            {
                rigidbody.AddForce(new Vector2(0f, jumpForce));
                jumping = false;
            }
        }
    
        #if UNITY_EDITOR
        public void FireTestRay()
        {
            RaycastHit2D hit 
                = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 0.6f, 1<<8);
            Debug.Log(hit.collider.name);
        }
        #endif
    }
}

namespace Platforms.Utility
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(PlayerControl))] class PlayerControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
    
            if(GUILayout.Button(PlayerControlLabels.fireRay))
            {
                PlayerControl playerControl = target as PlayerControl;
                playerControl.FireTestRay();
            }
        }
    }
    #endif

    public static class PlayerControlDimensions
    {
        /// <summary>The additional distance to allow between the player and the floor, 
        /// for detecting if the player is grounded.</summary>
        public const float groundBuffer = 0.1f;
        public const int groundLayerIndex = 8;
    }

    public static class PlayerControlTags
    {
        public const string horizontalInput = "Run";
        public const string jumpInput = "Jump";
    }

    #if UNITY_EDITOR
    public static class PlayerControlTooltips
    {
        public const string runForce = "The force to apply when the player is running.";
        public const string jumpForce = "The force to apply when the player jumps.";
        public const string maxRunVelocity = "The maximum velocity to allow for running";
    }

    public static class PlayerControlLabels
    {
        public const string fireRay = "Fire Ray";
    }
    #endif
}