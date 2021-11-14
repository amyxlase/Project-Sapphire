using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public void Start() {
        setGunDamage(gunDamage);
        setShootingSpeed(shootingSpeed);
    }

}
