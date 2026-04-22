using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0;

    // Array tracking which weapons are unlocked
    public bool[] weaponUnlocked;

    void Start()
    {
        // Auto-generate based on number of child weapons
        weaponUnlocked = new bool[transform.childCount];

        // Only the first weapon (index 0, handgun) is unlocked by default
        if (weaponUnlocked.Length > 0)
        {
            weaponUnlocked[0] = true;
        }

        SelectWeapon();
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        // --- Number key switching (requires weapon to be unlocked£© ---
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponUnlocked.Length > 0 && weaponUnlocked[0])
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponUnlocked.Length > 1 && weaponUnlocked[1])
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponUnlocked.Length > 2 && weaponUnlocked[2])
        {
            selectedWeapon = 2;
        }

        // --- Scroll wheel switching (skips locked weapons) ---
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            int nextWeapon = selectedWeapon;
            do
            {
                nextWeapon++;
                if (nextWeapon >= transform.childCount) nextWeapon = 0;
            } while (!weaponUnlocked[nextWeapon] && nextWeapon != selectedWeapon);
            selectedWeapon = nextWeapon;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            int prevWeapon = selectedWeapon;
            do
            {
                prevWeapon--;
                if (prevWeapon < 0) prevWeapon = transform.childCount - 1;
            } while (!weaponUnlocked[prevWeapon] && prevWeapon != selectedWeapon);
            selectedWeapon = prevWeapon;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    // Public so weapon pickups can call this
    public void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    // Called by weapon pickups to unlock and switch to a weapon
    public void UnlockWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weaponUnlocked.Length)
        {
            weaponUnlocked[weaponIndex] = true; // Unlock weapon
            selectedWeapon = weaponIndex;       // Switch to newly picked up weapon
            SelectWeapon();                     // Refresh weapon display
        }
    }
}