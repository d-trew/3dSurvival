using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;

    // Bools
    private bool shooting, readyToShoot, reloading;

    // References
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask whatIsEnemy;

    // Graphics
    public GameObject muzzleFlashPrefab; // Muzzle flash prefab reference
    public GameObject bulletHolePrefab;  // Bullet hole prefab reference
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        HandleInput();

        // Update ammo text
        ammoText.SetText($"{bulletsLeft} / {magazineSize}");
    }

    private void HandleInput()
    {
        // Handle shooting input
        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Handle reload input
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        // Trigger shooting logic
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate direction with spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        // Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit rayHit, range, whatIsEnemy))
        {
            Debug.Log("Hit object: " + rayHit.collider.name + " at point: " + rayHit.point);

            // If the ray hits an enemy
            // if (rayHit.collider.CompareTag("Enemy"))
            //     rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);

            // Instantiate bullet hole graphic at hit point
            if (bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                bulletHole.transform.SetParent(rayHit.collider.transform);
                Debug.Log("Bullet hole created at: " + rayHit.point);

                // Debugging visibility
                Renderer renderer = bulletHole.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Debug.Log("Bullet hole renderer found.");
                    // Check if the shader has a color property
                    if (renderer.material.HasProperty("_Color"))
                    {
                        Debug.Log("Bullet hole material color: " + renderer.material.color);
                    }
                    else
                    {
                        Debug.Log("Material doesn't support _Color property.");
                    }
                }
                else
                {
                    Debug.Log("No renderer found on bullet hole prefab.");
                }
            }
            else
            {
                Debug.LogError("Bullet hole prefab is not assigned.");
            }
        }
        else
        {
            Debug.Log("Raycast missed.");
        }

        // Instantiate muzzle flash at attack point
        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab, attackPoint.position, Quaternion.identity);
            Debug.Log("Muzzle flash instantiated.");
        }

        bulletsLeft--;
        bulletsShot--;

        Invoke(nameof(ResetShot), timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Debug.Log("Reloading...");
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        Debug.Log("Reload finished.");
    }
}
