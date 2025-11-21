using UnityEngine;

public class newLaserBeam : MonoBehaviour, IMagicSpell
{
    [Header("Laser Settings")]
    public float maxBeamDistance = 50f;
    public float beamThickness = 0.1f;
    public float damagePerSecond = 20f;

    [Header("Spell Settings")]
    public int ManaCost => 1;
    public float Cooldown => 0.1f;

    public LineRenderer lineRenderer;

    private bool isFiring = false;
    private Transform launchPoint;

    void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;
        }
    }

    void Update()
    {
        if (!isFiring || launchPoint == null)
            return;

        // Raycast from camera center
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

        Vector3 targetPoint = ray.origin + ray.direction * maxBeamDistance;

        // Hit detection
        RaycastHit[] hits = Physics.RaycastAll(ray, maxBeamDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage((int)(damagePerSecond * Time.deltaTime));
                }
            }
        }

        // Beam origin at launch point
        Vector3 direction = (targetPoint - launchPoint.position).normalized;
        transform.position = launchPoint.position;
        transform.rotation = Quaternion.LookRotation(direction);

        float beamLength = Vector3.Distance(launchPoint.position, targetPoint);

        // Update LineRenderer
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + direction * beamLength);
    }

    public void Cast(Vector3 direction, Transform launchPoint)
    {
        this.launchPoint = launchPoint;
        StartBeam();
    }

    public void StartBeam()
    {
        isFiring = true;
        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    public void StopBeam()
    {
        isFiring = false;
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}
