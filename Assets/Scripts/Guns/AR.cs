using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR : Gun
{
    public void Start() {
        setGunDamage(gunDamage);
        setShootingSpeed(shootingSpeed);
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.clip = gunSound;
    }

}
