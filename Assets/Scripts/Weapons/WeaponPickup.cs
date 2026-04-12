using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("拾取设置")]
    [Tooltip("这把枪对应 WeaponHolder 里的第几个子物体？(比如MP5是1)")]
    public int weaponIndexToUnlock = 1;

    [Tooltip("捡起枪附送多少发子弹？")]
    public int bonusAmmo = 30;

    [Tooltip("是步枪吗？(勾选加步枪子弹，不勾加手枪子弹)")]
    public bool isRifle = true;

    public AudioClip pickupSound;

    // 这个方法现在由玩家按下 E 键时触发！
    public void PickUpWeapon(GameObject player)
    {
        // 在传过来的玩家身上找切枪脚本
        // 使用 GetComponentInChildren 是为了防止脚本挂在玩家子物体上找不到
        WeaponSwitcher switcher = player.GetComponentInChildren<WeaponSwitcher>();

        if (switcher != null)
        {
            // 1. 解锁这把枪并自动切过去
            switcher.UnlockWeapon(weaponIndexToUnlock);

            // 2. 加子弹
            if (isRifle) GlobalAmmo.rifleAmmoCount += bonusAmmo;
            else GlobalAmmo.handgunAmmoCount += bonusAmmo;

            // 3. 播放上膛音效
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // 4. 武器被捡走，从场景中销毁
            Destroy(gameObject);
        }
    }
}