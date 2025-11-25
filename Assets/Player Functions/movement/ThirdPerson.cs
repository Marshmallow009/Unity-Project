using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform player;           // The player's transform
    public Vector3 offset = new Vector3(0, 2, -4); // Default camera offset

    [Header("Camera Settings")]
    public float lookSpeed = 3f;        // Mouse sensitivity
    public float minY = -35f;           // Vertical rotation limits
    public float maxY = 60f;
    public float distanceSmooth = 0.1f; // Smooth camera movement
    public float rotationSmooth = 0.1f; // Smooth rotation

    [Header("Collision Settings")]
    public float collisionRadius = 0.2f; // Sphere cast radius
    public LayerMask collisionMask;

    private float yaw = 0f;  // Horizontal rotation
    private float pitch = 10f; // Vertical rotation
    private Vector3 currentOffset;
    private Vector3 desiredPosition;
    private Vector3 smoothPosition;

    private bool active = true;

    public void Activate(bool state)
    {
        active = state;
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !state;
    }

    void Start()
    {
        currentOffset = offset;
        smoothPosition = transform.position;
    }

    void LateUpdate()
    {
        if (!active) return;

        HandleRotation();
        HandlePosition();
        RotatePlayer();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minY, maxY);
    }

    void HandlePosition()
    {
        // Desired camera position based on player + offset
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        desiredPosition = player.position + rotation * offset;

        // Collision handling
        RaycastHit hit;
        Vector3 direction = desiredPosition - player.position;
        if (Physics.SphereCast(player.position, collisionRadius, direction.normalized, out hit, direction.magnitude, collisionMask))
        {
            desiredPosition = hit.point + hit.normal * collisionRadius;
        }

        // Smoothly move camera
        smoothPosition = Vector3.Lerp(transform.position, desiredPosition, distanceSmooth);
        transform.position = smoothPosition;

        // Always look at player
        transform.LookAt(player.position + Vector3.up * 1.5f); // adjust look height
    }

    void RotatePlayer()
    {
        // Rotate player to face camera direction when moving forward/backward
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (inputDir.magnitude > 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, rotationSmooth);
        }
    }
}
