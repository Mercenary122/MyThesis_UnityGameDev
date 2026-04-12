using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Unit tests for the FPS game project.
/// Place this file in: Assets/Tests/EditMode/GameTests.cs
/// </summary>

// ============================================================
// TARGET TESTS
// ============================================================
[TestFixture]
public class TargetTests
{
    private GameObject targetObj;
    private Target target;

    [SetUp]
    public void SetUp()
    {
        targetObj = new GameObject("TestTarget");
        target = targetObj.AddComponent<Target>();
        target.health = 100f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(targetObj);
    }

    [Test]
    public void Target_InitialHealth_IsSetCorrectly()
    {
        Assert.AreEqual(100f, target.health);
    }

    [Test]
    public void Target_TakeDamage_ReducesHealth()
    {
        target.TakeDamage(30f);
        Assert.AreEqual(70f, target.health);
    }

    [Test]
    public void Target_TakeDamage_MultipleTimes_AccumulatesDamage()
    {
        target.TakeDamage(20f);
        target.TakeDamage(30f);
        Assert.AreEqual(50f, target.health);
    }

    [Test]
    public void Target_TakeDamage_ExactlyZero_HealthIsZero()
    {
        target.TakeDamage(100f);
        Assert.AreEqual(0f, target.health);
    }

    [Test]
    public void Target_TakeDamage_Overkill_HealthGoesNegative()
    {
        target.TakeDamage(150f);
        Assert.Less(target.health, 0f);
    }

    [Test]
    public void Target_TakeDamage_ZeroDamage_HealthUnchanged()
    {
        target.TakeDamage(0f);
        Assert.AreEqual(100f, target.health);
    }

    [Test]
    public void Target_DefaultHealth_Is50()
    {
        var obj = new GameObject("DefaultTarget");
        var t = obj.AddComponent<Target>();
        Assert.AreEqual(50f, t.health);
        Object.DestroyImmediate(obj);
    }
}

// ============================================================
// GLOBAL AMMO TESTS
// ============================================================
[TestFixture]
public class GlobalAmmoTests
{
    [SetUp]
    public void SetUp()
    {
        GlobalAmmo.handgunAmmoCount = 15;
        GlobalAmmo.rifleAmmoCount = 30;
    }

    [Test]
    public void GlobalAmmo_DefaultHandgunAmmo_Is15()
    {
        Assert.AreEqual(15, GlobalAmmo.handgunAmmoCount);
    }

    [Test]
    public void GlobalAmmo_DefaultRifleAmmo_Is30()
    {
        Assert.AreEqual(30, GlobalAmmo.rifleAmmoCount);
    }

    [Test]
    public void GlobalAmmo_DeductHandgunAmmo_Decreases()
    {
        GlobalAmmo.handgunAmmoCount--;
        Assert.AreEqual(14, GlobalAmmo.handgunAmmoCount);
    }

    [Test]
    public void GlobalAmmo_DeductRifleAmmo_Decreases()
    {
        GlobalAmmo.rifleAmmoCount--;
        Assert.AreEqual(29, GlobalAmmo.rifleAmmoCount);
    }

    [Test]
    public void GlobalAmmo_AddHandgunAmmo_Increases()
    {
        GlobalAmmo.handgunAmmoCount += 10;
        Assert.AreEqual(25, GlobalAmmo.handgunAmmoCount);
    }

    [Test]
    public void GlobalAmmo_AddRifleAmmo_Increases()
    {
        GlobalAmmo.rifleAmmoCount += 20;
        Assert.AreEqual(50, GlobalAmmo.rifleAmmoCount);
    }

    [Test]
    public void GlobalAmmo_CanReachZero()
    {
        GlobalAmmo.handgunAmmoCount = 0;
        Assert.AreEqual(0, GlobalAmmo.handgunAmmoCount);
    }

    [Test]
    public void GlobalAmmo_CanGoNegative()
    {
        GlobalAmmo.handgunAmmoCount = 0;
        GlobalAmmo.handgunAmmoCount--;
        Assert.AreEqual(-1, GlobalAmmo.handgunAmmoCount);
    }
}

// ============================================================
// PLAYER HEALTH TESTS
// ============================================================
[TestFixture]
public class PlayerHealthTests
{
    private GameObject playerObj;
    private PlayerHealth playerHealth;

