using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shield : NetworkBehaviour
{

    [SerializeField] private float shieldVal = 30f;

    [SyncVar]
    private float shield = 0f;

    public bool HasShield => shield > 0f;

    public override void OnStartServer()
    {
        shield = shieldVal;
    }
    public float getShield()
    {
        return shield;
    }

    [Server]
    public void Add(float value)
    {
        value = Mathf.Max(value, 0);
        shield = Mathf.Max(shield + value, 0);
    }

    [Server]
    public void Remove(float value)
    {
        value = Mathf.Max(value, 0);
        shield = Mathf.Max(shield - value, 0);
    }

    public float Overflow(float value)
    {
        print("shield - value:" + (shield-value));
        return Mathf.Abs(Mathf.Min(shield - value, 0));
    }
}
