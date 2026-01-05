using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    private float xInput;
    private bool canJump;
    protected override void Update()
    {
        base.Update();
        HandleMovement();
        TrytoJump();
    }

    protected override void HandleMovement()
    {
        xInput = Input.GetAxis("Horizontal");
        if ( isGrounded&& canMove)
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

        }

    }


    void TrytoJump()
    {
        if (isGrounded && canJump)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                jumpForce = 5;
                rb.velocity = Vector2.up * jumpForce;
                // Debug.Log("release shfit");

            }
        }

    }

    public override void EnableMotion(bool endable)
    {
        base.EnableMotion(endable);
        canJump = endable;
    }

}
