using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;
    public Animator anim;

    [AddComponentMenu("")]
    void FixedUpdate()
    {
        anim.SetFloat("Vertical", Mathf.Abs(Input.GetAxis("Vertical")));
        if (hasAuthority)
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime);
    }

    void Update() {
        Debug.LogFormat("Should have played animation: {0}", Input.GetAxis("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)) {
            anim.Play("Base Layer.Shoot", 0, 0.25f);
        }
    }

    public override void OnStartAuthority() {
        Camera camera = transform.Find("FirstPersonCharacter").GetComponent<Camera>();
        camera.enabled = true;
        
    }
}
