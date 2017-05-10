/* Written by Matthew Francis Keating */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
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
    public float movementSpeed;
    public float maxSpeed = 200f;
    public float jumpPower;
    public string horizontalAxisLabel;

    public bool canOnlyRunWhenGrounded = false;
    public bool runWithMomentum = true;

    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private BoxCollider2D collider;
    /// <summary>The distance from the center of the collider to the ground.</summary>
    [SerializeField] private float distanceToGround;
    /// <summary>The additional buffer between the edge of the player collider and the ground to 
    /// allow for ground checking.</summary>
    [SerializeField] private float groundingBuffer = 0.1f;

    bool isGrounded
    {
        get
        {
            return Physics2D.Raycast((Vector2)transform.position, Vector2.down, 
                0.6f, 1 << 8);
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        distanceToGround = collider.bounds.extents.y;
    }

    // Fixed Update runs in synch with the physics system
    private void FixedUpdate()
    {
        ApplyHorizontalInput();
        ApplyVerticalInput();
        CapSpeed();
    }

    void CapSpeed()
    {
        if(rigidbody.velocity.magnitude > maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }
    }

    void ApplyHorizontalInput()
    {
        if(!canOnlyRunWhenGrounded || isGrounded)
        {
            // If we are not set to only run when grounded, or are otherwise grounded, 
            // use the horizontal axis and movement speed to determine our current horizontal
            // movement, and apply fixed time scaling to it.
            float runningSpeed = Input.GetAxis(horizontalAxisLabel) * movementSpeed;

            // If the horizontal movement does not have an empty x value,
            if (runningSpeed != 0f)
            {
                // Move the player towards the horizontalMovement position by its rigidbody.
                rigidbody.AddForce(Vector2.right * runningSpeed);
            }
        }
    }

    void ApplyVerticalInput()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.AddForce(Vector2.up * jumpPower);
        }
    }

    #if UNITY_EDITOR
    public void FireTestRay()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 0.6f, 1<<8);
        Debug.Log(hit.collider.name);
    }
    #endif
}

[CustomEditor(typeof(PlayerControl))] class PlayerControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Fire Ray"))
        {
            PlayerControl playerControl = target as PlayerControl;
            playerControl.FireTestRay();
        }
    }
}