using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0;

    // 【新增】记录每把武器是否解锁的数组
    public bool[] weaponUnlocked;

    void Start()
    {
        // 自动根据你的枪械数量生成列表
        weaponUnlocked = new bool[transform.childCount];

        // 默认只解锁第一把枪（序号0，也就是手枪）
        if (weaponUnlocked.Length > 0)
        {
            weaponUnlocked[0] = true;
        }

        SelectWeapon();
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        // --- 按键切换（加入了必须解锁的判定） ---
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

        // --- 滚轮切换（遇到没解锁的枪会自动跳过） ---
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

    // 注意：这里改成了 public，为了让地上的枪能调用它！
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

    // 【新增】给地上的拾取物调用的“解锁并切枪”方法
    public void UnlockWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weaponUnlocked.Length)
        {
            weaponUnlocked[weaponIndex] = true; // 解锁！
            selectedWeapon = weaponIndex;       // 立刻把当前武器设为刚捡到的这把
            SelectWeapon();                     // 刷新显示
        }
    }
}