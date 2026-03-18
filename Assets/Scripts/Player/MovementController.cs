using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float jumpPower = 12.5f;

    private float moveDir;
    private float horizontalMov;

    //private bool grounded = true;

    [SerializeField]
    SpriteRenderer characterSprite;

    [SerializeField]
    BoxCollider2D boxColi;
    [SerializeField]
    LayerMask solidGroundLayer;
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    LayerMask semiSolidLayer;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    Transform wallCheck;
    float wallCheckOffset = 0.5f;

    [SerializeField]
    GameObject hookPrefab;
    GameObject currentHook;

    [SerializeField]
    GameObject ballObject;


    Vector2 jumpStartVel;

    bool jumpHeld = false;
    bool jumpStarted = false;

    int directionFacing = 1;
    Vector2 currentOmniDirection;


    float gravity = -0.981f;
    float gravityScale = 0.8f;

    float jumpBufferTime = 0.2f;
    float jumpBufferCurTime = 0;

    int jumpPowerRemaining = 0;

    [SerializeField] float dashDistance = 2;
    bool dashStarted = false;
    float dashStartupTime = 0.15f;
    float dashStartupCurTime = 0;
    Vector2 dashDirection;
    float remainingDashDistance = 0;
    float dashCooldown = 0;

    Vector2 preservedVel;
    Vector2[] preservedVelList = new Vector2[5];
    int preservedVelCurIndex = 0;

    float ceilingBumpLenience = 1f;

    float coyoteTime = 0.1f;
    float coyoteCurTime;
    Vector2 coyoteJumpStartVel;


    float horizontalVel = 0;

    bool groundedState = false;

    float acceleration = 1;

    bool ballin = false;
    bool bounced = false;

    Vector2 bouncePosition;
    float bounceHeight;

    bool canUseBall = true;
    bool recordHeight = true;
    float highestHeight = 0;
    bool ignoreGravityChanges = false;
    bool reachedBallApex = false;

    bool attemptingWallGrab = false;
    bool hasGrabbed = false;
    float grabCurDownTime = 0;

    [SerializeField] private HairHandler hairHandler;
    public int maxHookCount = 3;
    public GameObject[] hookClips;
    int curHookCount = 2;

    public Sprite activeHookSprite;
    public Sprite inactiveHookSprite;

    public bool attemptingWallHook = false;
    public bool stuckOnHook = false;
    public bool hookOnWall = false;
    private float timeSinceLastShift = 0;
    private float shiftTime = 0.3f;
    private bool attemptHookDetach = false;

    [SerializeField]
    private Transform cameraTargetPosition;

    [SerializeField]
    private PlayerAnimations animationHandler;

    float Pixel(float amount = 1)
    {
        float pixelSize = 1f / 16f;
        return amount * pixelSize;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxColi = transform.GetComponent<BoxCollider2D>();

        wallCheckOffset = boxColi.size.x / 2;


        Application.targetFrameRate = 60;
    }




    #region PLAYER_INPUT

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
        if (whipInput.started && !dashStarted)
        {
            attemptHookDetach = false;


            ExitBallState(false);

            Vector2 whipDir = currentOmniDirection;
            if (whipDir == Vector2.zero)
            {
                whipDir = new Vector2(directionFacing, 0);
            }

            if (curHookCount == 0) { return; }

            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, whipDir.normalized, dashDistance, solidGroundLayer);

            //if (currentHook != null) { Destroy(currentHook.gameObject); currentHook = null; }

            if(hit)
            {
                currentHook = Instantiate(hookPrefab, hit.point, Quaternion.identity);
                currentHook.GetComponent<HookAimSprite>().SetHookDirection(currentOmniDirection);
                hookOnWall = true;
            }
            else
            {
                currentHook = Instantiate(hookPrefab, new Vector2(transform.position.x, transform.position.y) + whipDir.normalized * dashDistance, Quaternion.identity);
                currentHook.GetComponent<HookAimSprite>().SetHookDirection(currentOmniDirection);
                hookOnWall = false;
            }
            curHookCount--;

            dashStarted = true;
            dashStartupCurTime = dashStartupTime;
            dashCooldown = 0.3f;
            rb.linearVelocity = Vector2.zero;
            preservedVelList = new Vector2[5];
            gravityScale = 0;

        }

        if (whipInput.canceled)
        {
            attemptHookDetach = true;
        }

    }

    public void OnWallHook(InputAction.CallbackContext wallHookInput)
    {
        if (wallHookInput.started)
        {
            attemptingWallGrab = true;

        }
        else if (wallHookInput.canceled)
        {
            attemptingWallGrab = false;
        }

    }

    public void OnBall(InputAction.CallbackContext ballInput)
    {
        if (!ballin && ballInput.started && canUseBall && !hasGrabbed)
        {
            DoHitFreeze();

            ballin = true;
            if (!groundedState)
            {
                rb.linearVelocityY = 8;
                recordHeight = true;
            }

        }
        else if (ballin && ballInput.started)
        {
            ExitBallState(false);
        }

    }



    #endregion PLAYER_INPUT

    #region PHYSICS_HANDLER
    private void ExitBallState(bool canStillUse)
    {
        ballin = false;
        bounced = false;
        canUseBall = canStillUse;

        recordHeight = false;
        highestHeight = 0;
    }

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

    private void HandleJump()
    {
        jumpBufferCurTime -= Time.fixedDeltaTime;
        
        if (!jumpStarted)
        {
            //if (rb.linearVelocityY > 2) { return; }

            if (IsGrounded(false) && (jumpBufferCurTime > 0) && !stuckOnHook)
            {
                print("1");
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = rb.linearVelocity;
            }
            else if (WasGrounded() && (jumpBufferCurTime > 0) && !stuckOnHook)
            {
                print("2");
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = coyoteJumpStartVel;
            }
            else if (stuckOnHook && jumpBufferCurTime > 0)
            {
                print("3");

                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = rb.linearVelocity;

                stuckOnHook = false;
            }


            //else if (hasGrabbed && (jumpBufferCurTime > 0) && grabCurDownTime <= 0)
            //{
            //    jumpBufferCurTime = 0;
            //    jumpStarted = true;

            //    hasGrabbed = false;
            //    grabCurDownTime = 0.6f;

            //    jumpPowerRemaining = 4;
            //    jumpStartVel = rb.linearVelocity;
            //    rb.linearVelocityX += -directionFacing * 4;

            //}

        }



        if (jumpStarted && jumpPowerRemaining > 0)
        {
            jumpPowerRemaining--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStartVel.y + jumpPower);
        }

        if (!jumpHeld && jumpStarted && jumpPowerRemaining < 2)
        {
            gravityScale = ignoreGravityChanges ? gravityScale : 1.5f;
            jumpStarted = false;

            if (rb.linearVelocity.y > jumpStartVel.y + 8)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                    Mathf.Max(jumpStartVel.y / 0.95f, jumpStartVel.y + 8));

            }

        }

        if (jumpHeld)
        {
            gravityScale = ignoreGravityChanges ? gravityScale : 1;
        }



    }



    private void WallSlide()
    {
        float wallSlideSpeedMult = 1;
        if (currentOmniDirection.y < 0)
        {
            wallSlideSpeedMult = 2.25f;
        }

        if (IsAgainstWall())
        {

            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -3 * wallSlideSpeedMult);

            if (rb.linearVelocity.y > -3 && rb.linearVelocity.y < 2)
            {
                LedgeClimb();
            }
            
        }


    }

    private void LedgeClimb()
    {
        Vector2 rayStart = new Vector2(transform.position.x + (boxColi.size.x / 2 + Pixel()) * GetMoveDir(), transform.position.y + (boxColi.size.y / 2));

        if (!Physics2D.OverlapBox(rayStart, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
        {

            RaycastHit2D hit = Physics2D.Raycast(rayStart, -Vector2.up, boxColi.size.y + Pixel(), solidGroundLayer);

            if (hit)
            {
                //print(hit.distance);
                if (hit.distance > 0.5f)
                {
                    //print(hit.distance);
                    transform.position = new Vector2(transform.position.x + moveDir / 8, transform.position.y + (boxColi.size.y - hit.distance) + Pixel());
                    rb.linearVelocityY = 0;
                }
            }

        }

    }

    private void SemiSolidClimb()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, boxColi.size.y / 2, 0) , -Vector2.up, boxColi.size.y, semiSolidLayer);
        if (hit)
        {
            print(hit.distance + " to " + boxColi.size.y / 4);
            if (hit.distance > boxColi.size.y / 4 && rb.linearVelocityY < 7 && rb.linearVelocityY > 2.5f)
            {
                rb.linearVelocityY = 8;
            }

            //hit = Physics2D.Raycast(transform.position - new Vector3(0, boxColi.size.y / 2, 0), -Vector2.up, boxColi.size.y / 3 - 0.1f, semiSolidLayer);


        }






    }




    private void HandleDash()
    {
        dashStartupCurTime -= Time.fixedDeltaTime;
        if (dashStartupCurTime < 0)
        {
            rb.linearVelocity = (currentHook.transform.position - transform.position).normalized * 20;

            float lastDashDistance = remainingDashDistance;
            remainingDashDistance = Vector2.Distance(transform.position, currentHook.transform.position);

            if (remainingDashDistance < 1)
            {

                AttemptWallHook();

                dashStarted = false;
                gravityScale = 1;
                if (currentHook != null) { Destroy(currentHook.gameObject); currentHook = null; }
                canUseBall = true;

            }
            else if (lastDashDistance == remainingDashDistance)
            {
                dashStarted = false;
                gravityScale = 1;
                if (currentHook != null) { Destroy(currentHook.gameObject); currentHook = null; }
                canUseBall = true;
            }

            //transform.position = Vector2.MoveTowards(transform.position, currentHook.transform.position, 0.4f);
        }
    } 


    private void AttemptWallHook()
    {
        if (hookOnWall)
        {
            //print("here");

            //
            transform.position = 
                new Vector3(currentHook.transform.position.x, Mathf.RoundToInt(currentHook.transform.position.y * 2) / 2, currentHook.transform.position.z);
            
            rb.linearVelocity = Vector2.zero;

            stuckOnHook = true;
        }

        return;

        hasGrabbed = false;
        if (attemptingWallGrab && grabCurDownTime < 0)
        {
            if (IsAgainstWallPassive(false))
            {
                ExitBallState(true);
                RaycastHit2D wallHit;
                wallHit = Physics2D.BoxCast(transform.position, new Vector2(0.01f, boxColi.size.y - 0.1f), 0, new Vector2(directionFacing, 0), 2, groundLayer);
                print(wallHit.distance);
                transform.position = transform.position + new Vector3((wallHit.distance - boxColi.size.x / 2) * directionFacing, 0, 0);
                rb.linearVelocityY = 0;
                hasGrabbed = true;
            }

        }
    }





    private void HandleGravity()
    {
        if (!groundedState && rb.linearVelocityY < 2 && rb.linearVelocityY > -2 && jumpHeld)
        {
            rb.linearVelocity += new Vector2(0, (gravity * gravityScale * 0.5f));
        }
        else
        {
            float gravityMult = 1;
            if (currentOmniDirection.y < 0)
            {
                gravityMult = 1.5f;
            }

            rb.linearVelocity += new Vector2(0, (gravity * gravityScale * gravityMult));
        }

        // Capping falling speed
        float maxFallMult = 1;
        if (currentOmniDirection.y < 0)
        {
            maxFallMult = 1.5f;
        }

        rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -20 * maxFallMult);

    }

    private void HandleHorizontalMovement()
    {
        if ((horizontalMov > 0 && horizontalVel < 0) || (horizontalMov < 0 && horizontalVel > 0))
        {
            horizontalVel = (horizontalVel * -1) * 0.5f;
        }


        float accAmount = groundedState ? 1 : 0.75f;
        if (!groundedState && rb.linearVelocityY < 3)
        {
            accAmount = 1f;
        }


        horizontalVel += (horizontalMov / 3) * acceleration * accAmount;

        horizontalVel = Mathf.Clamp(horizontalVel, -1, 1);

        if (horizontalMov == 0)
        {
            float decAmount = groundedState ? 1 : 0.2f;

            if (horizontalVel > 0)
            {
                horizontalVel = Mathf.Max(horizontalVel - 0.15f * decAmount, 0);

            }
            else
            {
                horizontalVel = Mathf.Min(horizontalVel + 0.15f * decAmount, 0);
            }

        }

        rb.linearVelocity = new Vector2(horizontalVel * speed, rb.linearVelocity.y);
    }

    private void HandleBounce()
    {
        //transform.position -= new Vector3(0, 0.2f, 0);
        //DoHitFreeze();

        bouncePosition = transform.position;
        bounceHeight = highestHeight - transform.position.y;
        highestHeight = -Mathf.Infinity;

        if (bounceHeight > 2)
        {
            rb.linearVelocityY = -preservedVel.y;
            bounced = true;
            reachedBallApex = false;
        }
        else
        {
            ballin = false;
            bounced = false;
        }
    }

    private void HandleBallPhysics()
    {
        if (transform.position.y > bouncePosition.y + (bounceHeight * 0.7f) - 1)
        {
            reachedBallApex = true;
            gravityScale = 1f;
            ignoreGravityChanges = false;

            if (rb.linearVelocityY > 0)
            {
                rb.linearVelocityY = Mathf.Max(rb.linearVelocityY * 0.75f, 0);
                // ?
            }

        }
        else if (!reachedBallApex)
        {
            gravityScale = 0f;
        }
    }

    private void HandleCeilingBump()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.up, boxColi.size.y / 2 + Pixel(2), solidGroundLayer);
        
        if (!hit && rb.linearVelocity.y > 0)
        {


            bool successfulBump = false;

            Vector2 tlOrigin = new Vector2(transform.position.x - boxColi.size.x / 2, transform.position.y + boxColi.size.y / 2 + Pixel(2));
            if (!Physics2D.OverlapBox(tlOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                RaycastHit2D hitTL = Physics2D.Raycast(tlOrigin, Vector2.right, boxColi.size.x - Pixel(0.25f), solidGroundLayer);
                if (hitTL.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTL.distance != 0)
                {
                    print("doing my thiung Left");
                    transform.position = new Vector2(transform.position.x - (boxColi.size.x - hitTL.distance) - Pixel(0.5f), transform.position.y);
                    if (!dashStarted) rb.linearVelocity = preservedVel;

                    successfulBump = true;
                }
            }

            Vector2 trOrigin = new Vector2(transform.position.x + boxColi.size.x / 2, transform.position.y + boxColi.size.y / 2 + Pixel(2));
            if (!Physics2D.OverlapBox(trOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                RaycastHit2D hitTR = Physics2D.Raycast(trOrigin, Vector2.left, boxColi.size.x - Pixel(0.25f), solidGroundLayer);
                if (hitTR.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTR.distance != 0)
                {
                    print("doing my thiung Left");
                    transform.position = new Vector2(transform.position.x + (boxColi.size.x - hitTR.distance) + Pixel(0.5f), transform.position.y);
                    if (!dashStarted) rb.linearVelocity = preservedVel;

                    successfulBump = true;
                }
            }


            if (ballin && !successfulBump)
            {
                reachedBallApex = true;
                gravityScale = 1f;
                ignoreGravityChanges = false;
            }

        }
    }

    private void PreserveVelocity()
    {
        preservedVelList[preservedVelCurIndex] = rb.linearVelocity;
        preservedVelCurIndex++;
        if (preservedVelCurIndex >= preservedVelList.Length) { preservedVelCurIndex = 0; }

        preservedVel = Vector2.zero;
        foreach (Vector2 vel in preservedVelList)
        {
            if (vel.magnitude > preservedVel.magnitude)
            {
                preservedVel = vel;
            }
        }

    }

    #endregion PHYSICS_HANDLER

    private void UpdateHairOffset()
    {
        Vector2 currentOffset = new Vector2(0.3f, 0);
        
        float hairGravity = Mathf.Clamp(0.3f - rb.linearVelocity.magnitude / 40, 0, 0.2f);
        currentOffset = new Vector2(-rb.linearVelocityX / 50, -rb.linearVelocityY / 50 - hairGravity);

        if (directionFacing < 0)
        {
            //currentOffset = new Vector2(-0.3f, 0);
        }

        hairHandler.partOffset = currentOffset.normalized * 0.2f;
    }


    Vector2 smoothVelocity;

    private void FixedUpdate()
    {

        PreserveVelocity();
        WasGrounded();
        coyoteCurTime -= Time.fixedDeltaTime;

        UpdateHairOffset();

        smoothVelocity = (smoothVelocity * 19 + preservedVel) / 20;
        cameraTargetPosition.localPosition = Vector2.zero + (smoothVelocity / 2);


        animationHandler.ChangeAnimation(PlayerAnimations.TargetAnimation.DEATH);


        if (stuckOnHook)
        {
            timeSinceLastShift -= Time.fixedDeltaTime;
            if (currentOmniDirection.y != 0)
            {
                if (timeSinceLastShift <= 0)
                {
                    timeSinceLastShift = shiftTime;
                    transform.position = new Vector3(transform.position.x, transform.position.y + currentOmniDirection.y, transform.position.z);
                }
                //print(currentOmniDirection.y);
            }

            HandleJump();

            if (attemptHookDetach)
            {
                stuckOnHook = false;
            }

            return;
        }



        groundedState = IsGrounded(false);

        if (groundedState && dashCooldown <= 0 && !ballin) { curHookCount = maxHookCount; }
        dashCooldown -= Time.fixedDeltaTime;



        if (dashStarted)
        {
            HandleDash();
            HandleCeilingBump();

            return;
        }

        HandleHorizontalMovement();

        HandleGravity();

        MoveWallCheck(GetMoveDir());



        if (groundedState == true)
        {
            gravityScale = 1;

            canUseBall = true;
            if (ballin)
            {
                HandleBounce();
            }
            //else
            //{
            //    rb.linearVelocityY = 0;
            //}

        }






        //hasGrabbed = false;
        //if (attemptingWallGrab && grabCurDownTime < 0)
        //{
        //    if (IsAgainstWallPassive(false))
        //    {
        //        ExitBallState(true);
        //        RaycastHit2D wallHit;
        //        wallHit = Physics2D.BoxCast(transform.position, new Vector2(0.01f, boxColi.size.y - 0.1f), 0, new Vector2(directionFacing, 0), 2, groundLayer);
        //        print(wallHit.distance);
        //        transform.position = transform.position + new Vector3((wallHit.distance - boxColi.size.x / 2) * directionFacing, 0, 0);
        //        rb.linearVelocityY = 0;
        //        hasGrabbed = true;
        //    }

        //}






        if (groundedState == false && !ballin)
        {
            WallSlide();
            SemiSolidClimb();
        }

        if (!ballin)
        {
            HandleJump();

        }
        grabCurDownTime -= Time.fixedDeltaTime;

        if (recordHeight)
        {
            highestHeight = Mathf.Max(highestHeight, transform.position.y);
        }


        if (bounced)
        {
            HandleBallPhysics();
        }

        if (!groundedState && (rb.linearVelocityY > 0))
        {
            HandleCeilingBump();
        }


        int moveDir = GetMoveDir();
        if (moveDir != 0)
        {
            directionFacing = Mathf.RoundToInt(moveDir);
        }


        

        // Landed Physics
        if (groundedState == false && groundedState != IsGrounded(false))
        {
            horizontalVel = 0.2f;
        }


        // TEMP STUFF

        ballObject.SetActive(ballin);
        if (directionFacing == 1)
        {
            characterSprite.flipX = false;
        }
        if (directionFacing == -1)
        {
            characterSprite.flipX = true;
        }


        if (GetMoveDir() != 0)
        {
            bool hairFlip = (GetMoveDir() > 0) ? false : true;
            hairHandler.SetFlipState(hairFlip);
        }

        for (int i = 0; i < hookClips.Length; i++) 
        {
            SpriteRenderer hookSprite = hookClips[i].GetComponent<SpriteRenderer>();

            if (i + 1 <= curHookCount)
            {
                hookSprite.sprite = activeHookSprite;
            }
            else
            {
                hookSprite.sprite = inactiveHookSprite;
            }
            
        }

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

    private bool IsAgainstWall(bool extended = false)
    {
       float reach = extended ? 2 : 1;

        return Physics2D.OverlapBox(wallCheck.transform.position, new Vector2(0.2f * reach, boxColi.size.y - 0.1f), 0, solidGroundLayer);
    }

    private bool IsAgainstWallPassive(bool extended = false)
    {
        float reach = extended ? 2 : 1;

        return Physics2D.OverlapBox(transform.position + new Vector3(boxColi.size.x / 2, 0,0) * directionFacing, new Vector2(0.2f * reach, boxColi.size.y - 0.1f), 0, solidGroundLayer);
    }

    private void DoHitFreeze(float duration = 0.125f)
    {

        StartCoroutine(HitFreeze(duration));
    }

    IEnumerator HitFreeze(float realTime)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(realTime);
        Time.timeScale = 1;

    }
}
