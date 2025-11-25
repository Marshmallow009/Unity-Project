using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform playerBody;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private float rotationX = 0f;
    private bool active = true;

    public void Activate(bool state)
    {
        active = state;
    }

    void Update()
    {
        if (!active) return;

        // Vertical camera rotation
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Horizontal player rotation
        playerBody.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
    }
}
