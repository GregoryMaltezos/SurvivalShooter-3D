using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/*
public class WeaponSlotUI : MonoBehaviour
{
    public WeaponSwap weaponSwap; // Reference to the WeaponSwap script
    public List<Button> weaponButtons; // List of weapon selection buttons

    void Start()
    {
        // Attach click listeners to each weapon selection button
        for (int i = 0; i < weaponButtons.Count; i++)
        {
            int weaponSlot = i; // Store the weapon slot for this button
            weaponButtons[i].onClick.AddListener(() => SelectWeapon(weaponSlot));
        }
    }

    void Update()
    {
        /*
        // Update the button text and interactability based on unlocked weapons
        for (int i = 0; i < weaponButtons.Count; i++)
        {
            if (i < weaponSwap.pickedUpWeapons.Count)
            {
                // Enable the button and set its text to the weapon's name or type
                weaponButtons[i].interactable = true;
                weaponButtons[i].GetComponentInChildren<Text>().text = weaponSwap.pickedUpWeapons[i].name;
                weaponButtons[i].gameObject.SetActive(true); // Make the button visible
            }
            else
            {
                // Disable the button if no weapon is unlocked for this slot
                weaponButtons[i].interactable = false;
                weaponButtons[i].GetComponentInChildren<Text>().text = "Locked";
                weaponButtons[i].gameObject.SetActive(false); // Make the button invisible
            }
        }
    }

    void SelectWeapon(int slot)
    {
        // Call the SelectWeapon function in the WeaponSwap script to change the selected weapon
      //  weaponSwap.selectedWeapon = slot;
        //weaponSwap.SelectWeapon();
    }
}
*/