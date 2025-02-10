using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string weaponParentName; // Set the parent object's name in the Inspector
    public int weaponSlotToUnlock = 0; // Set the weapon slot to unlock in the Inspector (0 for the first slot)
    private WeaponSwap weaponSwap;
    private Gun gunScript;

    private void Start()
    {
        // Find the WeaponSwap script in the scene
        weaponSwap = FindObjectOfType<WeaponSwap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Change the condition based on your player's tag
        {
            // Find the parent object by name
            GameObject weaponParent = GameObject.Find(weaponParentName);

            if (weaponParent != null && weaponSlotToUnlock >= 0 && weaponSlotToUnlock < weaponParent.transform.childCount)
            {
                // Get the child object corresponding to the specified weapon slot
                Transform weaponToUnlock = weaponParent.transform.GetChild(weaponSlotToUnlock);

                if (weaponSwap != null)
                {
                    if (!weaponSwap.IsWeaponUnlocked(weaponToUnlock))
                    {
                        // Call a method in the WeaponSwap script to enable the specified weapon object
                        weaponSwap.UnlockWeapon(weaponToUnlock);
                        // Destroy the pickupable object or deactivate it
                        gameObject.SetActive(false); // You can also use Destroy(gameObject) if needed
                    }
                    else
                    {
                        // If the weapon is already unlocked, get the Gun script and add bullets
                        gunScript = weaponToUnlock.GetComponent<Gun>();
                        if (gunScript != null)
                        {
                            gunScript.AddAmmo(); // Create a method in the Gun script to add bullets
                            gameObject.SetActive(false); // Deactivate the pickup object
                        }
                    }
                }
            }
        }
    }
}
