using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private Slider HP;

    [SyncVar(hook = nameof(UpdateHealth))]
    private float health = 0f;
    
    public bool IsDead => health == 0f;

    public override void OnStartServer()
    {
        health = maxHealth;
    }

    void Update() {

        if (!hasAuthority) { return; }

        GameObject healthUI = transform.GetChild(2).GetChild(1).gameObject;
        HP = healthUI.gameObject.GetComponent<Slider>();
        HP.value = health;
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
        NetworkServer.Destroy(gameObject);
    }

    void UpdateHealth(System.Single oldValue, System.Single newValue) {
        health = newValue;
        //UpdateHealthBar(newValue);
    }

}
