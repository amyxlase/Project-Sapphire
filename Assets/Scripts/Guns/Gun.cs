using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// how to make the gun work:

// attach this script to a gun (main gun parent class)
// attach another gun script to the gun (like Gun1.cs, can add other functions to make
// this gun more interesting)
// give the gun object a "gun" tag
// attach PickUp.cs to the gun
// give the gun an additional collider component (capsule collider)
// set collider to isTrigger, edit radius & height to something like 3 and 2
// i.e. look at temp_gun1 to see how to configure guns
// & now you can place them in the world for players to pick up

// in the fpsnetwork player script, you can now control the damage 
// according to the gun damage, like with
// float gunDamage = gun.getGunDamage();
// playerDamage.dealDamage(gunDamage);
// in this case, player will only deal damage if equipped w weapon

public class Gun : NetworkBehaviour
{

    [SerializeField] protected float gunDamage;
    [SerializeField] protected float shootingSpeed;
    [SerializeField] protected float ammoPerMagazine;

    [SerializeField] protected float queuedAmmo;
    [SerializeField] protected float totalAmmo;

    public float getGunDamage() {
        return gunDamage;
    }

    public float getShootingSpeed() {
        return shootingSpeed;
    }

    public void setGunDamage(float damage) {
        gunDamage = damage;
    }

    public void setShootingSpeed(float speed) {
        shootingSpeed = speed;
    }

    public float getTotalAmmo() {
        return totalAmmo;
    }

    public float getQueuedAmmo() {
        return queuedAmmo;
    }

    public void DecrementAmmo() {

        if (totalAmmo <= 0 || queuedAmmo <= 0) return;

        totalAmmo--;
        queuedAmmo--;
    }

    public void Reload() {

        if (totalAmmo <= 0) return;

        queuedAmmo = Mathf.Min(totalAmmo, ammoPerMagazine);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameObject player = other.gameObject;
            NetworkIdentity targetIdentity = player.GetComponent<NetworkIdentity>();
            PickUp pickUp = gameObject.GetComponent<PickUp>();
            print(targetIdentity.netId + " can pick up this gun");
            pickUp.enterPickupMode(player);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PickUp pickUp = gameObject.GetComponent<PickUp>();
        pickUp.exitPickupMode();
    }

}
