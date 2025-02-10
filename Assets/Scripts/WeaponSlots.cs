using UnityEngine;
using UnityEngine.UI;

public class WeaponSlots : MonoBehaviour
{
    public int maxSlots = 4; // Change the number of slots to 4
    private GameObject[] weaponSlots;
    private Image[] weaponSlotUI; // UI Image elements for weapon slots
    private int currentSlotIndex = 0;

    private void Start()
    {
        weaponSlots = new GameObject[maxSlots];
        weaponSlotUI = new Image[maxSlots];

        for (int i = 0; i < maxSlots; i++)
        {
            weaponSlots[i] = transform.Find("WeaponSlot" + i).gameObject;
            weaponSlots[i].SetActive(false);

            weaponSlotUI[i] = GameObject.Find("WeaponSlotUI" + i).GetComponent<Image>();
            weaponSlotUI[i].sprite = null;
        }

        // Activate the initial weapon slot and UI element
        weaponSlots[currentSlotIndex].SetActive(true);
        weaponSlotUI[currentSlotIndex].sprite = null; // Set this to an empty image if needed
    }

    private void Update()
    {
        // Switching between weapon slots
        if (Input.GetKeyDown(KeyCode.Alpha1) && maxSlots >= 1)
        {
            ChangeWeaponSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && maxSlots >= 2)
        {
            ChangeWeaponSlot(1);
        }
        // Add more slots as needed...

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Implement the logic for picking up a weapon and adding it to the current slot.
            // You should also update the UI Image element with the picked-up weapon's image.
        }
    }

    void ChangeWeaponSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < maxSlots)
        {
            weaponSlots[currentSlotIndex].SetActive(false);
            weaponSlotUI[currentSlotIndex].sprite = null; // Set this to an empty image if needed
            currentSlotIndex = slotIndex;
            weaponSlots[currentSlotIndex].SetActive(true);
        }
    }
}
