using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public FirstPersonCamera firstPersonCam;
    public ThirdPersonCamera thirdPersonCam;
    public Magic magicSystem;   // <-- NEW

    private Camera firstCamComponent;
    private Camera thirdCamComponent;

    private bool usingFirstPerson = true;

    void Start()
    {
        firstCamComponent = firstPersonCam.GetComponent<Camera>();
        thirdCamComponent = thirdPersonCam.GetComponent<Camera>();

        SetCameraState(usingFirstPerson);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleCameraMode();
    }

    void ToggleCameraMode()
    {
        usingFirstPerson = !usingFirstPerson;
        SetCameraState(usingFirstPerson);
    }

    void SetCameraState(bool firstPersonActive)
    {
        // Enable/disable Camera components
        firstCamComponent.enabled = firstPersonActive;
        thirdCamComponent.enabled = !firstPersonActive;

        // Activate scripts
        firstPersonCam.Activate(firstPersonActive);
        thirdPersonCam.Activate(!firstPersonActive);

        // ---- NEW: Tell magic system which camera to shoot from ----
        if (firstPersonActive)
            magicSystem.SetActiveCamera(firstCamComponent);
        else
            magicSystem.SetActiveCamera(thirdCamComponent);
    }
}
