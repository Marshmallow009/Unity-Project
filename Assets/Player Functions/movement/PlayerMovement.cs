using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stats")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 20f;
    public float gravity = 20f;

    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    public int PlayerHealth = 100;
    public int PlayerCurrentHealth;
    public Transform Spawn;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private Collider playerCollider;

    private bool canMove = true;

    [Header("Collision Ignore Settings")]
    public string ignoreTag = "NoCollision";

    [Header("Dash Settings")]
    public float dashDistance = 10f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.5f;
    public float doubleTapTime = 0.3f;
    public bool canDash = true;
    private bool isDashing = false;

    private float lastTapW, lastTapA, lastTapS, lastTapD;

    [Header("Double Jump Settings")]
    public int maxJumps = 2;
    private int jumpsRemaining;
    private bool isSprinting = false;

    public int GetCurrentHealth() => PlayerCurrentHealth;

    void Start()
    {
        PlayerCurrentHealth = PlayerHealth;
        characterController = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        jumpsRemaining = maxJumps;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        IgnoreExistingObjects();
    }

    void Update()
    {
        HandleDashInput();

        if (!isDashing)
            HandleMovement();
    }

    // ------------------------------
    // DASH INPUT
    // ------------------------------
    void HandleDashInput()
    {
        if (!canDash) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastTapW < doubleTapTime)
                StartCoroutine(Dash(transform.forward));
            lastTapW = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Time.time - lastTapS < doubleTapTime)
                StartCoroutine(Dash(-transform.forward));
            lastTapS = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastTapA < doubleTapTime)
                StartCoroutine(Roll(-transform.right));
            lastTapA = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastTapD < doubleTapTime)
                StartCoroutine(Roll(transform.right));
            lastTapD = Time.time;
        }
    }

    // ------------------------------
    // DASH LOGIC
    // ------------------------------
    IEnumerator Dash(Vector3 dashDirection)
    {
        isDashing = true;
        canDash = false;
        float timer = 0f;

        float originalGravity = gravity;
        gravity = 0f;

        if (playerCollider != null)
            playerCollider.enabled = false;

        while (timer < dashDuration)
        {
            characterController.Move(dashDirection.normalized * (dashDistance / dashDuration) * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (playerCollider != null)
            playerCollider.enabled = true;

        gravity = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ------------------------------
    // ROLL LOGIC
    // ------------------------------
    IEnumerator Roll(Vector3 rollDirection)
    {
        isDashing = true;
        canDash = false;
        float timer = 0f;

        float originalGravity = gravity;
        gravity = 0f;

        characterController.height = crouchHeight;

        if (playerCollider != null)
            playerCollider.enabled = false;

        while (timer < dashDuration)
        {
            characterController.Move(rollDirection.normalized * (dashDistance / dashDuration) * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        characterController.height = defaultHeight;

        if (playerCollider != null)
            playerCollider.enabled = true;

        gravity = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ------------------------------
    // MOVEMENT
    // ------------------------------
    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        if (Input.GetKeyDown(KeyCode.Tab))
            isSprinting = !isSprinting;

        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float verticalVelocity = moveDirection.y;

        moveDirection = (forward * verticalInput * currentSpeed)
                      + (right * horizontalInput * currentSpeed);

        moveDirection.y = verticalVelocity;

        if (characterController.isGrounded)
        {
            jumpsRemaining = maxJumps;
            moveDirection.y = -0.1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpPower;
                jumpsRemaining--;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
            {
                moveDirection.y = jumpPower;
                jumpsRemaining--;
            }
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    // ------------------------------
    // DAMAGE & DEATH
    // ------------------------------
    public void TakeDamage(int damage)
    {
        if (isDashing) return;

        PlayerCurrentHealth -= damage;

        if (PlayerCurrentHealth <= 0)
            Die();
    }

    void Die()
    {
        Respawn();
    }

    void Respawn()
    {
        transform.position = Spawn.position;
        PlayerCurrentHealth = PlayerHealth;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // ------------------------------
    // COLLISION IGNORE
    // ------------------------------
    void IgnoreExistingObjects()
    {
        GameObject[] objectsToIgnore = GameObject.FindGameObjectsWithTag(ignoreTag);

        foreach (GameObject obj in objectsToIgnore)
        {
            Collider objCollider = obj.GetComponent<Collider>();
            if (objCollider != null && playerCollider != null)
                Physics.IgnoreCollision(playerCollider, objCollider);
        }
    }

    public void IgnoreNewObject(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider != null && playerCollider != null)
            Physics.IgnoreCollision(playerCollider, objCollider);
    }
}
