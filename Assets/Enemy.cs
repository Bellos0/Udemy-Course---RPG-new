using UnityEngine;

public class Enemy : Entity
{

    bool playerDetected;
    protected override void Update()
    {
        HandleCollision();
        HandleMovement();
        HandleFlip();
        HanldeAnimator();
        HandleAttack();
    }



    protected override void HandleAttack()
    {
        //if detect player
        // settrigger attack animation
        if (playerDetected)
        {
            anim.SetTrigger("attack");
        }
    }


    protected override void HandleMovement()
    {
        xInput = Input.GetAxis("Horizontal");
        if (canMove)
        {
            rb.velocity = new Vector2(faceDir * moveSpeed, rb.velocity.y);

        }
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();
        playerDetected = Physics2D.OverlapCircle(attackPoint.position,attackRadius, whatisTarget);
    }
}
