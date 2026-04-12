using System.Collections;
using UnityEngine;

public class ModernGun : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 30f;
    public float range = 100f;
    public float timeBetweenShots = 0.1f; // 射速：数值越小射得越快
    public bool isAutomatic = false;      // 勾选这个就是连发步枪，不勾就是手枪

    [Header("Ammo Type")]
    public bool useRifleAmmo = false;     // 勾选这个扣步枪子弹，不勾扣手枪子弹

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject extraCross;
    public Animator gunAnimator;          // 以前是用 GetComponent 获取，现在直接拖拽更稳定

    [Header("Audio")]
    public AudioSource gunFireSound;
    public AudioSource emptyGunSound;

    [Header("Animation")]
    public string fireAnimationName = "HandgunFire";

    [Header("Bullet Hole")]
    public GameObject bulletHolePrefab;    // 弹痕预制体

    // 内部变量
    bool canFire = true;

    // --- 新加的代码，修复切枪卡住的问题 ---
    void OnEnable()
    {
        canFire = true; // 强制重置开火开关
        if (extraCross != null) extraCross.SetActive(false); // 隐藏准星
        // 如果你的枪切出来时动画卡在奇怪的地方，可以加这句强制回正：
        // if (gunAnimator != null) gunAnimator.Play("New State"); 
        Debug.Log("枪支被激活了！重置开火状态！"); // <--- 加这句
        canFire = true;
        if (extraCross != null) extraCross.SetActive(false);
    }

    void Update()
    {
        // 自动武器按住射击 (GetButton)，半自动武器按下射击 (GetButtonDown)
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
        // 检查弹药
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

        // 1. 扣除子弹
        if (useRifleAmmo) GlobalAmmo.rifleAmmoCount--;
        else GlobalAmmo.handgunAmmoCount--;

        // 2. 播放特效和声音
        muzzleFlash.Play();
        gunFireSound.Play();
        extraCross.SetActive(true);
        GunSoundBroadcaster.BroadcastGunshot(transform.position, 50f);

        // 3. 播放动画 (使用 Trigger 往往比 Play("StateName") 更平滑，但为了兼容你的旧动画，这里保留 Play)
        // 注意：如果是步枪，你可能需要去 Animator 里改名字，或者统一都叫 "Fire"
        if (gunAnimator != null && gunAnimator.gameObject.activeInHierarchy)
            {
            gunAnimator.Play(fireAnimationName); // 建议以后步枪动画也叫这个名字，或者叫 "Fire"
        }

        // 4. 发射射线造成伤害 (最关键的一步)
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
                // 在命中点生成弹痕，方向贴合被击中表面的法线
                GameObject hole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
                hole.transform.SetParent(hit.transform); // 跟随被击中的物体（如果物体会动）
                Destroy(hole, 15f); // 15秒后自动消失，防止太多弹痕影响性能
            }
        }

        

        // 5. 等待射速冷却
        yield return new WaitForSeconds(timeBetweenShots);

        // 恢复状态
        if (gunAnimator != null && gunAnimator.gameObject.activeInHierarchy)
        {
            gunAnimator.Play("Idle"); // 你的Idle状态
        }
        extraCross.SetActive(false);
        canFire = true;
    }

    IEnumerator EmptyGunSequence()
    {
        canFire = false;
        emptyGunSound.Play();
        yield return new WaitForSeconds(0.2f); // 空枪冷却可以短一点
        canFire = true;
    }
}