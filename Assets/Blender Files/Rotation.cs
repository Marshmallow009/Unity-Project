using UnityEngine;

public class Rotation : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis rotationAxis = Axis.Y;
    public float spinSpeed = 90f;

    void Update()
    {
        Vector3 axis = Vector3.up;
        if (rotationAxis == Axis.X) axis = Vector3.right;
        if (rotationAxis == Axis.Z) axis = Vector3.forward;

        transform.Rotate(axis, spinSpeed * Time.deltaTime, Space.Self);
    }
}
