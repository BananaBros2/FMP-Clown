using System.Collections;
using System.Collections.Generic;

//using System.Drawing; // Stop adding this god damn clanker
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    #region VARIABLES
    // ========================================================================#

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private GameObject ballObject;
    [SerializeField] private PlayerAnimations animationHandler;
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private HairHandler hairHandler;
    [SerializeField] private GameObject[] hookClips;
    [SerializeField] private Transform cameraTargetPosition;
    private Rigidbody2D rb;
    private BoxCollider2D boxColi;


    [Header("Prefabs")]
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private GameObject chainPrefab;


    [Header("Detection")]
    [SerializeField] private LayerMask solidGroundLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask semiSolidLayer;
    [SerializeField] private LayerMask metallicLayer;
    [SerializeField] private LayerMask bouncyLayer;
    private float wallCheckOffset;
    private float pixelSize = 1 / 16f;


    [Header("General")]
    [SerializeField] private float gravity = -0.981f;
    private float gravityScale = 1;
    private int directionFacing = 1;
    private Vector2 currentOmniDirection;
    bool groundedState = false;

    SurfaceVelocity surfaceVelRef;
    Vector2 surfaceVel;
    int dueToSurDetach;

    Vector2 presSurfaceVel;
    Vector2[] presSurfaceVelList = new Vector2[8];
    int presSurfaceVelCurIndex = 0;

    [Header("Running")]
    [SerializeField] private float runSpeed = 5f;
    float horizontalVel = 0;
    float acceleration = 0.8f;


    [Header("Jumping")]
    [SerializeField] private float jumpPower = 12.5f;
    private Vector2 jumpStartVel;
    private bool jumpHeld = false;
    private bool jumpStarted = false;
    [SerializeField] private float minJumpBuffer = 0.2f;
    [SerializeField] private float maxJumpBuffer = 0.8f;

    private float jumpBufferCurTime = 0;
    int jumpPowerRemaining = 0;

    float coyoteTime = 0.1f;
    float coyoteCurTime;
    Vector2 coyoteJumpStartVel;

    float limitedMovementTimer = 0;
    float noHorMovementTimer = 0;

    [Header("Input")]
    private float horizontalMov;


    [Header("Hook Throwing")]
    [SerializeField] float hookThrowDistance = 3;
    [SerializeField] float hookThrowLeeway = 0.5f;
    [SerializeField] private int maxHookCount = 3;
    [SerializeField] bool infiniteHooks = false;
    [SerializeField] bool nullVelocityOnThrow = true;
    private int curHookCount = 2;
    private GameObject currentHook;
    private GameObject currentChain;
    private bool hookThrown;
    private bool isWhipping;
    private float dashStartupTime = 0.125f;
    private float dashStartupCurTime = 0;
    private float remainingDashDistance = 0;
    private float curDashCooldown = 0;

    public Sprite activeHookSprite;
    public Sprite inactiveHookSprite;
    Vector2 whipDir;
    bool triggerEndHook;
    float recentDash = 0;
    bool endDashFloatiness = false;



    private int hookDurability = 6;
    private int curHookDurability;


    [Header("Hook Climbing")]
    public bool attemptingWallHook = false;
    public bool stuckOnHook = false;
    public bool hookOnWall = false;
    private float timeTillNextShift = 0;
    private float shiftTime = 0.3f;
    private bool attemptHookDetach = false;

    enum HookHitType
    {
        None, Wall, Ceiling, Floor
    }
    HookHitType hookedSurfaceType;


    [Header("Ballin'")]
    bool ballin = false;
    bool bounced = false;
    bool canUseBall = true;
    bool recordHeight = true;
    float highestHeight = 0;
    bool ignoreGravityChanges = false;
    bool reachedBallApex = false;
    Vector2 bouncePosition;
    float bounceHeight;
    bool ballInAir;


    [Header("Quality of Life")]
    [SerializeField, Range(0f,1f)] private float ceilingBumpLenience = 1f;


    [Header("Feature Toggle")]
    [SerializeField] bool enableJumping = true;
    [SerializeField] bool enableHookUse = true;
    [SerializeField] bool enableBallUse = true;
    [SerializeField] bool enableVariableJumping = true;
    [SerializeField] bool enableJumpBuffering = true;
    [SerializeField] bool enableCoyoteTime = true;
    [SerializeField] bool enableFastFalling = true;
    [SerializeField] bool enableCeilingBump = true;
    [SerializeField] bool enableLedgeClimb = true;
    [SerializeField] bool enableHookLeeway = true;
    [SerializeField] bool enableSemiSolidClimb = true;



    Vector2 preservedVel;
    Vector2[] preservedVelList = new Vector2[5];
    int preservedVelCurIndex = 0;

    Vector2 smoothVelocity;

    [SerializeField] private List<SpriteRenderer> characterVisuals = new List<SpriteRenderer>();
    bool isRunning = false;
    bool isJumping = false;
    bool isFalling = false;
    bool isFastFalling = false;
    bool isWallSliding = false;
    //bool isFastFalling = false;
    Vector2 normalSpritePos;
    [SerializeField] private Vector2 onBallSpritePos;

    Vector2 normalColiSize;
    [SerializeField] private Vector2 onBallColiSize;

    Vector2 lastPosition;
    Vector2 placementVelocity;

    bool disableControls = false;
    InputAction.CallbackContext iaStoredMove;
    InputAction.CallbackContext iaStoredJump;
    InputAction.CallbackContext iaStoredWhip;
    InputAction.CallbackContext iaStoredBall;

    bool momentumFrozen;
    Vector2 heldMomentum;

    float cannonTime = 0;
    CannonScript currentCannon;
    bool usingCannon;
    bool triggerCannon;

    bool inWater;
    Vector2 waterVelocity;
    float timeSinceLastSwim = 0;

    bool debuggerBool = false;

    float ignoreGravityTimer = 0;
    float bouncyBlockCooldown = 0;

    // ========================================================================#
    #endregion VARIABLES



    #region MONOBEHAVIOUR
    // ========================================================================#

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        boxColi = transform.GetComponent<BoxCollider2D>();
        normalColiSize = boxColi.size;

        normalSpritePos = characterSprite.transform.localPosition;

        wallCheckOffset = boxColi.size.x / 2; 

        Application.targetFrameRate = 60; // Move elsewhere

        disableControls = true;
    }

    private void FixedUpdate()
    {
        if (momentumFrozen)
        {
            return;
        }

        HandleMovement(); // All movement

        HandleCamera(); // Camerawork after movement logic

        HandleVisuals(); // Handle any visuals

    }

    private void Update()
    {
        if (isWhipping) // Need to check more here otherwise can skip
        {
            HandleHookEndTrigger();
        }
    }

    // ========================================================================#
    #endregion MONOBEHAVIOUR



    #region PLAYER INPUT
    // ========================================================================#

    /// <summary>
    /// Handle Movement Input
    /// </summary>
    public void OnMove(InputAction.CallbackContext movement)
    { 
        if (disableControls) 
        {
            horizontalMov = 0;
            currentOmniDirection = Vector2.zero;
            iaStoredMove = movement;
            return; 
        }

        horizontalMov = movement.ReadValue<Vector2>().x; // Get horizontal value

        currentOmniDirection = movement.ReadValue<Vector2>(); // Get movement vector

        if (movement.canceled)
        {
            // Wall hooking
            if (timeTillNextShift > 0.1f)
            {
                timeTillNextShift = 0.1f;
            }
        }

    }


    /// <summary>
    /// Handle Jump Input
    /// </summary>
    public void OnJump(InputAction.CallbackContext jump)
    {
        if (disableControls)
        {
            jumpBufferCurTime = 0;
            jumpHeld = false;
            iaStoredJump = jump;
            return;
        }

        if (!enableJumping) { return; } // If for whatever reason this is needed

        if (jump.started)
        {
            // Handle jump with jumpBuffer timer, longer timer if falling quickly
            jumpBufferCurTime = Mathf.Min(minJumpBuffer * Mathf.Max(-rb.linearVelocityY / 10, 1), maxJumpBuffer);
            jumpHeld = true;

            if (usingCannon)
            {
                triggerCannon = true;
            }

        }

        if (jump.canceled)
        {
            jumpHeld = false;
        }

    }


    /// <summary>
    /// Handle Whip Input
    /// </summary>
    public void OnWhip(InputAction.CallbackContext whipInput)
    {
        if (disableControls)
        {
            attemptHookDetach = false;
            iaStoredWhip = whipInput;
            return;
        }

        if (!enableHookUse || usingCannon) { return; } // Disable hook usage if desired

        if (whipInput.started && !hookThrown)
        {
            if (curHookCount == 0 && !infiniteHooks) { return; } // Cancel if out of Hooks

            attemptHookDetach = false;

            ThrowHook(); // Handle throwing logic

        }

        if (whipInput.canceled)
        {
            attemptHookDetach = true; // Attempt to detach from wall
        }

    }


    /// <summary>
    /// Handle ??? Input
    /// </summary>
    public void OnWallHook(InputAction.CallbackContext wallHookInput)
    {
        //if (wallHookInput.started)
        //{
        //    attemptingWallGrab = true;

        //}
        //else if (wallHookInput.canceled)
        //{
        //    attemptingWallGrab = false;
        //}

    }


    /// <summary>
    /// Handle Ball Input
    /// </summary>
    public void OnBall(InputAction.CallbackContext ballInput)
    {
        if (disableControls)
        {
            iaStoredBall = ballInput;
            return;
        }

        if (!enableBallUse || usingCannon) { return; } // Disable ball usage if desired

        if (!ballin && ballInput.started && canUseBall) // NEEDS AND !GRABBED
        {
            GameManager.Instance.DoHitFreeze(); // Freeze game briefly

            ballin = true;
            ballInAir = true;

            characterSprite.transform.localPosition = onBallSpritePos;
            ChangeColliderSize(onBallColiSize);

            if (!groundedState)
            {
                if (!(currentOmniDirection.y < 0 && enableFastFalling))
                {
                    rb.linearVelocityY = 8; // Add initial bounce upwards
                }

                recordHeight = true;
            }

        }
        else if (ballin && ballInput.started)
        {
            ExitBallState(false);
        }

    }

    // ========================================================================#
    #endregion PLAYER INPUT



    #region PHYSICS LOGIC
    // ========================================================================#

    private void HandleMovement()
    {
        groundedState = IsGrounded(false);
        WasGrounded(); // Handle Coyote Time
        coyoteCurTime -= Time.fixedDeltaTime;
        MoveWallCheck(GetMoveDir());

        placementVelocity = (Vector2)transform.position - lastPosition; // Track player's position relative to the world
        lastPosition = transform.position;

        if (HandleCannonPhysics()) { return; }

        HandleBouncyBlocks();

        PreserveVelocity(ref preservedVelList, ref preservedVelCurIndex, ref preservedVel, rb.linearVelocity); // Store movement velocity

        TrackSurfaceOn();

        //surfaceVel = Vector2.zero;
        //if (surfaceVelRef != null)
        //{
        //    Vector2 calcSurfaceVel = surfaceVelRef.GetVelocity() * 50;
        //    PreserveVelocity(ref presSurfaceVelList, ref presSurfaceVelCurIndex, ref presSurfaceVel, calcSurfaceVel); // Store surface velocity
        //    surfaceVel = presSurfaceVel;
        //    print(presSurfaceVel);
        //}


        if (HandleWallHook()) { return; } // Wall Hooking

        if (GetMoveDir() != 0 && !isWhipping && !hookThrown) { directionFacing = GetMoveDir(); }


        curDashCooldown -= Time.fixedDeltaTime;
        recentDash -= Time.fixedDeltaTime;
        if ((endDashFloatiness && recentDash < 0) || groundedState)
        {
            endDashFloatiness = false;
            ignoreGravityChanges = false;
            AlterGravityScale(1);
        }
        if (hookThrown)
        {
            HandleHookGrapple();
            HandleCeilingBump();

            HandleDownBump(); // MESSY TEMP FIX
            HandleRightBump(); // MESSY TEMP FIX
            HandleLeftBump(); // MESSY TEMP FIX

            return;
        }

        // Hook Refill on Ground
        if (groundedState && curDashCooldown <= 0 && !ballInAir) { RefillHookUses(); }

        if (inWater)
        {
            HandleWaterPhysics();
            return;
        }




        HandleHorizontalMovement();




        HandleJump();
        HandleCeilingBump();
        HandleGravity();

        isWallSliding = false;

        if (groundedState == false)
        {
            if (IsAgainstWall())
            {
                if (!ballin && !inWater)
                {
                    WallSlide(); // mmm nestihg
                }

                LedgeClimb();
            }

            SemiSolidClimb();
        }




        HandleBallPhysics();

    }


    private void TrackSurfaceOn()
    {
        bool detachedFromSurface = true;

        // Physics2D.boxcast is so ass ;[

        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(boxColi.size.x / 2, 0, 0), Vector2.down, boxColi.size.y / 2 + Pixel(5), groundLayer);
        if (!hit)
        {
            // Check right side of player for ground
            hit = Physics2D.Raycast(transform.position + new Vector3(boxColi.size.x / 2, 0, 0), Vector2.down, boxColi.size.y / 2 + Pixel(5), groundLayer);
        }

        if (hit)
        {
            // Check left side of player for ground

            if (hit) // If detected ground 
            {
                //print("hit");
                if (hit.transform.CompareTag("ComplexSurface"))
                {
                    detachedFromSurface = false; // Detected to still be on same (complex) surface


                    SurfaceVelocity surfaceObject = hit.transform.GetComponent<SurfaceVelocity>();
                    if (surfaceObject != surfaceVelRef)
                    {
                        // On different (complex) surface, transfer to new surface
                        surfaceVelRef = hit.transform.GetComponent<SurfaceVelocity>();
                        surfaceVelRef.playerToMove = this;
                        //print("contacted surface");
                    }
                }
            }


        }
        else
        {
            int wallDirection = GetMoveDir();
            hit = Physics2D.Raycast(transform.position, Vector2.down, boxColi.size.y / 2 + Pixel(5), groundLayer); // Check middle of character
            if (!hit)
            {
                // Check top half of character
                hit = Physics2D.Raycast(transform.position + new Vector3(0, boxColi.size.x / 2, 0), Vector2.right * wallDirection, boxColi.size.x / 2 + Pixel(5), groundLayer);
                
                if (!hit)
                {
                    // Check lower half of character
                    hit = Physics2D.Raycast(transform.position - new Vector3(0, boxColi.size.x / 2, 0), Vector2.right * wallDirection, boxColi.size.x / 2 + Pixel(5), groundLayer);
                }


            }

            if (hit) // If detected ground 
            {
                if (hit.transform.CompareTag("ComplexSurface"))
                {
                    detachedFromSurface = false; // Detected to still be on same (complex) surface

                    SurfaceVelocity surfaceObject = hit.transform.GetComponent<SurfaceVelocity>();
                    if (surfaceObject != surfaceVelRef)
                    {
                        // On different (complex) surface, transfer to new surface
                        surfaceVelRef = hit.transform.GetComponent<SurfaceVelocity>();
                        surfaceVelRef.playerToMove = this;
                    }
                }
            }

        }

        if (detachedFromSurface && surfaceVelRef != null)
        {
            rb.linearVelocity = presSurfaceVel;
            horizontalVel = rb.linearVelocity.x;
            surfaceVelRef.playerToMove = null;
            surfaceVelRef = null;

        }


    }


    #region BASIC MOVEMENT

    private void HandleHorizontalMovement()
    {
        if (endDashFloatiness) { return; }

        if (noHorMovementTimer > 0)
        {
            noHorMovementTimer -= Time.fixedDeltaTime;
            return;
        }


        isRunning = Mathf.Abs(horizontalMov) > 0 ? true : false;

        float slowdown = 0;

        // Quick Turn-around
        if (groundedState && ((horizontalMov > 0 && horizontalVel < 0) || (horizontalMov < 0 && horizontalVel > 0)))
        {
            horizontalVel = (horizontalVel * -1) * 0.5f;
        }

        // Acceleration
        float accAmount = groundedState ? 1 : 0.75f;
        if (!groundedState && rb.linearVelocityY < 3)
        {
            accAmount = 1f;
        }
        accAmount *= ballin ? 0.7f : 1f;


        limitedMovementTimer += Time.fixedDeltaTime * 4;
        if (groundedState) { limitedMovementTimer = 1; }
        if (limitedMovementTimer < 1)
        {
            accAmount *= 0.1f;
        }


        horizontalVel += (horizontalMov / 3) * acceleration * accAmount;

        horizontalVel = Mathf.Clamp(horizontalVel, -1, 1);

        //Deceleration
        if (horizontalMov == 0)
        {
            float decAmount = groundedState ? 1 : 0.2f;
            slowdown = 0.5f;

            if (horizontalVel > 0)
            {
                horizontalVel = Mathf.Max(horizontalVel - 0.15f * decAmount, 0);

            }
            else
            {
                horizontalVel = Mathf.Min(horizontalVel + 0.15f * decAmount, 0);
            }

        }

        if (groundedState) { slowdown = 0.3f; }


        // Final Horizontal Velocity
        //print(slowdown);
        float newHorizontalSpeed = horizontalVel * runSpeed;

        if (newHorizontalSpeed > rb.linearVelocityX && newHorizontalSpeed > 0) // If right movement surpasses current right velocity
        {
            rb.linearVelocity = new Vector2(newHorizontalSpeed, rb.linearVelocityY);
        }
        else if (newHorizontalSpeed < rb.linearVelocityX && newHorizontalSpeed < 0) // If left movement surpasses current left velocity
        {
            rb.linearVelocity = new Vector2(newHorizontalSpeed, rb.linearVelocityY);
        }
        else if (slowdown > 0)
        {
            if (rb.linearVelocityX > newHorizontalSpeed + 1) // Intended speed is slower than current speed (positive)
            {
                //print("hec");
                newHorizontalSpeed = Mathf.Max(rb.linearVelocityX - (10 * slowdown), 0);
                rb.linearVelocity = new Vector2(newHorizontalSpeed, rb.linearVelocityY);
            }
            else if (rb.linearVelocityX < newHorizontalSpeed - 1)  // Intended speed is slower than current speed (negative)
            {
                //print("option 222");
                newHorizontalSpeed = Mathf.Min(rb.linearVelocityX + (10 * slowdown), 0);
                rb.linearVelocity = new Vector2(newHorizontalSpeed, rb.linearVelocityY);
            }
            else
            {
                //print("third");
                rb.linearVelocity = new Vector2(newHorizontalSpeed, rb.linearVelocityY);
            }


        }

    }

    private void HandleJump()
    {
        jumpBufferCurTime -= Time.fixedDeltaTime;

        if (ballInAir || endDashFloatiness) { return; }

        isJumping = false;

        if (!jumpStarted)
        {
            //if (rb.linearVelocityY > 2) { return; }
            
            if (groundedState && (jumpBufferCurTime > 0) && !stuckOnHook)
            {
                // Normal jump when grounded
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = new Vector2(rb.linearVelocity.x, surfaceVel.y); 
            }
            else if (WasGrounded() && (jumpBufferCurTime > 0) && !stuckOnHook)
            {
                // Jump from coyote time
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = coyoteJumpStartVel;
            }
            else if (stuckOnHook && jumpBufferCurTime > 0)
            {
                // Jump off hook
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 4;
                jumpStartVel = new Vector2(rb.linearVelocity.x, surfaceVel.y);

                attemptHookDetach = true;
            }
            else if (!groundedState && IsAgainstWall() && jumpBufferCurTime > 0 && rb.linearVelocityY < -1)
            {
                // Jump off wall (No hook)
                jumpBufferCurTime = 0;
                jumpStarted = true;
                jumpPowerRemaining = 1;

                rb.linearVelocityX += -directionFacing * 6f;
                horizontalVel = rb.linearVelocityX;

                limitedMovementTimer = 0;
                jumpStartVel = new Vector2(rb.linearVelocity.x, 0);

                print("hfd");
            }
        }


        // If jumpbuffer is disabled; cancel jump buffer timer
        if (!enableJumpBuffering) { jumpBufferCurTime = 0; }


        if (jumpStarted && jumpPowerRemaining > 0) // Apply jump force x times
        {
            float weak = (limitedMovementTimer < 1) ? 0.75f : 1;
            jumpPowerRemaining--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStartVel.y + jumpPower * weak);
        }

        if (!enableVariableJumping) // Prevent variable jumping if disabled
        {
            if (jumpPowerRemaining == 0 && jumpStarted)
            {
                jumpStarted = false;
            }
        }
        else if (!jumpHeld && jumpStarted && jumpPowerRemaining < 2) // Variable Jumping
        {
            AlterGravityScale(1.3f); // Increase gravity to quicken jump
            jumpStarted = false;

            if (rb.linearVelocity.y > jumpStartVel.y + 8) // Reduce upwards velocity if too strong
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                    Mathf.Max(jumpStartVel.y / 0.95f, jumpStartVel.y + 8));
            }
        }

        if (jumpHeld || !enableVariableJumping) // Scale of gravity while jump button is held
        {
            AlterGravityScale(1);
        }

        if (rb.linearVelocityY > 0 && !groundedState)
        {
            isJumping = true;
        }

    }

    private void LedgeClimb()
    {
        if (!enableLedgeClimb) { return; } // Don't climb ledges if ability to is disabled
        
        // Skip ledge-climb if moving too fast downwards or if current velocity can already clear it
        if (!(rb.linearVelocity.y > -3 && rb.linearVelocity.y < 2)) { return; }


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
                    transform.position = new Vector2(transform.position.x + directionFacing / 8, transform.position.y + (boxColi.size.y - hit.distance) + Pixel());
                    rb.linearVelocityY = 0;
                }
            }

        }

    }

    private void SemiSolidClimb()
    {
        if (!enableSemiSolidClimb) { return; } // Disable extra leeway if feature is disabled

        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, boxColi.size.y / 2, 0), -Vector2.up, boxColi.size.y, semiSolidLayer);
        if (hit)
        {
            // Check if character is enough above platform and has a specific Y velocity
            if (hit.distance > boxColi.size.y / 5 && rb.linearVelocityY < 7 && rb.linearVelocityY > 1.5f)
            {
                rb.linearVelocityY = 8;
            }


        }



         


    }

    private void WallSlide()
    {
        float wallSlideSpeedMult = 1;
        if (currentOmniDirection.y < 0)
        {
            isWallSliding = true;
            wallSlideSpeedMult = 2.25f;
        }

        rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -3 * wallSlideSpeedMult);

        HandleJump();
    }

    private void HandleCeilingBump()
    {
        if (!enableCeilingBump) { return; } // Disable ceiling bumping if desired

        if (groundedState || (rb.linearVelocityY < 0)) { return; } // Don't attempt ceiling bump if not moving upwards

        RaycastHit2D hit; // Check if center of character has space to move up
        hit = Physics2D.Raycast(transform.position, Vector2.up, boxColi.size.y / 2 + Pixel(2), solidGroundLayer);
        if (!hit && rb.linearVelocity.y > 0)
        {
            bool successfulBump = false; // Track bump status

            // Check wall towards right (Checks if ray starts inside a wall first)
            Vector2 tlOrigin = new Vector2(transform.position.x - boxColi.size.x / 2, transform.position.y + boxColi.size.y / 2 + Pixel(3));
            if (!Physics2D.OverlapBox(tlOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                // Check if left side is clear
                RaycastHit2D hitTL = Physics2D.Raycast(tlOrigin, Vector2.right, boxColi.size.x, solidGroundLayer);
                if (hitTL.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTL.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x - (boxColi.size.x - hitTL.distance) - Pixel(0.5f), transform.position.y);
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                    successfulBump = true;
                }
            }

            // Check wall towards left (Checks if ray starts inside a wall first)
            Vector2 trOrigin = new Vector2(transform.position.x + boxColi.size.x / 2, transform.position.y + boxColi.size.y / 2 + Pixel(3));
            if (!Physics2D.OverlapBox(trOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                // Check if right side is clear
                RaycastHit2D hitTR = Physics2D.Raycast(trOrigin, Vector2.left, boxColi.size.x, solidGroundLayer);
                if (hitTR.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTR.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x + (boxColi.size.x - hitTR.distance) + Pixel(0.5f), transform.position.y);
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                    successfulBump = true;
                }
            }

            //Debug.DrawLine(tlOrigin, tlOrigin + new Vector2(boxColi.size.x / 2 - Pixel(0.25f), 0), Color.red, 1f);
            //Debug.DrawLine(trOrigin, trOrigin - new Vector2(boxColi.size.x / 2 - Pixel(0.25f), 0), Color.blue, 1f);


            if (ballin && !successfulBump)
            {
                reachedBallApex = true;
                AlterGravityScale(1, true);
                ignoreGravityChanges = false;
            }

        }
    }

    /// <summary>
    /// COME BACK HERE LATER
    /// </summary>
    /// <param name="vector2Dir"></param>
    //private void HandleSurfaceBump(Vector2 vector2Dir)
    //{
    //    Vector2 newBoxiColi;
    //    Vector2 newLinearVelocity;
    //    float bumpLenience;
    //    float dir;

    //    if (vector2Dir == Vector2.up)
    //    {
    //        newBoxiColi = boxColi.size;
    //        newLinearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY);
    //        bumpLenience = 0.7f;
    //        dir = 1;
    //    }
    //    else if (vector2Dir == Vector2.down)
    //    {
    //        newBoxiColi = boxColi.size;
    //        newLinearVelocity = new Vector2(rb.linearVelocityX, -rb.linearVelocityY);
    //        bumpLenience = 0.7f;
    //        dir = -1;
    //    }
    //    else if (vector2Dir != Vector2.left) 
    //    {
    //        newBoxiColi = new Vector2(boxColi.size.y, boxColi.size.x);
    //        newLinearVelocity = new Vector2(-rb.linearVelocityY, rb.linearVelocityX);
    //        bumpLenience = 0.7f;
    //        dir = -1;
    //    }
    //    else if (vector2Dir != Vector2.right) 
    //    {
    //        newBoxiColi = new Vector2(boxColi.size.y, boxColi.size.x);
    //        newLinearVelocity = new Vector2(-rb.linearVelocityY, rb.linearVelocityX);
    //        bumpLenience = 0.7f;
    //        dir = 1;
    //    }








    //    RaycastHit2D hit; // Check if center of character has space to move down
    //    hit = Physics2D.Raycast(transform.position, Vector2.up, boxColi.size.y / 2 + Pixel(2), solidGroundLayer);
    //    if (!hit && rb.linearVelocity.y > 0)
    //    {
    //        bool successfulBump = false; // Track bump status

    //        // Check wall towards right (Checks if ray starts inside a wall first)
    //        Vector2 tlOrigin = new Vector2(transform.position.x - boxColi.size.x / 2, transform.position.y + boxColi.size.y / 2 + Pixel(3));
    //        if (!Physics2D.OverlapBox(tlOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
    //        {
    //            // Check if left side is clear
    //            RaycastHit2D hitTL = Physics2D.Raycast(tlOrigin, Vector2.right, boxColi.size.x - Pixel(0.25f), solidGroundLayer);
    //            if (hitTL.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTL.distance != 0)
    //            {
    //                transform.position = new Vector2(transform.position.x - (boxColi.size.x - hitTL.distance) - Pixel(0.5f), transform.position.y);
    //                if (!hookThrown) rb.linearVelocity = preservedVel;

    //                successfulBump = true;
    //            }
    //        }

    //        // Check wall towards left (Checks if ray starts inside a wall first)
    //        Vector2 trOrigin = new Vector2(transform.position.x + boxColi.size.x / 2, transform.position.y + boxColi.size.y / 2 + Pixel(3));
    //        if (!Physics2D.OverlapBox(trOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
    //        {
    //            // Check if right side is clear
    //            RaycastHit2D hitTR = Physics2D.Raycast(trOrigin, Vector2.left, boxColi.size.x - Pixel(0.25f), solidGroundLayer);
    //            if (hitTR.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTR.distance != 0)
    //            {
    //                transform.position = new Vector2(transform.position.x + (boxColi.size.x - hitTR.distance) + Pixel(0.5f), transform.position.y);
    //                if (!hookThrown) rb.linearVelocity = preservedVel;

    //                successfulBump = true;
    //            }
    //        }

    //        //Debug.DrawLine(tlOrigin, tlOrigin + new Vector2(boxColi.size.x / 2 - Pixel(0.25f), 0), Color.red, 1f);
    //        //Debug.DrawLine(trOrigin, trOrigin - new Vector2(boxColi.size.x / 2 - Pixel(0.25f), 0), Color.blue, 1f);


    //        //if (ballin && !successfulBump)
    //        //{
    //        //    reachedBallApex = true;
    //        //    gravityScale = 1f;
    //        //    ignoreGravityChanges = false;
    //        //}

    //    }
    //}

    private void MoveWallCheck(int dir)
    {
        // Shift wallcheck to direction facing (or what is provided)
        wallCheck.transform.localPosition = new Vector2(wallCheckOffset * dir, 0);
    }

    private int GetMoveDir()
    {
        if (horizontalMov == 0)
        {
            return 0;
        }
        return horizontalMov > 0 ? 1 : -1;

    }

    #endregion BASIC MOVEMENT


    #region WATER PHYSICS

    private void HandleWaterPhysics()
    {
        bool isSwimming = true;
        Vector2 swimDir = currentOmniDirection;
        if (jumpHeld) { swimDir += Vector2.up; }

        if (swimDir.magnitude == 0)
        {
            isSwimming = false;
        }


        RefillHookUses();
        canUseBall = true;

        if (isSwimming)
        {
            timeSinceLastSwim = 0;
            waterVelocity += swimDir.normalized;
            waterVelocity += Vector2.down / 20;
            waterVelocity *= 0.8f;
        }
        else
        {
            timeSinceLastSwim += Time.fixedDeltaTime;
            if (waterVelocity.y > -4 && timeSinceLastSwim > 0.3f)
            {
                waterVelocity += Vector2.down / 6;
            }
            waterVelocity *= 0.85f;
        }

        if (waterVelocity.magnitude > 5)
        {
            waterVelocity = waterVelocity.normalized * 5;
        }

        if (ballin)
        {
            waterVelocity += Vector2.up;
        }
        
        rb.linearVelocity = waterVelocity;

    }

    #endregion WATER PHYSICS


    #region HOOK LOGIC

    /// <summary>
    /// Handle initial logic for throwing the hook
    /// </summary>
    private void ThrowHook()
    {
        hookThrown = true;
        curHookDurability = hookDurability;
        //ExitBallState(false);

        // Set hook direction
        whipDir = currentOmniDirection;
        if (whipDir == Vector2.zero)
        {
            whipDir = new Vector2(directionFacing, 0);
        }


        LayerMask layersToDetect = solidGroundLayer;
        if (whipDir.y < 0)
        {
            print("detect semi");
            layersToDetect = groundLayer;
        }


        // Check path of hook

        RaycastHit2D hitA;
        hitA = Physics2D.BoxCast(transform.position, new Vector2(Pixel(3), Pixel(3)), 0, whipDir.normalized, hookThrowDistance, layersToDetect);

        float extraDist = enableHookLeeway ? hookThrowLeeway : 0; // Check if hook can extend past limit
        RaycastHit2D hitB;
        hitB = Physics2D.BoxCast(transform.position, new Vector2(Pixel(3), Pixel(3)), 0, whipDir.normalized, hookThrowDistance + extraDist, layersToDetect);
        RaycastHit2D trueHit = hitB;

        bool hitMetal = false;

        if (trueHit) // Furthest hook hit something
        {
            // Check furthest point for metallic contact
            if (Physics2D.OverlapBox(hitB.point, new Vector2(Pixel(1), Pixel(1)), 0, metallicLayer))
            {
                trueHit = hitA; // Revert to old distance to bypass metallic surface (QoL)

                if (Physics2D.OverlapBox(hitA.point, new Vector2(Pixel(1), Pixel(1)), 0, metallicLayer))
                {
                    print("hit metal");
                    // Both distances hit metal wall, hook failed
                    hitMetal = true;
                }
            }
        }


        if (hitMetal)
        {
            hookThrown = false;
            isWhipping = false;
            triggerEndHook = false;
            hookOnWall = false;

            print("hit metal fail");
            return;
        }


        if (trueHit) // Hook hit something 
        {
            currentHook = Instantiate(hookPrefab, trueHit.point, Quaternion.identity);
            currentHook.GetComponent<HookAimSprite>().SetHookDirection(whipDir);
            hookOnWall = true;

            // Set accurate surface type hook hit
            hookedSurfaceType = GetHookHitDirection(whipDir, trueHit);

        }
        else // Hook did not hit anything
        {
            currentHook = Instantiate(hookPrefab, new Vector2(transform.position.x, transform.position.y) + whipDir.normalized * hookThrowDistance, Quaternion.identity);
            currentHook.GetComponent<HookAimSprite>().SetHookDirection(whipDir);
            hookOnWall = false;

            hookedSurfaceType = HookHitType.None;
        }

        if (nullVelocityOnThrow) { rb.linearVelocity = Vector2.zero; }
        preservedVelList = new Vector2[5];

        dashStartupCurTime = dashStartupTime; // NEED TO START ANIMATION HERE

        if (!infiniteHooks) { curHookCount--; } // Decrease hook count

    }
    private HookHitType GetHookHitDirection(Vector2 whipDir, RaycastHit2D hit)
    {
        if (Mathf.Abs(whipDir.x) > 0 && whipDir.y == 0)
        {
            // Hooked Left or Right
            return HookHitType.Wall;
        }
        else if (whipDir.y > 0 && whipDir.x == 0)
        {
            // Hooked Upwards
            return HookHitType.Ceiling;
        }
        else if (whipDir.y < 0 && whipDir.x == 0)
        {
            // Hooked Downwards
            return HookHitType.Floor;
        }
        else if (Mathf.Abs(whipDir.x) > 0 && whipDir.y > 0)
        {
            // Hooked Up-Left or Up-Right
            Vector2 hitDirection = (hit.point - hit.centroid);
            hitDirection = new Vector2(Mathf.Abs(hitDirection.x), Mathf.Abs(hitDirection.y));
            if (hitDirection.x > hitDirection.y)
            {
                return HookHitType.Wall;
            }
            else
            {
                return HookHitType.Ceiling;
            }

        }
        else if (Mathf.Abs(whipDir.x) > 0 && whipDir.y < 0)
        {
            // Hooked Down-Left or Down-Right
            Vector2 hitDirection = (hit.point - hit.centroid);
            hitDirection = new Vector2(Mathf.Abs(hitDirection.x), Mathf.Abs(hitDirection.y));
            if (hitDirection.x > hitDirection.y)
            {
                return HookHitType.Wall;
            }
            else
            {
                return HookHitType.Floor;
            }
        }

        return HookHitType.None;
    }
    
    IEnumerator LaunchWhip()
    {
        currentChain = Instantiate(chainPrefab, currentHook.transform.position, Quaternion.identity);
        currentChain.GetComponent<SetupPlayerWhip>().playerTransform = transform;
        currentChain.GetComponent<SetupPlayerWhip>().targetPoint = currentHook.transform.GetComponent<HookAimSprite>().GetChainTarget();

        yield return new WaitForSecondsRealtime(0.08f);


        float hookSpeed = 8;

        while (!triggerEndHook)
        {
            if (!momentumFrozen)
            {
                if (currentHook == null) { break; }

                float maxHookSpeed = 10;
                if (inWater)
                {
                    maxHookSpeed /= 2;
                }

                hookSpeed = Mathf.Min(hookSpeed + 6, maxHookSpeed);
                rb.linearVelocity = (currentHook.transform.position - transform.position).normalized * hookSpeed;

            }


            yield return new WaitForSeconds(0.1f);

        }

        if (currentHook != null)
        {
            HandleGrappleEnd();
            Destroy(currentChain.gameObject);
        }

    }

    private void HandleHookGrapple()
    {
        if (isWhipping)
        {
            HandleHookEndTrigger();
        }

        // Wait x amount of time before grappling hook
        dashStartupCurTime -= Time.fixedDeltaTime;
        if (dashStartupCurTime > 0)
        {
            return;
        }

        if (!isWhipping)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(LaunchWhip());
        }

        isWhipping = true;

        //hookThrown = false;


    }

    private void HandleHookEndTrigger()
    {
        if (triggerEndHook) { return; }

        float lastDashDistance = remainingDashDistance;

        remainingDashDistance = Vector2.Distance(transform.position, currentHook.transform.position);
        //if (lastDashDistance == remainingDashDistance)
        //{
        //    triggerEndHook = true;
        //}
        if (remainingDashDistance < 1f)
        {
            triggerEndHook = true;
        }



    }

    private void HandleGrappleEnd()
    {
        // Cancel out velocity held from before grapple
        horizontalVel = 0;
        rb.linearVelocity = Vector2.zero;

        recentDash = 0.15f;
        endDashFloatiness = true;

        hookThrown = false;
        isWhipping = false;
        triggerEndHook = false;

        bool wallHookSuccess = AttemptWallHook();

        if (currentHook != null)
        {
            if (hookOnWall)
            {
                //print("using hook on wall");
            }
            else
            {
                currentHook.GetComponent<BreakHookSpawner>().BreakHook(whipDir * 3);
            }

            Destroy(currentHook.gameObject);
            currentHook = null;
        }

        if (wallHookSuccess)
        {
            endDashFloatiness = false;
            return;
        }


        if (whipDir.y > 0)
        {
            rb.linearVelocityY = 5;
        }
        else if (whipDir.y < 0)
        {
            rb.linearVelocityY = -4;
        }

        rb.linearVelocityX = whipDir.x * 5;
        if (whipDir.x > 0 && horizontalMov < 0)
        {
            recentDash = 0.1f;
            rb.linearVelocityX = 4;
        }
        else if (whipDir.x < 0 && horizontalMov > 0)
        {
            recentDash = 0.1f;
            rb.linearVelocityX = -4;
        }

        horizontalVel = rb.linearVelocityX;

        if (momentumFrozen)
        {
            heldMomentum = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
        }

        if (whipDir.y >= 0 && !groundedState) { rb.linearVelocityY += 5; }

        ignoreGravityChanges = true;
        AlterGravityScale(0.3f, true);

        canUseBall = true;
    }


    private bool AttemptWallHook()
    {
        if (hookOnWall)
        {
            if (hookedSurfaceType == HookHitType.Wall)
            {
                print(whipDir);
                transform.position = 
                new Vector3(currentHook.transform.position.x - (whipDir.x * boxColi.size.x / 2), 
                currentHook.transform.position.y - (whipDir.y * boxColi.size.y / 2), currentHook.transform.position.z);

                RemoveAllVelocity();
                ExitBallState(true);

                stuckOnHook = true;
                timeTillNextShift = shiftTime / 2;

                return true;
            }



        }

        return false;

        //hasGrabbed = false;
        //if (attemptingWallGrab && grabCurDownTime < 0)
        //{
        //    if (IsAgainstWallPassive(false))
        //    {
        //        RaycastHit2D wallHit;
        //        wallHit = Physics2D.BoxCast(transform.position, new Vector2(0.01f, boxColi.size.y - 0.1f), 0, new Vector2(directionFacing, 0), 2, groundLayer);
        //        print(wallHit.distance);
        //        transform.position = transform.position + new Vector3((wallHit.distance - boxColi.size.x / 2) * directionFacing, 0, 0);
        //        rb.linearVelocityY = 0;
        //        hasGrabbed = true;
        //    }

        //}
    }

    private bool HandleWallHook()
    {
        if (stuckOnHook)
        {
            rb.linearVelocity = Vector2.zero;

            timeTillNextShift -= Time.fixedDeltaTime;
            if (currentOmniDirection.y != 0 && timeTillNextShift <= 0)
            {
                bool examineWall = false;


                LayerMask layersToDetect = solidGroundLayer;
                if (currentOmniDirection.y < 0)
                {
                    layersToDetect = groundLayer;
                }

                Vector2 targetNewPos = (Vector2)transform.position + new Vector2(Vector2.up.x, Vector2.up.y + boxColi.size.y / 2) * currentOmniDirection.y;

                RaycastHit2D hit;
                hit = Physics2D.Linecast(transform.position, targetNewPos, layersToDetect);
                if (!hit)
                {
                    examineWall = true;
                }
                else
                {
                    if (currentOmniDirection.y < 0)
                    {
                        attemptHookDetach = true;
                    }
                    else
                    {
                        // IDK hit ceiling
                    }
                        print("MISS");
                }

                if (examineWall)
                {

                    hit = Physics2D.Linecast(targetNewPos, targetNewPos + new Vector2((boxColi.size.x + Pixel(2)) * directionFacing, 0), solidGroundLayer);
                    if (!hit)
                    {
                        print("no more wall (long)");

                        if (currentOmniDirection.y > 0)
                        {
                            print("ANTOINESS");
                            stuckOnHook = false;
                            attemptHookDetach = false;
                            rb.linearVelocityY = 8;
                            horizontalVel = directionFacing / 1.5f;

                            return true;
                        }


                    }
                    else
                    {
                        // Move up/down wall
                        timeTillNextShift = shiftTime;
                        transform.position = new Vector3(transform.position.x, transform.position.y + currentOmniDirection.y, transform.position.z);

                        curHookDurability--;
                        if (curHookDurability <= 0)
                        {
                            attemptHookDetach = true;
                        }
                    }
                }


            }

            HandleJump();

            if (attemptHookDetach)
            {
                stuckOnHook = false;
                attemptHookDetach = false;
            }

            return true;
        }

        return false;
    }
    
    private void CancelHook()
    {
        triggerEndHook = true;
        StopCoroutine(LaunchWhip());

        hookThrown = false;
        isWhipping = false;
        triggerEndHook = false;

        if (currentHook != null)
        {
            currentHook.GetComponent<BreakHookSpawner>().BreakHook(whipDir * 3);

            Destroy(currentHook.gameObject);
            currentHook = null;
        }
        if (currentChain != null)
        {
            Destroy(currentChain.gameObject);
        }

    }

    public bool CanGetHooks()
    {
        return (curHookCount < maxHookCount) ? true : false;
    }

    public void RefillHookUses(int amount = -1)
    {
        if (amount == -1)
        {
            curHookCount = maxHookCount;
        }
        else
        {
            curHookCount = Mathf.Min(curHookCount + amount, maxHookCount);
        }
    }

    #endregion HOOK LOGIC


    #region BALL LOGIC

    private void HandleBallPhysics()
    {
        if (recordHeight)
        {
            highestHeight = Mathf.Max(highestHeight, transform.position.y);
        }

        if (groundedState == true)
        {
            canUseBall = true;
            if (ballin)
            {
                CheckBounce();
            }
            //else
            //{
            //    rb.linearVelocityY = 0;
            //}
        }
        else if (ballin)
        {
            ballInAir = true;
        }

        if (bounced)
        {
            HandleBallBounce();
        }
    }

    private void CheckBounce()
    {
        //transform.position -= new Vector3(0, 0.2f, 0);
        //GameManager.Instance.DoHitFreeze();

        bouncePosition = transform.position;
        bounceHeight = highestHeight - transform.position.y;
        highestHeight = -Mathf.Infinity;
        
        if (bounceHeight < 3 && jumpBufferCurTime > 0)
        {
            ballInAir = false;
            bounced = false;

            HandleJump();
        }
        else if (bounceHeight > 2)
        {
            rb.linearVelocityY = -preservedVel.y;
            bounced = true;
            reachedBallApex = false;
            ballInAir = true;
        }
        else
        {
            ballInAir = false;
            bounced = false;
        }
    }
    private void HandleBallBounce()
    {
        if (transform.position.y > bouncePosition.y + (bounceHeight * 0.7f) - 1)
        {
            reachedBallApex = true;
            AlterGravityScale(1f);
            ignoreGravityChanges = false;

            if (rb.linearVelocityY > 0)
            {
                rb.linearVelocityY = Mathf.Max(rb.linearVelocityY * 0.75f, 0);
                // ?
            }

        }
        else if (!reachedBallApex)
        {
            AlterGravityScale(0);
        }
    }
    private void ExitBallState(bool canStillUse)
    {
        ballin = false;
        ballInAir = false;
        bounced = false;
        canUseBall = canStillUse;
        AlterGravityScale(1);
        
        recordHeight = false;
        highestHeight = 0;

        characterSprite.transform.localPosition = normalSpritePos;
        ChangeColliderSize(normalColiSize);

    }
    
    #endregion BALL LOGIC


    private void HandleGravity()
    {
        if (ignoreGravityTimer > 0)
        {
            ignoreGravityTimer -= Time.fixedDeltaTime;
            if (groundedState || IsAgainstWall())
            {
                ignoreGravityTimer = 0;
            }
            return;
        }

        if (groundedState) // If on ground
        {
            AlterGravityScale(1);
        }

        isFalling = false;
        isFastFalling = false;

        if (rb.linearVelocityY < 0 && !groundedState) // Set isFalling variable
        {
            isFalling = true;
        }

        if (!groundedState && rb.linearVelocityY < 2 && rb.linearVelocityY > -2 && jumpHeld) // Extended Jump apex
        {
            isFalling = true;
            rb.linearVelocity += new Vector2(0, (gravity * gravityScale * 0.4f));
        }
        else if (!groundedState) // Normal gravity if not doing the held apex
        {
            float gravityMult = 0.9f;
            if (currentOmniDirection.y < 0 && enableFastFalling)
            {
                isFastFalling = true;
                gravityMult = 1.3f;
            }

            rb.linearVelocity += new Vector2(0, (gravity * gravityScale * gravityMult));
        }


        float maxFallMult = 0.9f;
        if (currentOmniDirection.y < 0 && enableFastFalling && !groundedState) // Fast falling
        {
            isFastFalling = true;
            maxFallMult = 1.3f;
        }

        rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -15 * maxFallMult);

    }

    private void AlterGravityScale(float newScale = 1, bool highPriority = false)
    {
        if (!ignoreGravityChanges || highPriority)
        {
            gravityScale = newScale;
        }

    }

    private void AlterVelocity(Vector2 newVelocity)
    {
        rb.linearVelocity = newVelocity;

    }

    private void PreserveVelocity(ref Vector2[] velList, ref int velListIndex, ref Vector2 presVel, Vector2 targetVelocity)
    {
        velList[velListIndex] = targetVelocity;
        velListIndex++;
        if (velListIndex >= velList.Length) { velListIndex = 0; }

        presVel = Vector2.zero;
        foreach (Vector2 vel in velList)
        {
            if (vel.magnitude > presVel.magnitude)
            {
                presVel = vel;
            }
        }

    }

    private void RemoveAllVelocity()
    {
        rb.linearVelocity = Vector2.zero;

        horizontalVel = 0;

        preservedVel = Vector2.zero;
        preservedVelList = new Vector2[5];

    }


    public void FreezeMomentum()
    {
        momentumFrozen = true;
        heldMomentum = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;

        DisablePlayerControls(true);
    }

    public void ResumeMomentum()
    {
        rb.linearVelocity = heldMomentum;
        momentumFrozen = false;

        DisablePlayerControls(false);
    }


    private void ChangeColliderSize(Vector2 newSize)
    {
        boxColi.size = newSize;
        groundCheck.localPosition = new Vector3(0, -boxColi.size.y / 2 + Pixel(), 0);
    }

    // ========================================================================#
    #endregion PHYSICS LOGIC



    #region OBSTACLES
    // ========================================================================#

    public void EnterCannon(CannonScript cannon)
    {
        currentCannon = cannon;

        RefillHookUses();

        RemoveAllVelocity();
        ExitBallState(true);
        if (hookThrown)
        {
            CancelHook(); 
        }

        cannonTime = 0.3f;
        StartCoroutine(CannonControls());
        
    }

    IEnumerator CannonControls()
    {
        usingCannon = true;

        float cannonLaunchTimer = 0.7f;

        bool canChangeDir = currentCannon.GetCannonType() == "Free" ? true : false;

        Vector2 targetCannonDir = currentOmniDirection;

        if (canChangeDir)
        {
            currentCannon.ChangeCannonDirection(targetCannonDir);
        }


        while (cannonLaunchTimer > 0)
        {
            cannonTime = 0.4f;
            cannonLaunchTimer -= 0.1f;

            if (targetCannonDir != currentOmniDirection && canChangeDir)
            {
                targetCannonDir = currentOmniDirection;
                currentCannon.ChangeCannonDirection(targetCannonDir);
            }

            if (triggerCannon)
            {
                cannonLaunchTimer = 0;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }

        }

        usingCannon = false;
        triggerCannon = false;

        rb.linearVelocity = currentCannon.GetCannonDirection().normalized * 15;
        horizontalVel = rb.linearVelocityX;

    }

    private bool HandleCannonPhysics()
    {
        if (cannonTime > 0)
        {
            cannonTime -= Time.fixedDeltaTime;

            RaycastHit2D hit;
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxColi.size.x + Pixel(), boxColi.size.y + Pixel()), 0, Vector2.zero, 0, groundLayer);
            if (hit)
            {
                print(hit.transform.gameObject.layer + " " + groundLayer.value);
                cannonTime = 0;
            }

            return true;
        }
        return false;
    }

    public void PlayerMoveRequest(Vector3 displacement)
    {
        transform.position = transform.position + displacement;
        //surfaceVel = displacement * 50; // Turn displacement into velocity
    }

    private void HandleBouncyBlocks()
    {
        bool bouncyDetection = Physics2D.OverlapBox(transform.position, boxColi.size + new Vector2(Pixel(2), Pixel(2)), 0, bouncyLayer);
        bouncyBlockCooldown -= Time.fixedDeltaTime;
        if (bouncyBlockCooldown > 0) { return; }

        if (bouncyDetection)
        {
            List<Vector2> detectionDirections = new List<Vector2>();
            detectionDirections.AddRange(new Vector2[] { 
                new Vector2(0, -boxColi.size.y / 2 - Pixel()), // Down
                new Vector2(0, boxColi.size.y / 2 + Pixel()), // Up
                new Vector2(-boxColi.size.x / 2 - Pixel(), 0), // Left
                new Vector2(boxColi.size.x / 2 + Pixel(), 0), // Right
                new Vector2(-boxColi.size.x / 2 - Pixel(), -boxColi.size.y / 2 - Pixel()), // Down Left
                new Vector2(boxColi.size.x / 2 + Pixel(), -boxColi.size.y / 2 - Pixel()), // Down Right
                new Vector2(-boxColi.size.x / 2 - Pixel(),  boxColi.size.y / 2 + Pixel()), // Top Left
                new Vector2(boxColi.size.x / 2 + Pixel(),  boxColi.size.y / 2 + Pixel()), // Top Right
            });

            int bounceDirection = 0;
            RaycastHit2D hit;
            foreach (Vector2 direction in detectionDirections) 
            {
                bounceDirection++;
                hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + direction, bouncyLayer);
                if (hit)
                {
                    break;
                }
            }

            Vector2 currentVel = rb.linearVelocity;

            switch (bounceDirection)
            {
                case 1: // Block below, bounce upwards
                    rb.linearVelocity = Vector2.up;
                    break; 
                case 2: // Block above, bounce downwards
                    rb.linearVelocity = Vector2.down;
                    break;
                case 3: // Block to the Left, bounce right
                    rb.linearVelocity = Vector2.right;
                    break;
                case 4: // Block to the Right, bounce left
                    rb.linearVelocity = Vector2.left;
                    break;
                case 5: // Block is Down-Left, bounce top-right
                    rb.linearVelocity = new Vector2(1, 1);
                    break;
                case 6: // Block is Down-Right, bounce top-left
                    rb.linearVelocity = new Vector2(-1, 1);
                    break;
                case 7: // Block is Top-Left, bounce down-right
                    rb.linearVelocity = new Vector2(1, -1);
                    break;
                case 8: // Block is Top-Right, bounce down-left
                    rb.linearVelocity = new Vector2(-1, -1);
                    break;
            }

            GameManager.Instance.DoHitFreeze();
            CancelHook();
            RefillHookUses();

            rb.linearVelocity *= 12;
            rb.linearVelocity += currentVel / 3;
            horizontalVel = rb.linearVelocity.x;

            ignoreGravityTimer = 0.1f;
            noHorMovementTimer = 0.15f;
            bouncyBlockCooldown = 0.1f;
        }
    }

    // ========================================================================#
    #endregion OBSTACLES



    #region CHECKS
    // ========================================================================#

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            horizontalVel = 0;
            inWater = true;
            waterVelocity = rb.linearVelocity;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            inWater = false;
        }

    }


    public void DisablePlayerControls(bool state = true)
    {
        disableControls = state;

        if (!state) // Refresh input
        {
            if (iaStoredMove.action != null) OnMove(iaStoredMove);
            if (iaStoredJump.action != null) OnJump(iaStoredJump);
            if (iaStoredWhip.action != null) OnWhip(iaStoredWhip);
            if (iaStoredBall.action != null) OnBall(iaStoredBall);
        }

    }

    private bool IsGrounded(bool extended)
    {
        float size = extended ? 20 : 1;
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(boxColi.size.x - 0.03f, Pixel(3) * size), 0, groundLayer);
    }

    private bool WasGrounded()
    {
        if (IsGrounded(false) && enableCoyoteTime)
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

        return Physics2D.OverlapBox(transform.position + new Vector3(boxColi.size.x / 2, 0, 0) * directionFacing, new Vector2(0.2f * reach, boxColi.size.y - 0.1f), 0, solidGroundLayer);
    }

    /// <summary>
    /// Get length of a designated amount of pixels
    /// </summary>
    /// <param name="amount">Number of pixels</param>
    /// <returns></returns>
    float Pixel(float amount = 1) { return amount * pixelSize; }

    // ========================================================================#
    #endregion CHECKS



    #region VISUALS
    // ========================================================================#

    private void HandleVisuals()
    {

        UpdateHairOffset();

        RequestCharacterAnimation();

        if (usingCannon)
        {
            foreach (SpriteRenderer charSprite in characterVisuals)
            {
                charSprite.enabled = false;
            }
        }
        else
        {
            foreach (SpriteRenderer charSprite in characterVisuals)
            {
                charSprite.enabled = true;
            }
        }

            ballObject.SetActive(ballin);

        if (directionFacing != 0)
        {
            characterSprite.flipX = (directionFacing > 0) ? false : true;

            bool hairFlip = (directionFacing > 0) ? false : true;
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

        //float xScale = rb.linearVelocityX;
        //float yScale = Mathf.Clamp(Mathf.Abs(rb.linearVelocityY) / 7 + 1, 1, 1.2f);
        //characterSprite.transform.localScale = new Vector3(1, yScale, 1);

        Animator ballAnimator = ballObject.GetComponent<Animator>();
        if (GetMoveDir() != 0) 
        { ballObject.GetComponent<SpriteRenderer>().flipX = (GetMoveDir() > 0 ? false : true); }
        ballAnimator.speed = Mathf.Abs(placementVelocity.x) * 12;
        //if (GetMoveDir() != 0) { ballAnimator. = GetMoveDir(); }


    }

    private void RequestCharacterAnimation()
    {

        if (isFastFalling)
        {
            animationHandler.ChangeAnimation(PlayerAnimations.TargetAnimation.FALL);
        }
        else if (isFalling)
        {
            animationHandler.ChangeAnimation(PlayerAnimations.TargetAnimation.FALL);
        }
        else if (isJumping)
        {
            animationHandler.ChangeAnimation(PlayerAnimations.TargetAnimation.JUMP);
        }
        else if (isRunning)
        {
            animationHandler.ChangeAnimation(PlayerAnimations.TargetAnimation.RUNNING);
        }
        else
        {

            animationHandler.ChangeAnimation(PlayerAnimations.TargetAnimation.IDLE);
        }


    }

    private void HandleCamera()
    {
        smoothVelocity = (smoothVelocity * 19 + preservedVel) / 20;
        cameraTargetPosition.localPosition = Vector2.zero + (smoothVelocity / 2);
    }

    private void UpdateHairOffset()
    {
        Vector2 currentOffset = new Vector2(0.3f, 0);
        
        float hairGravity = Mathf.Clamp(0.3f - placementVelocity.magnitude, 0, 0.2f);
        currentOffset = new Vector2(-placementVelocity.x, -placementVelocity.y - hairGravity);

        hairHandler.partOffset = currentOffset.normalized * 0.2f;
    }


    // ========================================================================#
    #endregion VISUALS



    // TO FIX/COMPRESS

    private void HandleDownBump()
    {

        if (!groundedState || rb.linearVelocityY > 0) { return; } // Don't attempt ceiling bump if not moving DOWN

        RaycastHit2D hit; // Check if center of character has space to move up
        hit = Physics2D.Raycast(transform.position, Vector2.down, boxColi.size.y / 2 + Pixel(2), groundLayer);
        if (!hit && rb.linearVelocity.y < 0)
        {
            // Check wall towards right (Checks if ray starts inside a wall first)
            Vector2 tlOrigin = new Vector2(transform.position.x - boxColi.size.x / 2, transform.position.y - boxColi.size.y / 2 - Pixel(3));
            if (!Physics2D.OverlapBox(tlOrigin, new Vector2(Pixel(), Pixel()), 0, groundLayer))
            {
                // Check if left side is clear
                RaycastHit2D hitTL = Physics2D.Raycast(tlOrigin, Vector2.right, boxColi.size.x, groundLayer);
                if (hitTL.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTL.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x - (boxColi.size.x - hitTL.distance) - Pixel(0.5f), transform.position.y);
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                }
            }

            // Check wall towards left (Checks if ray starts inside a wall first)
            Vector2 trOrigin = new Vector2(transform.position.x + boxColi.size.x / 2, transform.position.y - boxColi.size.y / 2 - Pixel(3));
            if (!Physics2D.OverlapBox(trOrigin, new Vector2(Pixel(), Pixel()), 0, groundLayer))
            {
                // Check if right side is clear
                RaycastHit2D hitTR = Physics2D.Raycast(trOrigin, Vector2.left, boxColi.size.x, groundLayer);
                if (hitTR.distance > (boxColi.size.x) * (1 - ceilingBumpLenience) && hitTR.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x + (boxColi.size.x - hitTR.distance) + Pixel(0.5f), transform.position.y);
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                }
            }

            Debug.DrawLine(tlOrigin, tlOrigin + new Vector2(boxColi.size.x / 2 - Pixel(0.25f), 0), Color.red, 1f);
            Debug.DrawLine(trOrigin, trOrigin - new Vector2(boxColi.size.x / 2 - Pixel(0.25f), 0), Color.blue, 1f);


        }
    }


    private void HandleRightBump()
    {

        if (rb.linearVelocityX < 0 && Mathf.Abs(rb.linearVelocityX) > Mathf.Abs(rb.linearVelocityY * 1.5f)) { return; } // Don't attempt ceiling bump if not moving upwards

        RaycastHit2D hit; // Check if center of character has space to move up
        hit = Physics2D.Raycast(transform.position, Vector2.right, boxColi.size.x / 2 + Pixel(2), solidGroundLayer);
        if (!hit && rb.linearVelocityX > 0)
        {
            // Check wall towards right (Checks if ray starts inside a wall first)
            Vector2 tlOrigin = new Vector2(transform.position.x + boxColi.size.x / 2 + Pixel(2), transform.position.y + boxColi.size.y / 2);
            if (!Physics2D.OverlapBox(tlOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                // Check if left side is clear
                RaycastHit2D hitTL = Physics2D.Raycast(tlOrigin, Vector2.down, boxColi.size.y - Pixel(0.25f), solidGroundLayer);
                if (hitTL.distance > (boxColi.size.y) * (1 - ceilingBumpLenience) && hitTL.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y + (boxColi.size.y - hitTL.distance) + Pixel(0.5f));
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                }
            }

            // Check wall towards left (Checks if ray starts inside a wall first)
            Vector2 trOrigin = new Vector2(transform.position.x + boxColi.size.x / 2 + Pixel(2), transform.position.y - boxColi.size.y / 2);
            if (!Physics2D.OverlapBox(trOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                // Check if right side is clear
                RaycastHit2D hitTR = Physics2D.Raycast(trOrigin, Vector2.up, boxColi.size.y - Pixel(0.25f), solidGroundLayer);
                if (hitTR.distance > (boxColi.size.y) * (1 - ceilingBumpLenience) && hitTR.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - (boxColi.size.y - hitTR.distance) - Pixel(0.5f));
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                }
            }

            //Debug.DrawLine(tlOrigin, tlOrigin - new Vector2(0, boxColi.size.y / 2 - Pixel(0.25f)), Color.red, 1f);
            //Debug.DrawLine(trOrigin, trOrigin + new Vector2(0, boxColi.size.y / 2 - Pixel(0.25f)), Color.blue, 1f);

        }
    }

    private void HandleLeftBump()
    {

        if (rb.linearVelocityX > 0 && Mathf.Abs(rb.linearVelocityX) > Mathf.Abs(rb.linearVelocityY * 1.5f)) { return; } // Don't attempt ceiling bump if not moving upwards

        RaycastHit2D hit; // Check if center of character has space to move up
        hit = Physics2D.Raycast(transform.position, Vector2.left, boxColi.size.x / 2 + Pixel(2), solidGroundLayer);
        if (!hit && rb.linearVelocityX < 0)
        {
            // Check wall towards right (Checks if ray starts inside a wall first)
            Vector2 tlOrigin = new Vector2(transform.position.x - boxColi.size.x / 2 - Pixel(2), transform.position.y + boxColi.size.y / 2);
            if (!Physics2D.OverlapBox(tlOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                // Check if left side is clear
                RaycastHit2D hitTL = Physics2D.Raycast(tlOrigin, Vector2.down, boxColi.size.y - Pixel(0.25f), solidGroundLayer);
                if (hitTL.distance > (boxColi.size.y) * (1 - ceilingBumpLenience) && hitTL.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y + (boxColi.size.y - hitTL.distance) + Pixel(0.5f));
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                }
            }

            // Check wall towards left (Checks if ray starts inside a wall first)
            Vector2 trOrigin = new Vector2(transform.position.x - boxColi.size.x / 2 - Pixel(2), transform.position.y - boxColi.size.y / 2);
            if (!Physics2D.OverlapBox(trOrigin, new Vector2(Pixel(), Pixel()), 0, solidGroundLayer))
            {
                // Check if right side is clear
                RaycastHit2D hitTR = Physics2D.Raycast(trOrigin, Vector2.up, boxColi.size.y - Pixel(0.25f), solidGroundLayer);
                if (hitTR.distance > (boxColi.size.y) * (1 - ceilingBumpLenience) && hitTR.distance != 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - (boxColi.size.y - hitTR.distance) - Pixel(0.5f));
                    if (!hookThrown) rb.linearVelocity = preservedVel;

                }
            }

            Debug.DrawLine(tlOrigin, tlOrigin - new Vector2(0, boxColi.size.y / 2 - Pixel(0.25f)), Color.red, 1f);
            Debug.DrawLine(trOrigin, trOrigin + new Vector2(0, boxColi.size.y / 2 - Pixel(0.25f)), Color.blue, 1f);

        }
    }


}
