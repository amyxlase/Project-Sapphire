using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunContainer : MonoBehaviour
{
    [SyncVar(hook = nameof(OnChangeGun))]
    public GunType gunType;
    public Gun gun;
    public GameObject RiflePrefab;
    public GameObject PistolPrefab;

    [AddComponentMenu("")]

    void OnChangeGun(GunType oldGun, GunType newGun) {
        Destroy(gun.gameObject);

        if (newGun == GunType.Rifle) {
            Instantiate(RiflePrefab, this.transform);
        } else {
            Instantiate(PistolPrefab, this.transform);
        }
    }
}