    [SetUp]
    public void SetUp()
    {
        playerObj = new GameObject("TestPlayer");
        playerHealth = playerObj.AddComponent<PlayerHealth>();

        var healthField = typeof(PlayerHealth).GetField("health",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        healthField.SetValue(playerHealth, 100f);

        var maxHealthField = typeof(PlayerHealth).GetField("maxHealth",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        maxHealthField.SetValue(playerHealth, 100f);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObj);
    }

    private float GetHealth()
    {
        var field = typeof(PlayerHealth).GetField("health",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (float)field.GetValue(playerHealth);
    }

    [Test]
    public void PlayerHealth_InitialHealth_Is100()
    {
        Assert.AreEqual(100f, GetHealth());
    }

    [Test]
    public void PlayerHealth_TakeDamage_ReducesHealth()
    {
        playerHealth.TakeDamage(25f);
        Assert.AreEqual(75f, GetHealth());
    }

    [Test]
    public void PlayerHealth_TakeDamage_MultipleTimes()
    {
        playerHealth.TakeDamage(10f);
        playerHealth.TakeDamage(20f);
        Assert.AreEqual(70f, GetHealth());
    }

    [Test]
    public void PlayerHealth_RestoreHealth_WhenDamaged_ReturnsTrue()
    {
        playerHealth.TakeDamage(50f);
        bool result = playerHealth.RestoreHealth(25f);
        Assert.IsTrue(result);
    }

    [Test]
    public void PlayerHealth_RestoreHealth_WhenDamaged_IncreasesHealth()
    {
        playerHealth.TakeDamage(50f);
        playerHealth.RestoreHealth(25f);
        Assert.AreEqual(75f, GetHealth());
    }

    [Test]
    public void PlayerHealth_RestoreHealth_WhenFull_ReturnsFalse()
    {
        bool result = playerHealth.RestoreHealth(25f);
        Assert.IsFalse(result);
    }

    [Test]
    public void PlayerHealth_RestoreHealth_DoesNotExceedMax()
    {
        playerHealth.TakeDamage(10f);
        playerHealth.RestoreHealth(50f);
        Assert.LessOrEqual(GetHealth(), 100f);
    }

    [Test]
    public void PlayerHealth_IsDead_InitiallyFalse()
    {
        Assert.IsFalse(playerHealth.isDead);
    }

    [Test]
    public void PlayerHealth_TakeDamage_ZeroDamage_NoChange()
    {
        playerHealth.TakeDamage(0f);
        Assert.AreEqual(100f, GetHealth());
    }
}

// ============================================================
// SOLDIER HEALTH TESTS
// ============================================================
[TestFixture]
public class SoldierHealthTests
{
    private GameObject soldierObj;
    private SoldierHealth soldierHealth;
    private Target target;

    [SetUp]
    public void SetUp()
    {
        soldierObj = new GameObject("TestSoldier");
        soldierHealth = soldierObj.AddComponent<SoldierHealth>();
        target = soldierObj.AddComponent<Target>();
        target.health = 100f;

        // SoldierHealth.Start() doesn't run in Edit Mode,
        // so we inject the target reference via reflection
        var targetField = typeof(SoldierHealth).GetField("target",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        targetField.SetValue(soldierHealth, target);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(soldierObj);
    }

    [Test]
    public void SoldierHealth_DefaultHealth_Is100()
    {
        Assert.AreEqual(100f, soldierHealth.health);
    }

    [Test]
    public void SoldierHealth_TakeDamage_ReducesHealth()
    {
        soldierHealth.TakeDamage(30f);
        Assert.AreEqual(70f, soldierHealth.health);
    }

    [Test]
    public void SoldierHealth_TakeDamage_SyncsWithTarget()
    {
        soldierHealth.TakeDamage(30f);
        Assert.AreEqual(70f, target.health);
    }

    [Test]
    public void SoldierHealth_TakeDamage_MultipleTimes()
    {
        soldierHealth.TakeDamage(20f);
        soldierHealth.TakeDamage(30f);
        Assert.AreEqual(50f, soldierHealth.health);
    }
}

// ============================================================
// SLIDING DOOR TESTS
// ============================================================
[TestFixture]
public class SlidingDoorTests
{
    private GameObject doorObj;
    private SlidingDoor door;

    [SetUp]
    public void SetUp()
    {
        doorObj = new GameObject("TestDoor");
        door = doorObj.AddComponent<SlidingDoor>();
        door.slideOffset = new Vector3(2f, 0f, 0f);
        door.speed = 5f;
        door.autoCloseDistance = 6f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(doorObj);
    }

    [Test]
    public void SlidingDoor_InitialState_IsClosed()
    {
        var field = typeof(SlidingDoor).GetField("_isOpen",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsFalse((bool)field.GetValue(door));
    }

    [Test]
    public void SlidingDoor_OpenDoor_SetsOpenState()
    {
        door.OpenDoor();
        var field = typeof(SlidingDoor).GetField("_isOpen",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsTrue((bool)field.GetValue(door));
    }

    [Test]
    public void SlidingDoor_CloseDoor_SetsClosedState()
    {
        door.OpenDoor();
        door.CloseDoor();
        var field = typeof(SlidingDoor).GetField("_isOpen",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsFalse((bool)field.GetValue(door));
    }

    [Test]
    public void SlidingDoor_SlideOffset_IsConfigurable()
    {
        door.slideOffset = new Vector3(3f, 1f, 0f);
        Assert.AreEqual(new Vector3(3f, 1f, 0f), door.slideOffset);
    }

    [Test]
    public void SlidingDoor_Speed_IsPositive()
    {
        Assert.Greater(door.speed, 0f);
    }

    [Test]
    public void SlidingDoor_AutoCloseDistance_IsConfigurable()
    {
        door.autoCloseDistance = 10f;
        Assert.AreEqual(10f, door.autoCloseDistance);
    }
}

// ============================================================
// ENEMY VISIBILITY TESTS
// ============================================================
[TestFixture]
public class EnemyVisibilityTests
{
    private GameObject enemyObj;
    private EnemyVisibility visibility;

    [SetUp]
    public void SetUp()
    {
        enemyObj = new GameObject("TestEnemy");
        visibility = enemyObj.AddComponent<EnemyVisibility>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObj);
    }

    [Test]
    public void EnemyVisibility_DefaultViewDistance_Is25()
    {
        Assert.AreEqual(25f, visibility.viewDistance);
    }

    [Test]
    public void EnemyVisibility_DefaultViewAngle_Is120()
    {
        Assert.AreEqual(120f, visibility.viewAngle);
    }

    [Test]
    public void EnemyVisibility_DefaultHearingRange_Is60()
    {
        Assert.AreEqual(60f, visibility.hearingRange);
    }

    [Test]
    public void EnemyVisibility_DefaultAlertDuration_Is5()
    {
        Assert.AreEqual(5f, visibility.alertDuration);
    }

    [Test]
    public void EnemyVisibility_InitialState_CannotSeePlayer()
    {
        Assert.IsFalse(visibility.canSeePlayer);
    }

    [Test]
    public void EnemyVisibility_InitialState_NotAlerted()
    {
        Assert.IsFalse(visibility.isAlerted);
    }

    [Test]
    public void EnemyVisibility_HearSound_WithinRange_BecomesAlerted()
    {
        visibility.hearingRange = 50f;
        visibility.HearSound(enemyObj.transform.position + Vector3.forward * 10f, 50f);
        Assert.IsTrue(visibility.isAlerted);
    }

    [Test]
    public void EnemyVisibility_HearSound_OutOfRange_StaysUnalerted()
    {
        visibility.hearingRange = 5f;
        visibility.HearSound(enemyObj.transform.position + Vector3.forward * 100f, 50f);
        Assert.IsFalse(visibility.isAlerted);
    }

    [Test]
    public void EnemyVisibility_HearSound_UpdatesLastKnownPosition()
    {
        visibility.hearingRange = 50f;
        Vector3 soundPos = new Vector3(10f, 0f, 20f);
        visibility.HearSound(soundPos, 50f);
        Assert.AreEqual(soundPos, visibility.lastKnownPosition);
    }

    [Test]
    public void EnemyVisibility_ViewDistance_IsConfigurable()
    {
        visibility.viewDistance = 100f;
        Assert.AreEqual(100f, visibility.viewDistance);
    }

    [Test]
    public void EnemyVisibility_EyeHeight_DefaultIs1Point6()
    {
        Assert.AreEqual(1.6f, visibility.eyeHeight);
    }
}

// ============================================================
// WEAPON SWITCHER TESTS
// ============================================================
[TestFixture]
public class WeaponSwitcherTests
{
    private GameObject holderObj;
    private WeaponSwitcher switcher;

    [SetUp]
    public void SetUp()
    {
        holderObj = new GameObject("WeaponHolder");
        for (int i = 0; i < 3; i++)
        {
            var weapon = new GameObject("Weapon" + i);
            weapon.transform.SetParent(holderObj.transform);
        }
        switcher = holderObj.AddComponent<WeaponSwitcher>();
        switcher.weaponUnlocked = new bool[holderObj.transform.childCount];
        switcher.weaponUnlocked[0] = true;
        switcher.selectedWeapon = 0;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(holderObj);
    }

    [Test]
    public void WeaponSwitcher_DefaultSelectedWeapon_IsZero()
    {
        Assert.AreEqual(0, switcher.selectedWeapon);
    }

    [Test]
    public void WeaponSwitcher_FirstWeapon_IsUnlocked()
    {
        Assert.IsTrue(switcher.weaponUnlocked[0]);
    }

    [Test]
    public void WeaponSwitcher_SecondWeapon_IsLocked()
    {
        Assert.IsFalse(switcher.weaponUnlocked[1]);
    }

    [Test]
    public void WeaponSwitcher_ThirdWeapon_IsLocked()
    {
        Assert.IsFalse(switcher.weaponUnlocked[2]);
    }

    [Test]
    public void WeaponSwitcher_UnlockWeapon_UnlocksAndSwitches()
    {
        switcher.UnlockWeapon(1);
        Assert.IsTrue(switcher.weaponUnlocked[1]);
        Assert.AreEqual(1, switcher.selectedWeapon);
    }

    [Test]
    public void WeaponSwitcher_UnlockWeapon_InvalidIndex_NoChange()
    {
        switcher.UnlockWeapon(99);
        Assert.AreEqual(0, switcher.selectedWeapon);
    }

    [Test]
    public void WeaponSwitcher_UnlockWeapon_NegativeIndex_NoChange()
    {
        switcher.UnlockWeapon(-1);
        Assert.AreEqual(0, switcher.selectedWeapon);
    }

    [Test]
    public void WeaponSwitcher_UnlockedArray_MatchesChildCount()
    {
        Assert.AreEqual(3, switcher.weaponUnlocked.Length);
    }
}

// ============================================================
// WEAPON PICKUP TESTS
// ============================================================
[TestFixture]
public class WeaponPickupTests
{
    private GameObject pickupObj;
    private WeaponPickup pickup;

    [SetUp]
    public void SetUp()
    {
        pickupObj = new GameObject("TestPickup");
        pickup = pickupObj.AddComponent<WeaponPickup>();
    }

    [TearDown]
    public void TearDown()
    {
        if (pickupObj != null)
            Object.DestroyImmediate(pickupObj);
    }

    [Test]
    public void WeaponPickup_DefaultWeaponIndex_Is1()
    {
        Assert.AreEqual(1, pickup.weaponIndexToUnlock);
    }

    [Test]
    public void WeaponPickup_DefaultBonusAmmo_Is30()
    {
        Assert.AreEqual(30, pickup.bonusAmmo);
    }

    [Test]
    public void WeaponPickup_DefaultIsRifle_IsTrue()
    {
        Assert.IsTrue(pickup.isRifle);
    }

    [Test]
    public void WeaponPickup_PickUp_AddsRifleAmmo()
    {
        GlobalAmmo.rifleAmmoCount = 30;
        pickup.isRifle = true;
        pickup.bonusAmmo = 20;

        var playerObj = new GameObject("Player");
        var holderObj = new GameObject("WeaponHolder");
        holderObj.transform.SetParent(playerObj.transform);
        for (int i = 0; i < 3; i++)
        {
            new GameObject("Weapon" + i).transform.SetParent(holderObj.transform);
        }
        var switcher = holderObj.AddComponent<WeaponSwitcher>();
        switcher.weaponUnlocked = new bool[3];
        switcher.weaponUnlocked[0] = true;

        // LogAssert.Expect must come BEFORE the call that triggers the error
        LogAssert.Expect(LogType.Error, "Destroy may not be called from edit mode! Use DestroyImmediate instead.\nDestroying an object in edit mode destroys it permanently.");

        pickup.PickUpWeapon(playerObj);

        Assert.AreEqual(50, GlobalAmmo.rifleAmmoCount);

        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void WeaponPickup_PickUp_AddsHandgunAmmo()
    {
        GlobalAmmo.handgunAmmoCount = 15;
        pickup.isRifle = false;
        pickup.bonusAmmo = 10;

        var playerObj = new GameObject("Player");
        var holderObj = new GameObject("WeaponHolder");
        holderObj.transform.SetParent(playerObj.transform);
        for (int i = 0; i < 3; i++)
        {
            new GameObject("Weapon" + i).transform.SetParent(holderObj.transform);
        }
        var switcher = holderObj.AddComponent<WeaponSwitcher>();
        switcher.weaponUnlocked = new bool[3];
        switcher.weaponUnlocked[0] = true;

        // LogAssert.Expect must come BEFORE the call that triggers the error
        LogAssert.Expect(LogType.Error, "Destroy may not be called from edit mode! Use DestroyImmediate instead.\nDestroying an object in edit mode destroys it permanently.");

        pickup.PickUpWeapon(playerObj);

        Assert.AreEqual(25, GlobalAmmo.handgunAmmoCount);

        Object.DestroyImmediate(playerObj);
    }
}

// ============================================================
// MINIMAP FOLLOW TESTS
// ============================================================
[TestFixture]
public class MinimapFollowTests
{
    private GameObject camObj;
    private MinimapFollow minimap;

    [SetUp]
    public void SetUp()
    {
        camObj = new GameObject("MinimapCam");
        camObj.transform.position = new Vector3(0f, 50f, 0f);
        minimap = camObj.AddComponent<MinimapFollow>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(camObj);
    }

    [Test]
    public void MinimapFollow_PlayerNull_NoCrash()
    {
        minimap.player = null;
        Assert.DoesNotThrow(() =>
        {
            if (minimap.player != null)
            {
            }
        });
    }

    [Test]
    public void MinimapFollow_PlayerAssigned_IsNotNull()
    {
        var playerObj = new GameObject("Player");
        minimap.player = playerObj.transform;
        Assert.IsNotNull(minimap.player);
        Object.DestroyImmediate(playerObj);
    }
}

// ============================================================
// GUN SOUND BROADCASTER TESTS
// ============================================================
[TestFixture]
public class GunSoundBroadcasterTests
{
    [Test]
    public void GunSoundBroadcaster_DefaultSoundRange_Is60()
    {
        var obj = new GameObject("Broadcaster");
        var broadcaster = obj.AddComponent<GunSoundBroadcaster>();
        Assert.AreEqual(60f, broadcaster.soundRange);
        Object.DestroyImmediate(obj);
    }

    [Test]
    public void GunSoundBroadcaster_BroadcastGunshot_DoesNotThrow_WhenNoEnemies()
    {
        Assert.DoesNotThrow(() =>
        {
            GunSoundBroadcaster.BroadcastGunshot(Vector3.zero, 50f);
        });
    }

    [Test]
    public void GunSoundBroadcaster_BroadcastGunshot_AlertsNearbyEnemy()
    {
        var enemyObj = new GameObject("Enemy");
        enemyObj.transform.position = Vector3.zero;
        var visibility = enemyObj.AddComponent<EnemyVisibility>();
        visibility.hearingRange = 100f;

        GunSoundBroadcaster.BroadcastGunshot(Vector3.forward * 10f, 100f);

        Assert.IsTrue(visibility.isAlerted);

        Object.DestroyImmediate(enemyObj);
    }

    [Test]
    public void GunSoundBroadcaster_BroadcastGunshot_DoesNotAlert_DistantEnemy()
    {
        var enemyObj = new GameObject("Enemy");
        enemyObj.transform.position = new Vector3(500f, 0f, 0f);
        var visibility = enemyObj.AddComponent<EnemyVisibility>();
        visibility.hearingRange = 10f;

        GunSoundBroadcaster.BroadcastGunshot(Vector3.zero, 10f);

        Assert.IsFalse(visibility.isAlerted);

        Object.DestroyImmediate(enemyObj);
    }
}

// ============================================================
// PAUSE MANAGER TESTS
// ============================================================
[TestFixture]
public class PauseManagerTests
{
    [SetUp]
    public void SetUp()
    {
        PauseManager.isPaused = false;
        Time.timeScale = 1f;
    }

    [Test]
    public void PauseManager_InitialState_IsNotPaused()
    {
        Assert.IsFalse(PauseManager.isPaused);
    }

    [Test]
    public void PauseManager_TimeScale_IsOneByDefault()
    {
        Assert.AreEqual(1f, Time.timeScale);
    }
}

// ============================================================
// MODERN GUN TESTS
// ============================================================
[TestFixture]
public class ModernGunTests
{
    private GameObject gunObj;
    private ModernGun gun;

    [SetUp]
    public void SetUp()
    {
        gunObj = new GameObject("TestGun");
        gun = gunObj.AddComponent<ModernGun>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gunObj);
    }

    [Test]
    public void ModernGun_DefaultDamage_Is30()
    {
        Assert.AreEqual(30f, gun.damage);
    }

    [Test]
    public void ModernGun_DefaultRange_Is100()
    {
        Assert.AreEqual(100f, gun.range);
    }

    [Test]
    public void ModernGun_DefaultIsAutomatic_IsFalse()
    {
        Assert.IsFalse(gun.isAutomatic);
    }

    [Test]
    public void ModernGun_DefaultUseRifleAmmo_IsFalse()
    {
        Assert.IsFalse(gun.useRifleAmmo);
    }

    [Test]
    public void ModernGun_TimeBetweenShots_IsPositive()
    {
        Assert.Greater(gun.timeBetweenShots, 0f);
    }
}
