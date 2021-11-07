using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [SyncVar]
    private float health = 0f;
    private Slider HP; 
    //public GameObject Dead;
    
    public bool IsDead => health == 0f;

    public override void OnStartServer()
    {
        health = maxHealth;

        // Configure health bar
        HP = GameObject.Find("HealthSystem").GetComponent<Slider>();
        HP.maxValue = maxHealth;
        HP.value = health;

        // Find death screen
        //Dead = transform.GetChild(3).gameObject;
    }

    public float getHealth()
    {
        HP.value = health;
        return health;
    }

    [Server]
    public void Add(float value)
    {
        value = Mathf.Max(value, 0);
        health = Mathf.Max(health + value, 0);
        HP.value = health;
    }

    [Server]
    public void Remove(float value)
    {
        value = Mathf.Max(value, 0);
        health = Mathf.Max(health - value, 0);
        HP.value = health;


        if (health <= 0) 
        {
            HandleDeath();
        }
    }

    [Server]
    public void HandleDeath() {
        //Dead.SetActive(true);
        Destroy(gameObject);
    }

}
