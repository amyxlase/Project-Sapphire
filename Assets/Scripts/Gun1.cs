using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1 : Gun
{
    [SerializeField]
    private float gunDamage;

    [SerializeField]
    private float shootingSpeed;

    public void Start() {
        setGunDamage(gunDamage);
        setShootingSpeed(shootingSpeed);
    }

}
