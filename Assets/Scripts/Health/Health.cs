using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityStandardAssets.Characters.FirstPerson;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [SyncVar]
    private float health = 0f;
    //private Slider HP; 
    //public GameObject Dead;
    
    public bool IsDead => health == 0f;

    public override void OnStartServer()
    {
        health = maxHealth;
    }

    void Update() {

        if (!isLocalPlayer) { return;}

        Slider HP = this.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Slider>();
        HP.value = health;
    }

    public float getHealth()
    {
        //HP.value = health;
        return health;
    }

    [Server]
    public void Add(float value)
    {
        value = Mathf.Max(value, 0);
        health = Mathf.Max(health + value, 0);
        //HP.value = health;
    }

    [Server]
    public void Remove(float value)
    {
        value = Mathf.Max(value, 0);
        health = Mathf.Max(health - value, 0);
        //HP.value = health;


        if (health <= 0) 
        {
            HandleDeath();
        }
    }

    [Server]
    public void HandleDeath() {
        DeathUI();
        NetworkServer.Destroy(this.gameObject);
    }

    [ClientRpc]
    public void DeathUI() {

        if (!hasAuthority) { return; }
        
        FPSNetworkPlayer player = this.gameObject.GetComponent<FPSNetworkPlayer>();
        player.Dead.SetActive(true);
        FirstPersonController controller = this.gameObject.GetComponent<FirstPersonController>();
        controller.m_MouseLook.SetCursorLock(false);
    }

}