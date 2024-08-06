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
    [SerializeField] private GameObject bulletHoleGraphic;
    [SerializeField] private ParticleSystem muzzleFlash;
    public TextMeshProUGUI text;


    // Game management
    private GameTrigger gameTrigger; // Reference to the GameTrigger script


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Start()
    {
        gameTrigger = FindObjectOfType<GameTrigger>();
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
            Debug.Log(rayHit.collider.gameObject);

            // If the ray hits an enemy
            // if (rayHit.collider.CompareTag("Enemy"))
            //     rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);


            // Check if ray hit a target
            if (rayHit.collider.CompareTag("TargetCentre") || rayHit.collider.CompareTag("TargetMiddle") || rayHit.collider.CompareTag("TargetOuter"))
            {
                gameTrigger.RegisterHit(rayHit.collider.tag);
            }
            // Graphics - Instantiate bullet hole and muzzle flash
            if (bulletHoleGraphic != null)
            {
                //Instantiate(DebugObj, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                GameObject bulletHole = Instantiate(bulletHoleGraphic, rayHit.point+rayHit.normal*0.03f, Quaternion.LookRotation(rayHit.normal));
                bulletHole.transform.SetParent(rayHit.collider.transform);

             

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

        muzzleFlash.Play();
        Debug.Log("Muzzle flash instantiated.");

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
