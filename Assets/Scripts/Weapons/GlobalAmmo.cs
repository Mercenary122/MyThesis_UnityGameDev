using UnityEngine;

public class GlobalAmmo : MonoBehaviour
{
    public static int handgunAmmoCount = 15;
    public static int rifleAmmoCount = 30;
    [SerializeField] GameObject ammoDisplay;

    void Start()
    {
        // Reset ammo when scene is reloaded
        handgunAmmoCount = 15;
        rifleAmmoCount = 30;
    }

    void Update()
    {
        ammoDisplay.GetComponent<TMPro.TMP_Text>().text = "" + handgunAmmoCount;
    }
}