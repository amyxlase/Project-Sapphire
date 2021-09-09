using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;
    public Animator anim;
    public NetworkAnimator networkAnimator;

    [AddComponentMenu("")]
    void FixedUpdate()
    {
        anim.SetFloat("Vertical", Mathf.Abs(Input.GetAxis("Vertical")));
        if (hasAuthority)
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            Debug.LogFormat("Should have played animation");
        }
    }

    public override void OnStartAuthority() {
        Camera camera = transform.Find("FirstPersonCharacter").GetComponent<Camera>();
        camera.enabled = true;
        
    }
}
