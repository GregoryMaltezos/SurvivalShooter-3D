using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    public float fireRate = 15f;
    public ParticleSystem muzzleFlash;
    private bool isReloading = false;
    private Animator animator;
    public GameObject impactEffect;
    private float nextTimeToFire = 0f;
    private bool isShooting = false;
    public int maxAmmo = 30;
    private int currentAmmo;
    private int currentAmmoInReserve; // New variable for reserve ammo
    public float normalReloadTime = 1.5f;
    public Text ammoText; // Reference to the UI text element to display ammo information
    public bool infiniteReserveAmmo = false;

    private bool canFire = true; // Flag to control firing ability
    private PlayerHealth playerHealth; // Reference to PlayerHealth script

    public AudioSource shootingAudioSource;
    public AudioClip shootingSound;
    public AudioClip reloadSound;
    public AudioSource reloadAudioSource;


    void Start()
    {
        currentAmmo = maxAmmo;
        currentAmmoInReserve = maxAmmo; // Start with full reserve ammo
        animator = GetComponent<Animator>();

        // Assign the PlayerHealth component reference here
        playerHealth = GetComponentInParent<PlayerHealth>();
        shootingAudioSource = GetComponent<AudioSource>();
        reloadAudioSource = gameObject.AddComponent<AudioSource>();
        reloadAudioSource.clip = reloadSound;
        reloadAudioSource.playOnAwake = false;
        // Assign the shooting sound to the AudioSource
        shootingAudioSource.clip = shootingSound;
        // Set initial UI text
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo, reloading...");
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }

            return;
        }

        // Check if the playerHealth reference is null or the player is dead
        if (playerHealth == null || playerHealth.IsDead())
        {
            
            canFire = false;
        }
        else
        {
            
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && canFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R) && canFire)
            {
                StartCoroutine(Reload());
            }
        }

        // Update UI text during gameplay
        UpdateAmmoUI();
    }



    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reloading", true);
        reloadAudioSource.Play();
        int bulletsToReload;

        if (infiniteReserveAmmo)
        {
            bulletsToReload = maxAmmo - currentAmmo;
        }
        else
        {
            bulletsToReload = Mathf.Min(maxAmmo - currentAmmo, currentAmmoInReserve);
        }

        Debug.Log("Reloading " + bulletsToReload + " bullets...");
        yield return new WaitForSeconds(normalReloadTime);

        animator.SetBool("Reloading", false);

        // Update ammo counts after reloading
        currentAmmo += bulletsToReload;

        if (!infiniteReserveAmmo)
        {
            currentAmmoInReserve -= bulletsToReload;
        }

        isReloading = false;

        // Update UI text after reloading
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        // Update UI text to display current ammo information
        ammoText.text = "Ammo: " + currentAmmo + "/" + currentAmmoInReserve;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    void Shoot()
    {
        muzzleFlash.Play();
        currentAmmo--;

        // Trigger the shooting animation
        animator.SetBool("IsShooting", true);
        isShooting = true;

        StartCoroutine(StopPreviousAudioSource());

        // Play the shooting sound
        shootingAudioSource.PlayOneShot(shootingSound);
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            GameObject impactGameObj = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGameObj, 2f);
        }
        StartCoroutine(ResetShootingState());
    }
    IEnumerator StopPreviousAudioSource()
    {
        // Wait for one second before stopping the audio source
        yield return new WaitForSeconds(1.5f);

        // Stop the audio source
        shootingAudioSource.Stop();
    }
    IEnumerator ResetShootingState()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsShooting", false);
        isShooting = false;
    }

    public void AddAmmo()
    {
        int bulletsToAdd = 2 * maxAmmo; // Adding two magazines worth of bullets
        int totalReserveAmmo = currentAmmoInReserve + bulletsToAdd;

        // Check if adding the bullets would exceed the maximum reserve ammo limit (3 magazines)
        if (totalReserveAmmo > 3 * maxAmmo)
        {
            // Calculate the remaining space for reserve ammo before reaching the limit
            int remainingSpace = 3 * maxAmmo - currentAmmoInReserve;

            // Add ammo up to the limit
            currentAmmoInReserve += remainingSpace;
        }
        else
        {
            // Add the full amount of bullets (two magazines worth)
            currentAmmoInReserve += bulletsToAdd;
        }

        UpdateAmmoUI();
    }


    // This function can be called from an AnimationEvent to finish the reloading process
    public void FinishReload()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
        Debug.Log("Reload complete.");
    }
}
