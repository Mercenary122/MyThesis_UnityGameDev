using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Weapon index in WeaponHolder (e.g. MP5 = 1)")]
    public int weaponIndexToUnlock = 1;

    [Tooltip("Bonus ammo given when picked up")]
    public int bonusAmmo = 30;

    [Tooltip("Is this a rifle? (checked = rifle ammo, unchecked = handgun ammo)")]
    public bool isRifle = true;

    public AudioClip pickupSound;

    // Called when the player presses E to pick up
    public void PickUpWeapon(GameObject player)
    {
        // Find the weapon switcher on the player
        // Use GetComponentInChildren in case script is on a child object
        WeaponSwitcher switcher = player.GetComponentInChildren<WeaponSwitcher>();

        if (switcher != null)
        {
            // 1. Unlock the picked gun and switch to it automatically
            switcher.UnlockWeapon(weaponIndexToUnlock);

            // 2. Add the ammo
            if (isRifle) GlobalAmmo.rifleAmmoCount += bonusAmmo;
            else GlobalAmmo.handgunAmmoCount += bonusAmmo;

            // 3. Play pickup sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // 4. The gun is picked up, destroy the object in the scene
            Destroy(gameObject);
        }
    }
}