/* Written by Matthew Francis Keating */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//TODO:Fix namespace generation

namespace Platforms
{
    using Tags = Platforms.Utility.PlatformsTags;
    
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

        public bool grounded { get; private set; }
        public bool preparedToJump { get; private set; }
        public bool facingRight { get; private set; }

        [SerializeField][Tooltip(Tooltips.runForce)] private float runForce = 365f;
        [SerializeField][Tooltip(Tooltips.jumpForce)] private float jumpForce = 1000f;
        [SerializeField][Tooltip(Tooltips.maximumRunSpeed)] private float maximumRunSpeed = 5f;
        [SerializeField][Tooltip(Tooltips.groundCheck)] private Transform groundCheck;

        [SerializeField] private PlayerState playerState;
        [SerializeField] private float ledgeDistance = 1.0f;
        [SerializeField] private Transform hand;
        
        private Rigidbody2D rigidbody;
        private BoxCollider2D collider;
        private Animator animator;
        /// <summary>The distance from the center of the collider to the ground.</summary>
        private float distanceToGround;
        private Transform attachedLedge;
        
        private void Awake()
        {
            // Set a reference to the local Rigidbody2D and BoxCollider2D components.
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerState = PlayerState.Default;
            facingRight = true;
            preparedToJump = false;
        }
        
        // Fixed Update runs in synch with the physics system
        private void FixedUpdate()
        {
            if(playerState == PlayerState.Default)
            {
                CheckInputDefaultState();
            }
            else if(playerState == PlayerState.Hanging)
            {
                CheckInputHangingState();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag == Tags.ledge)
            {
                AttemptToHang(other.transform);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(other.tag == Tags.ledge)
            {
                AttemptToHang(other.transform);
            }
        }

        private void Update()
        {
            grounded = Physics2D.Linecast(transform.position, groundCheck.position, 
                1 << LayerMask.NameToLayer(Tags.groundLayer));

            if(Input.GetButtonDown(Tags.jumpInput) && grounded)
            {
                preparedToJump = true;
                //TODO: Implement ability to "power" jump
            }
        }

        private void CheckInputDefaultState()
        {
            float horizontalInput = Input.GetAxis(Tags.horizontalInput);

            animator.SetFloat(Tags.speed, Mathf.Abs(horizontalInput));

            if((horizontalInput * rigidbody.velocity.x) < maximumRunSpeed)
            {
                rigidbody.AddForce(Vector2.right * horizontalInput * runForce);
            }

            if(Mathf.Abs(rigidbody.velocity.x) > maximumRunSpeed)
            {
                Vector2 cappedVelocity 
                    = new Vector2((Mathf.Sign(rigidbody.velocity.x) * maximumRunSpeed), 
                    rigidbody.velocity.y);

                rigidbody.velocity = cappedVelocity;
            }

            if((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            {
                FlipCharacter();
            }

            if(preparedToJump)
            {
                animator.SetTrigger(Tags.jumping);

                rigidbody.AddForce(Vector2.up * jumpForce);

                preparedToJump = false; 
            }
        }

        private void CheckInputHangingState()
        {
        }

        private void ApplyClimbingInput()
        {
            float verticalInput = Input.GetAxis(Tags.horizontalInput);
        }

        private void FlipCharacter()
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private bool CheckInputForHanging(Vector3 ledgePosition)
        {
            if(Vector2.Distance(ledgePosition, hand.position) <= ledgeDistance)
            {
                if((transform.position.x - ledgePosition.x) < 0)
                {
                    if(Input.GetAxis(Tags.horizontalInput) > 0)
                    {
                        // we are moving towards the ledge;
                        return true;
                    }
                }
                else
                {
                    if(Input.GetAxis(Tags.horizontalInput) < 0)
                    {
                        return true;
                    }
                }
            }
                
            return false;
        }

        public void AttemptToHang(Transform ledge)
        {
            if(CheckInputForHanging(ledge.position))
            {
                Debug.Log("Hanging");
                rigidbody.gravityScale = 0f;
                playerState = PlayerState.Hanging;
                attachedLedge = ledge;

                transform.position = ledge.position - (hand.position - transform.position);
            }
        }

        public void StopHanging()
        {
            playerState = PlayerState.Default;
            attachedLedge = null;
            rigidbody.gravityScale = 1.0f;
        }

        #if UNITY_EDITOR
        public void FireTestRay()
        {
            RaycastHit2D hit 
                = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 0.6f, 1<<8);
            Debug.Log(hit.collider.name + " : " + hit.collider.gameObject.layer);
        }
        #endif
    }

    [System.Serializable]public enum PlayerState
    {
        Default,
        Hanging
    };
}

namespace Platforms.Utility
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(PlayerControl))] class PlayerControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PlayerControl playerControl = target as PlayerControl;
    
            if(GUILayout.Button(PlayerControlLabels.fireRay))
            {
                playerControl.FireTestRay();
            }

            if(GUILayout.Button("Stop Hanging"))
            {
                playerControl.StopHanging();
            }
        }
    }
    #endif
    
    public static class PlayerControlDimensions
    {
    }
    
    public static partial class PlatformsTags
    {
        public const string horizontalInput = "Run";
        public const string jumpInput = "Jump";
        public const string ledge = "Ledge";
        public const string player = "Player";
        public const string groundLayer = "Ground";
        public const string speed = "Speed";
        public const string climbing = "Climbing";
        public const string jumping = "Jumping";
    }
    
    #if UNITY_EDITOR
    public static class PlayerControlTooltips
    {
        public const string runForce = "The force to apply when the player is running.";
        public const string jumpForce = "The force to apply when the player jumps.";
        public const string maximumRunSpeed = "The maximum velocity to allow for running";
        public const string groundCheck = "Transform to use when casting line checks for "
            + "ground detection.";
    }
    
    public static class PlayerControlLabels
    {
        public const string fireRay = "Fire Ray";
    }
    #endif
}