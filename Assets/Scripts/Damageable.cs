using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< Updated upstream

=======
using UnityEngine.UI;
>>>>>>> Stashed changes
using Mirror;

public class Damageable : NetworkBehaviour
{
    private Health health;
    private Shield shield;
    public Slider HP;

    public override void OnStartServer()
    {
        health = gameObject.GetComponent<Health>();
        shield = gameObject.GetComponent<Shield>();
        HP = gameObject.GetComponentInChildren<Slider>();
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
        Debug.Log(HP);
        Debug.Log("Current HP is " + HP.value);
    }


}