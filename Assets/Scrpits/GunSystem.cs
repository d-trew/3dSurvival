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
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    // Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public TextMeshProUGUI text;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        // Set Text
        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        // Shoot
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

        // Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        // RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            // If the ray hits an enemy
            // if (rayHit.collider.CompareTag("Enemy"))
            //     rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);

            // Graphics - Instantiate bullet hole and muzzle flash
            if (bulletHoleGraphic != null)
            {
                GameObject bulletHole = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                bulletHole.transform.SetParent(rayHit.collider.transform);

                // Set the scale of the bullet hole to fit the surface properly
                bulletHole.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Adjust scale as needed

                // Debug the bullet hole instantiation
                Debug.Log("Bullet hole created at: " + rayHit.point);
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
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            Debug.Log("Muzzle flash instantiated.");
        }

        // Draw a debug ray to visualize the shot
        Debug.DrawRay(fpsCam.transform.position, direction * range, Color.red, 2f);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Debug.Log("Reloading...");
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        Debug.Log("Reload finished.");
    }
}
