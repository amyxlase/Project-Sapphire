using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [SyncVar]
    private float health = 0f;
    //public GameObject Dead;
    
    public bool IsDead => health == 0f;

    public override void OnStartServer()
    {
        health = maxHealth;

        // Find death screen
        //Dead = transform.GetChild(3).gameObject;
    }

    public float getHealth()
    {
        return health;
    }

    [Server]
    public void Add(float value)
    {
        value = Mathf.Max(value, 0);
        health = Mathf.Max(health + value, 0);
    }

    [Server]
    public void Remove(float value)
    {
        value = Mathf.Max(value, 0);
        health = Mathf.Max(health - value, 0);

        if (health <= 0) 
        {
            HandleDeath();
        }
    }

    [Server]
    public void HandleDeath() {
        //Dead.SetActive(true);
        NetworkServer.Destroy(gameObject);
    }

}
