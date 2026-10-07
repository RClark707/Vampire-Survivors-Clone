using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    PlayerMovement playerMovement;
    SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.movementDirection.magnitude != 0)
        {
            anim.SetBool("IsMoving", true);
            SpriteDirectionChecker();
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }

    void SpriteDirectionChecker()
    {
        if (playerMovement.prevMoveX < 0)
        {
            // facing left
            sr.flipX = true;
        }
        else
        {
            // facing right
            sr.flipX = false;
        }
    }
}
