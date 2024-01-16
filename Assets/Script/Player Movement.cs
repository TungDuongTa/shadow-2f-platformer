using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float xRaw;
    private float yRaw;
    private float xInput;
    private float yInput;
    public bool isFacingRight;
    [Header("Run")]
    [SerializeField] private float groundTime;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float accelRate;
    [SerializeField] private float runDeccel;
    [SerializeField] private float runAccel;
    [SerializeField] private float runMaxSpeed;
    [Header("Shadow")]
    private int currentAnimationFrame;
    [SerializeField] private GameObject shadowPrefab;
    private GameObject shadowInstance;
    //[Header("Animation")]
    private enum state { idle, walk, jump, fall, slide };
    state playerState;
    private Animator playerAnimator;
    [Header("Jump")]
    public float gravityStrength;
    public float gravityScale;
    public float jumpMaxForce;
    public float jumpForce;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTimeToApex;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isJumpCut;
    [Header("Collision")]
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private GameObject wallCheck;
    [SerializeField] private GameObject rightWallCheck;
    [SerializeField] private GameObject leftWallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime;

    [SerializeField] private float wallTime;
    [SerializeField] private float rightWallTime;
    [SerializeField] private float leftWallTime;
    [SerializeField] private bool isSliding;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpTime;


    [SerializeField] private bool isDashing;
    [SerializeField] private float dashBuffer;
    [SerializeField] private bool canDash;
    [SerializeField] private float lastTimeSinceDashed;
    [SerializeField] private bool isTryingToJumpDuringDash;
    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        canDash = true;
    }

    // Update is called once per frame
    void Update()
    {
        lastTimeSinceDashed += Time.deltaTime;
        groundTime -= Time.deltaTime;
        jumpBufferTime -= Time.deltaTime;
        rightWallTime -= Time.deltaTime;
        leftWallTime -= Time.deltaTime;
        dashBuffer -= Time.deltaTime;
        KeyInput();
        animationControl();
        Flip();
        CollisionCheck();
        gravityControl();
        if (Input.GetKeyDown(KeyCode.C))
        {
            LeaveShadow();
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTime = coyoteTime;
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0 && isJumping)
        {
            isJumpCut = true;

        }
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
        {
            dashBuffer = coyoteTime;
        }
        if (!isDashing)
        {
            if (jumpBufferTime > 0 && groundTime > 0 && !isJumping)
            {
                Jump();
                isJumping = true;
                isFalling = false;
                isJumpCut = false;
                isWallJumping = false;
            }
            else if (wallTime > 0 && groundTime < 0 && jumpBufferTime > 0 && !isWallJumping)
            {
                WallJump();
                isWallJumping = true;
                isJumping = false;
                isJumpCut = false;
                isFalling = false;
                wallJumpTime = Time.time;
            }
        }



        if ((isJumping || isWallJumping) && rb.velocity.y < 0)
        {
            isJumping = false;
            isFalling = true;
        }
        else if (rb.velocity.y < 0)
        {
            isFalling = true;
        }
        if (groundTime > 0)
        {
            isFalling = false;
        }
        if (groundTime > 0 && !isJumping && !isWallJumping)
        {
            isJumpCut = false;

            isFalling = false;

        }
        if (wallTime > 0 && ((isFacingRight && xRaw > 0) || (!isFacingRight && xRaw < 0)) && !isJumping && groundTime <= 0 && !isWallJumping)
        {
            isSliding = true;
            isFalling = false;
            isJumping = false;
            isJumpCut = false;

        }
        else
        {
            isSliding = false;
        }
        if (isSliding)
        {
            Slide();
        }
        if (isWallJumping && Time.time - wallJumpTime > 0.15f)
        {
            isWallJumping = false;
        }
        if (!isDashing && dashBuffer > 0 && canDash)
        {
            StartCoroutine(nameof(Sleep), 0.05f);
            Vector2 dashDir;
            //If not direction pressed, dash forward
            Vector2 _moveInput = new Vector2(xRaw, yRaw);
            if (_moveInput != Vector2.zero)
                dashDir = _moveInput;
            else
                dashDir = isFacingRight ? Vector2.right : Vector2.left;



            isDashing = true;
            isJumping = false;
            isWallJumping = false;
            isJumpCut = false;

            StartCoroutine(nameof(Dash), dashDir);
        }
        if ((groundTime > 0) && lastTimeSinceDashed > 0.5f)
        {
            canDash = true;
        }

        if (Input.GetButtonDown("Jump") && isDashing && yRaw == 0)
        {
            isTryingToJumpDuringDash = true;

        }

    }
    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Run(1);
            if (isWallJumping)
            {
                Run(0.2f);
            }
        }
        if (isDashing || isTryingToJumpDuringDash)
        {
            Run(0.2f);
        }



    }
    private void KeyInput()
    {
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }
    private void animationControl()
    {
        if (xRaw != 0)
        {
            playerState = state.walk;
        }
        else
        {
            playerState = state.idle;
        }

        if (isFalling) {
            playerState = state.fall;
        }
        else if (isJumping || isJumpCut)
        {
            playerState = state.jump;
        }
        
        playerAnimator.SetInteger("playerState", (int)playerState);
    }
    private void Flip()
    {
        if (isFacingRight && xRaw < 0 || !isFacingRight && xRaw > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

    }
    private void LeaveShadow()
    {
        if (shadowInstance != null)//allow player to leave only 1 shadow
        {
            Destroy(shadowInstance);
        }

        shadowInstance = Instantiate(shadowPrefab, transform.position, transform.rotation);

        // Clone the current state of the player's Animator
        Animator shadowAnimator = shadowInstance.GetComponent<Animator>();
        shadowAnimator.SetInteger("playerState", (int)playerState);
        shadowAnimator.speed = 0f;
        // Set darker color for the shadow
        SpriteRenderer shadowRenderer = shadowInstance.GetComponent<SpriteRenderer>();
        shadowRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    }
    private void CollisionCheck()
    {
        Vector2 groundCheckSize = new Vector2(0.7f, 0.1f);
        Vector2 wallCheckSize = new Vector2(0.2f, 1f);
        if (Physics2D.OverlapBox(groundCheck.transform.position, groundCheckSize, 0, groundLayer)) //checks if set box overlaps with ground
        {
            groundTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
        }
        if ((Physics2D.OverlapBox(rightWallCheck.transform.position, wallCheckSize, 0, wallLayer) && isFacingRight) ||
                (Physics2D.OverlapBox(leftWallCheck.transform.position, wallCheckSize, 0, wallLayer) && !isFacingRight)) //checks if set box overlaps with ground
        {
            rightWallTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
        }
        if ((Physics2D.OverlapBox(leftWallCheck.transform.position, wallCheckSize, 0, wallLayer) && isFacingRight) ||
                (Physics2D.OverlapBox(rightWallCheck.transform.position, wallCheckSize, 0, wallLayer) && !isFacingRight)) //checks if set box overlaps with ground
        {
            leftWallTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
        }

        wallTime = Mathf.Max(rightWallTime, leftWallTime);
    }
    private void Run(float lerfValue)
    {
        targetSpeed = xRaw * runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerfValue);
        if (groundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > .1f) ? runAccel : runDeccel;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > .1f) ? runAccel * 0.3f : runDeccel * 0.3f;

        }

        if ((isJumping || isFalling) && Mathf.Abs(rb.velocity.y) < 20)
        {
            accelRate *= 1.1f;
            targetSpeed *= 1.2f;
        }

        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && groundTime < 0)
        {

            accelRate = 0;
        }
        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);


    }





    private void OnValidate()
    {
        //all these function below is to ensure that player can reach the desired height(jumpHeight) in desired time(jumpTimeToApex)
        //by modify gravity scale in rigidbody2d
        //Calculate gravity strength
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        //Calculate the rigidbody's gravity scale (
        gravityScale = gravityStrength / Physics2D.gravity.y;
        //Calculate jumpForce 
        jumpMaxForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
    }

    private void Jump()
    {

        groundTime = 0;
        jumpBufferTime = 0;
        if (isTryingToJumpDuringDash)
        {
            jumpForce = jumpMaxForce * 1 / 1.3f;
        }
        else
        {
            jumpForce = jumpMaxForce;
        }

        if (rb.velocity.y < 0)
        {
            jumpForce = jumpForce + (-rb.velocity.y);
        }
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log($"Jump Force: {jumpForce}");

    }

    private void gravityControl()
    {
        float fallMultiply = 1.5f;
        float jumpCutFallMultiply = 2.5f;
        if (!isDashing)
        {
            if (isSliding)
            {
                rb.gravityScale = 0;
            }
            else if (rb.velocity.y < 0 && yRaw < 0)
            {
                rb.gravityScale = gravityScale * jumpCutFallMultiply * 3f;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -55));
            }
            else if (isJumpCut)
            {
                rb.gravityScale = gravityScale * jumpCutFallMultiply;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -35));
            }

            else if ((isJumping) && Mathf.Abs(rb.velocity.y) < 3)
            {
                rb.gravityScale = gravityScale / 2;
            }
            else if (isFalling)
            {
                rb.gravityScale = gravityScale * fallMultiply;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -25f));
            }

            else
            {
                rb.gravityScale = gravityScale;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -25f));
            }
        }
        else
        {
            rb.gravityScale = 0;
        }

    }

    private void Slide()
    {
        float slideTargetSpeed = -5f;
        float slideAccelRate = 4f;
        if (rb.velocity.y > 0 && !isWallJumping)
        {
            rb.AddForce(-rb.velocity.y * Vector2.up, ForceMode2D.Impulse);

        }
        float slideSpeedDif = slideTargetSpeed - rb.velocity.y;
        float slideMovement = slideSpeedDif * slideAccelRate;
        rb.AddForce(slideMovement * Vector2.up, ForceMode2D.Force);

    }
    private void WallJump()
    {
        groundTime = 0;
        jumpBufferTime = 0;
        wallTime = 0;
        Vector2 wallJumpForce = new Vector2(15, 25);

        if (rightWallTime > 0)
        {
            wallJumpForce.x *= -1;
        }
        else
        {
            wallJumpForce.x *= 1;
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);


        }
        rb.AddForce(wallJumpForce, ForceMode2D.Impulse);
        Debug.Log($"Wall Jump Force  X: {wallJumpForce.x}, Y: {wallJumpForce.y}");
    }

    private IEnumerator Dash(Vector2 dir)
    {
        groundTime = 0;
        dashBuffer = 0;
        float dashSpeed = 20f;
        float startTime = Time.time;
        rb.gravityScale = 0;
        lastTimeSinceDashed = 0;
        isTryingToJumpDuringDash = false;
        //dash time
        while (Time.time - startTime <= 0.15)
        {

            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            if (isTryingToJumpDuringDash)
            {



                rb.velocity = new Vector2(dir.normalized.x * 30, rb.velocity.y);
                // Exit the coroutine


                canDash = false;
                isDashing = false;

            }
            else
            {
                rb.velocity = dir.normalized * dashSpeed;
                Debug.Log($"Dash Velocity: {rb.velocity}");
            }
            yield return null;
        }

        rb.gravityScale = gravityScale;
        //rb.velocity = new Vector2(12, 12) * dir.normalized;//velocity after dashing finish
        canDash = false;
        isDashing = false;
        isTryingToJumpDuringDash = false;

    }
    private IEnumerator Sleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }

}
