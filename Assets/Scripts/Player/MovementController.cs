using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float jumpHeight = 8f;

    private float moveDir;
    private float horizontalMov;

    //private bool grounded = true;

    [SerializeField]
    BoxCollider2D boxColi;
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    Transform wallCheck;
    float wallCheckOffset = 0.5f;

    [SerializeField]
    GameObject hookPrefab;
    GameObject currentHook;

    Vector2 jumpStartVel;

    bool jumpHeld = false;
    bool jumpStarted = false;

    int currentDirection = 1;
    Vector2 currentOmniDirection;


    float gravity = -0.981f;
    float gravityScale = 0.8f;

    float jumpBufferTime = 0.2f;
    float jumpBufferCurTime = 0;

    int jumpPowerRemaining = 0;


    bool dashStarted = false;
    float dashStartupTime = 0.2f;
    float dashStartupCurTime = 0;
    Vector2 dashDirection;

    Vector2 preservedVel;
    Vector2[] preservedVelList = new Vector2[5];
    int preservedVelCurIndex = 0;

    float ceilingBumpLenience = 0.3f;

    float coyoteTime = 0.125f;
    float coyoteCurTime;
    Vector2 coyoteJumpStartVel;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxColi = transform.GetComponent<BoxCollider2D>();

        wallCheckOffset = boxColi.size.x / 2;


    }




    #region PLAYER_CONTROLS


    public void OnMove(InputAction.CallbackContext movement)
    { 
        horizontalMov = movement.ReadValue<Vector2>().x;

        currentOmniDirection = movement.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext jump)
    {
        if (jump.started)
        {
            jumpBufferCurTime = jumpBufferTime * Mathf.Clamp(-rb.linearVelocityY / 10, 1, 4);
            jumpHeld = true;
        }
        if (jump.canceled)
        {
            jumpHeld = false;
        }

    }

    public void OnWhip(InputAction.CallbackContext whipInput)
    {
        print("whip");
        if (whipInput.started && !dashStarted)
        {

            Vector2 whipDir = currentOmniDirection;
            if (whipDir == Vector2.zero)
            {
                whipDir = new Vector2(currentDirection, 0);
            }

            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, whipDir.normalized, 4, groundLayer);

            //if (currentHook != null) { Destroy(currentHook.gameObject); currentHook = null; }

            if(hit)
            {
                currentHook = Instantiate(hookPrefab, hit.point, Quaternion.identity);
            }
            else
            {
                currentHook = Instantiate(hookPrefab, new Vector2(transform.position.x, transform.position.y) + whipDir.normalized * 4, Quaternion.identity);
            }

            dashStarted = true;
            dashStartupCurTime = dashStartupTime;

            rb.linearVelocity = Vector2.zero;
            gravityScale = 0;

        }

    }


    #endregion


    private int GetMoveDir()
    {
        if (horizontalMov == 0)
        {
            return 0;
        }
        return horizontalMov > 0 ? 1 : -1;

    }

    private void MoveWallCheck(int dir)
    {
        wallCheck.transform.localPosition = new Vector2(wallCheckOffset * dir, 0);
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void StartJump()
    {
        jumpStartVel = rb.linearVelocity;
        jumpHeld = true;
        jumpBufferCurTime = 0;
    }

    //private void JumpApex()
    //{
    //    hangTimeTime = 0.015f;
    //    hangTime = true;
    //    jumpHeld = false;
    //}

    //private void HangTime()
    //{
    //    if (rb.linearVelocityY < 2 && hangTimeTime > 0)
    //    {
    //        rb.linearVelocity += new Vector2(0, 1f);
    //        hangTimeTime -= Time.fixedDeltaTime;
    //    }
        
    //}


    private void StartDash()
    {

    }


    private void HandleJump()
    {
        jumpBufferCurTime -= Time.fixedDeltaTime;


        if (!jumpStarted)
        {
            if (IsGrounded(false) && (jumpBufferCurTime > 0))
            {
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = rb.linearVelocity;
            }
            else if (WasGrounded() && (jumpBufferCurTime > 0))
            {
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = coyoteJumpStartVel;
            }

        }



        if (jumpStarted && jumpPowerRemaining > 0)
        {
            jumpPowerRemaining--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStartVel.y + 12);
        }

        if (!jumpHeld && jumpStarted && jumpPowerRemaining < 2)
        {
            gravityScale = 1.5f;
            jumpStarted = false;

            if (rb.linearVelocity.y > jumpStartVel.y + 8)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                    Mathf.Max(jumpStartVel.y / 0.95f, jumpStartVel.y + 8));

            }

        }

        if (jumpHeld)
        {
            gravityScale = 1;
        }



        //if(IsGrounded())
        //{
        //    jumpStarted = false;
        //}

    }



    private void WallSlide()
    {

        if (IsAgainstWall())
        {
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -3);

            if (rb.linearVelocity.y > -3 && rb.linearVelocity.y < 2)
            {
                LedgeClimb();
            }
            
        }


    }

    private void LedgeClimb()
    {
        Vector2 rayStart = new Vector2(transform.position.x + (boxColi.size.x / 2 + 0.1f) * GetMoveDir(), transform.position.y + (boxColi.size.y / 2));

        if (!Physics2D.OverlapBox(rayStart, new Vector2(0.1f, 0.1f), 0, groundLayer))
        {

            RaycastHit2D hit = Physics2D.Raycast(rayStart, -Vector2.up, boxColi.size.y, groundLayer);

            if (hit)
            {
                print(hit.distance);
                if (hit.distance > 0.5f)
                {
                    //print(hit.distance);
                    transform.position = new Vector2(transform.position.x + moveDir / 8, transform.position.y + (boxColi.size.y - hit.distance) + 0.05f);
                    rb.linearVelocityY = 0;
                }
            }

        }

    }






    private void HandleDash()
    {
        dashStartupCurTime -= Time.fixedDeltaTime;
        if (dashStartupCurTime < 0)
        {
            rb.linearVelocity = (currentHook.transform.position - transform.position).normalized * 20;
            if (Vector2.Distance(transform.position, currentHook.transform.position) < 1)
            {
                dashStarted = false;
                gravityScale = 1;
                if (currentHook != null) { Destroy(currentHook.gameObject); currentHook = null; }

            }
            //transform.position = Vector2.MoveTowards(transform.position, currentHook.transform.position, 0.4f);
        }
    } 


    private void FixedUpdate()
    {

        if (dashStarted)
        {
            HandleDash();

            return;
        }

        rb.linearVelocity = new Vector2(horizontalMov * speed, rb.linearVelocity.y);

        rb.linearVelocity += new Vector2(0, (gravity * gravityScale));

        MoveWallCheck(GetMoveDir());

        if (IsGrounded(false))
        {
            gravityScale = 1;
            rb.linearVelocityY = 0;
        }
        
        if(!IsGrounded(true))
        {
            WallSlide();
        }

        HandleJump();




        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.up, 1, groundLayer);


        if (!hit)
        {
            RaycastHit2D hitTL;
            hitTL = Physics2D.Raycast(new Vector2(transform.position.x - boxColi.size.x / 2, transform.position.y + (boxColi.size.y / 2 + 0.05f)), Vector2.right, boxColi.size.x, groundLayer);

            RaycastHit2D hitTR;
            hitTR = Physics2D.Raycast(new Vector2(transform.position.x + boxColi.size.x / 2, transform.position.y + (boxColi.size.y / 2 + 0.05f)), Vector2.left, boxColi.size.x, groundLayer);

            if (hitTR)
            { 
                if (hitTR.distance < ceilingBumpLenience)
                {
                    transform.position = new Vector2(transform.position.x - hitTR.distance - 0.01f, transform.position.y);
                    //rb.linearVelocity = preservedVel;
                }
            }

            if (hitTL)
            {
                if (hitTL.distance < ceilingBumpLenience)
                {
                    transform.position = new Vector2(transform.position.x + hitTL.distance + 0.01f, transform.position.y);
                    rb.linearVelocity = preservedVel;
                }
            }

        }




        int moveDir = GetMoveDir();
        if (moveDir != 0)
        {
            currentDirection = moveDir;
        }

        

        preservedVelList[preservedVelCurIndex] = rb.linearVelocity;
        preservedVelCurIndex++;
        if (preservedVelCurIndex >= preservedVelList.Length) { preservedVelCurIndex = 0; }

        preservedVel = Vector2.zero;
        foreach (Vector2 vel in preservedVelList)
        {
            preservedVel += vel;
        }
        preservedVel = preservedVel / preservedVelList.Length;


        WasGrounded();
        coyoteCurTime -= Time.fixedDeltaTime;



        // Limit falling speed
        rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -20);

    }

    private bool IsGrounded(bool extended)
    {
        float size = extended ? 20 : 1;
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(boxColi.size.x - 0.02f, 0.04f * size), 0, groundLayer);
    }

    private bool WasGrounded()
    {
        if (IsGrounded(false))
        {
            coyoteCurTime = coyoteTime;
            coyoteJumpStartVel = rb.linearVelocity;
        }

        return coyoteCurTime > 0 ? true : false;
    }

    private bool IsAgainstWall()
    {
        return Physics2D.OverlapBox(wallCheck.transform.position, new Vector2(0.2f, boxColi.size.y - 0.1f), 0, groundLayer);
    }
}
