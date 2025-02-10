using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WeaponSwap : MonoBehaviour
{
    private List<Transform> unlockedWeapons = new List<Transform>();
    private int selectedWeapon = 0;
    public AudioSource weaponSwapSound; 
    public AudioClip weaponSwapClip;
    [Header("Weapon Initialization")]
    public float initialWeaponDelay = 2f; // Delay in seconds before giving the first weapon to the player

    private Gun currentGunScript; // Store a reference to the currently active Gun script

    void Start()
    {
        // Ensure all weapons are initially disabled
        DisableAllWeapons();

        // Start a coroutine to give the first weapon to the player after a delay
        StartCoroutine(GiveFirstWeaponAfterDelay());
    }

    IEnumerator GiveFirstWeaponAfterDelay()
    {
        yield return new WaitForSeconds(initialWeaponDelay);

        // Add the first weapon as unlocked
        if (transform.childCount > 0)
        {
            unlockedWeapons.Add(transform.GetChild(0));
            unlockedWeapons[0].gameObject.SetActive(true);

            // Enable the selected weapon's Gun script
            EnableGunScript(unlockedWeapons[0], true);
        }
    }

    private void EnableGunScript(Transform weapon, bool enable)
    {
        Gun gunScript = weapon.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.enabled = enable;
            gunScript.fpsCam = Camera.main; // Set the appropriate camera here
        }
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
           
            SelectNextWeapon();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            
            SelectPreviousWeapon();
        }
        for (int i = 1; i <= 4; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                SelectWeaponByNumber(i);
            }
        }
        // ... (your existing key-based selection logic)

        EnableSelectedWeapon();
    }
    void SelectWeaponByNumber(int weaponNumber)
    {
        int index = weaponNumber - 1; // Subtract 1 because the list is zero-based

        if (index >= 0 && index < unlockedWeapons.Count)
        {
            if (weaponSwapSound != null && weaponSwapClip != null)
            {
                weaponSwapSound.PlayOneShot(weaponSwapClip);
            }
            // Disable the previously selected weapon
            Transform previousWeapon = unlockedWeapons[selectedWeapon];
            previousWeapon.gameObject.SetActive(false);
            EnableGunScript(previousWeapon, false);

            selectedWeapon = index;
        }
    }
    private void DisableAllWeapons()
    {
        foreach (Transform weapon in unlockedWeapons)
        {
            weapon.gameObject.SetActive(false);
            EnableGunScript(weapon, false);
        }
    }

    private void EnableSelectedWeapon()
    {
        if (unlockedWeapons.Count > 0)
        {
            // Enable the selected weapon at the specified index
            int index = selectedWeapon;
            if (index >= 0 && index < unlockedWeapons.Count)
            {
                Transform selectedWeapon = unlockedWeapons[index];
                selectedWeapon.gameObject.SetActive(true);

                // Enable the selected weapon's Gun script
                EnableGunScript(selectedWeapon, true);
            }
        }
    }

    void SelectNextWeapon()
    {
        if (unlockedWeapons.Count > 1)
        {
            // Disable the previously selected weapon
            Transform previousWeapon = unlockedWeapons[selectedWeapon];
            previousWeapon.gameObject.SetActive(false);
            EnableGunScript(previousWeapon, false);
            weaponSwapSound.PlayOneShot(weaponSwapClip);
            selectedWeapon = (selectedWeapon + 1) % unlockedWeapons.Count;
        }
    }

    void SelectPreviousWeapon()
    {
        if (unlockedWeapons.Count > 1)
        {
            // Disable the previously selected weapon
            Transform previousWeapon = unlockedWeapons[selectedWeapon];
            previousWeapon.gameObject.SetActive(false);
            EnableGunScript(previousWeapon, false);
            weaponSwapSound.PlayOneShot(weaponSwapClip);
            selectedWeapon = (selectedWeapon - 1 + unlockedWeapons.Count) % unlockedWeapons.Count;
        }
    }

    public void UnlockWeapon(Transform weaponTransform)
    {
        if (!unlockedWeapons.Contains(weaponTransform))
        {
            // Player has not picked up this weapon yet, so add it to the unlocked weapons list
            unlockedWeapons.Add(weaponTransform);
        }
    }
    public bool IsWeaponUnlocked(Transform weaponTransform)
    {
        return unlockedWeapons.Contains(weaponTransform);
    }

    public int GetUnlockedWeaponCount()
    {
        return unlockedWeapons.Count;
    }
}
