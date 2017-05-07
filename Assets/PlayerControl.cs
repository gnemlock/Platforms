/* Written by Matthew Francis Keating */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float jumpPower;
    public string horizontalAxisLabel;

    public bool canOnlyRunWhenGrounded = false;
    public bool runWithMomentum = true;

    [SerializeField] private Vector2 horizontalMovement;
    [SerializeField] private Vector2 verticalMovement;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private bool isGrounded;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        horizontalMovement = Vector2.zero;
        verticalMovement = Vector2.zero;
    }

    // Fixed Update runs in synch with the physics system
    private void FixedUpdate()
    {
        ApplyHorizontalMovement ();
        ApplyVerticalMovement ();
    }

    void ApplyHorizontalMovement()
    {
        if (!canOnlyRunWhenGrounded || isGrounded)
        {
            // If we are not set to only run when grounded, or are otherwise grounded, 
            // use the horizontal axis and movement speed to determine our current horizontal
            // movement.
            horizontalMovement.x = Input.GetAxis (horizontalAxisLabel) * movementSpeed;

            // If the horizontal movement does not have an empty x value,
            if (horizontalMovement.x != 0f)
            {
                if (runWithMomentum)
                {
                    // If we are running with momentum, add the horizontal movement as a force
                    // to the rigidbody.
                    rigidBody.AddForce (horizontalMovement);
                } 
                else
                {
                    // If we are not running with momentum, apply a direct translation to the 
                    // transform, accounting for the fixed delta time gap.
                    transform.Translate (horizontalMovement * Time.fixedDeltaTime);
                }
            }
        }
    }

    void ApplyVerticalMovement()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log ("HJU");
            verticalMovement.y = jumpPower;

            rigidBody.AddForce(verticalMovement);
        }
    }
}
