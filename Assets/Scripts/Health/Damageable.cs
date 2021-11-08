using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Damageable : NetworkBehaviour
{   
    [SyncVar]
    private Health health;
    private Shield shield;

    public override void OnStartServer()
    {
        health = gameObject.GetComponent<Health>();
        shield = gameObject.GetComponent<Shield>();
    }

    [Server]
    public void dealDamage(float damageToDeal)
    {
        //print("Dealing damage");
        float shieldOverflow = 0f;

        if(shield.HasShield)
        {
            shieldOverflow = shield.Overflow(damageToDeal);
            print("shield overflow: " + shieldOverflow);
            shield.Remove(damageToDeal);
        }
        else {
            health.Remove(damageToDeal);
        }

        if(shieldOverflow > 0f)
        {
            health.Remove(shieldOverflow);
        }
    }
}