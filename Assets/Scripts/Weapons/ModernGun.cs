using System.Collections;
using UnityEngine;

public class ModernGun : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 30f;
    public float range = 100f;
    public float timeBetweenShots = 0.1f; // Fire rate: lower value = faster firing
    public bool isAutomatic = false;      // Enable for automatic fire, disable for semi-auto

    [Header("Ammo Type")]
    public bool useRifleAmmo = false;     // Enable to use rifle ammo, disable for handgun ammo

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject extraCross;
    public Animator gunAnimator;          // Assigned via Inspector for stability

    [Header("Audio")]
    public AudioSource gunFireSound;
    public AudioSource emptyGunSound;

    [Header("Animation")]
    public string fireAnimationName = "HandgunFire";

    [Header("Bullet Hole")]
    public GameObject bulletHolePrefab;    // Bullet hole decal prefab

    // Internal variables
    bool canFire = true;

    // --- Fix for weapon switch getting stuck ---
    void OnEnable()
    {
        canFire = true; // Force reset fire flag
        if (extraCross != null) extraCross.SetActive(false); // Hide crosshair
        Debug.Log("Weapon activated! Fire state reset."); 
        canFire = true;
        if (extraCross != null) extraCross.SetActive(false);
    }

    void Update()
    {
        // Automatic: hold to fire (GetButton); Semi-auto: press to fire (GetButtonDown)
        if (isAutomatic)
        {
            if (Input.GetButton("Fire1") && canFire)
            {
                CheckAmmoAndFire();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && canFire)
            {
                CheckAmmoAndFire();
            }
        }

        if (PauseManager.isPaused) return;
    }

    void CheckAmmoAndFire()
    {
        // Check ammo
        int currentAmmo = useRifleAmmo ? GlobalAmmo.rifleAmmoCount : GlobalAmmo.handgunAmmoCount;

        if (currentAmmo > 0)
        {
            StartCoroutine(ShootSequence());
        }
        else
        {
            StartCoroutine(EmptyGunSequence());
        }
    }

    IEnumerator ShootSequence()
    {
        canFire = false;

        // 1. Deduct ammo
        if (useRifleAmmo) GlobalAmmo.rifleAmmoCount--;
        else GlobalAmmo.handgunAmmoCount--;

        // 2. Play effects and sound
        muzzleFlash.Play();
        gunFireSound.Play();
        extraCross.SetActive(true);
        GunSoundBroadcaster.BroadcastGunshot(transform.position, 50f);

        // 3. Play fire animation
        if (gunAnimator != null && gunAnimator.gameObject.activeInHierarchy)
            {
            gunAnimator.Play(fireAnimationName);
        }

        // 4. Raycast to deal damage
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            Target target = hit.collider.GetComponent<Target>();
            if (target == null) target = hit.collider.GetComponentInParent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (bulletHolePrefab != null && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Zombie"))
            {
                // Spawn bullet hole decal aligned to hit surface normal
                GameObject hole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
                hole.transform.SetParent(hit.transform); // Parent to hit object (follows moving objects)
                Destroy(hole, 15f); // Auto-destroy after 15s to prevent performance issues
            }
        }



        // 5. Wait for fire rate cooldown
        yield return new WaitForSeconds(timeBetweenShots);

        // Reset state
        if (gunAnimator != null && gunAnimator.gameObject.activeInHierarchy)
        {
            gunAnimator.Play("Idle"); // Return to idle animation
        }
        extraCross.SetActive(false);
        canFire = true;
    }

    IEnumerator EmptyGunSequence()
    {
        canFire = false;
        emptyGunSound.Play();
        yield return new WaitForSeconds(0.2f); // Shorter cooldown for empty gun click
        canFire = true;
    }
}