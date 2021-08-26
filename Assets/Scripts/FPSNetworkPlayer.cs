using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;

    [AddComponentMenu("")]
    void FixedUpdate()
    {
        if (hasAuthority)
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime);
    }

    public override void OnStartAuthority() {
        Camera camera = transform.Find("FirstPersonCharacter").GetComponent<Camera>();
        camera.enabled = true;
        
    }
}
