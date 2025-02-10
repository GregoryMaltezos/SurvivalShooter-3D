using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class YourUIScript : MonoBehaviour
{
    public List<Button> weaponButtons; // Reference to your UI buttons for weapon selection

    public void EnableWeaponButton(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weaponButtons.Count)
        {
            weaponButtons[weaponIndex].gameObject.SetActive(true);
        }
    }

    public void DisableWeaponButton(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weaponButtons.Count)
        {
            weaponButtons[weaponIndex].gameObject.SetActive(false);
        }
    }
}
