using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Gun : NetworkBehaviour
{

    private float gunDamage;
    private float shootingSpeed;

    public bool isEquipped = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !isEquipped)
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
