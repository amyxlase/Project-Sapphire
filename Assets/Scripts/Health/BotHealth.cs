using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class BotHealth : NetworkBehaviour
{
    // private GameObject manager;
    [SerializeField] private float maxHealth = 100f;

    [SyncVar]
    private float health = 0f;
    
    public bool IsDead => health == 0f;

    public override void OnStartServer()
    {
        health = maxHealth;
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
        //Drop gun
        Debug.Log("killed bot");
        Destroy(gameObject);
        FPSNetworkManager manager = GameObject.Find("Network Manager").GetComponent("FPSNetworkManager") as FPSNetworkManager;
        manager.botCount--;
    }

}
