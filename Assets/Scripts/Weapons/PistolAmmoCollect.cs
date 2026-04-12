using UnityEngine;

public class PistolAmmoCollect : MonoBehaviour
{
    [SerializeField] AudioSource ammoCollect;
     void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        ammoCollect.Play();
        GlobalAmmo.handgunAmmoCount += 15;
        Destroy(gameObject);
    }
}
