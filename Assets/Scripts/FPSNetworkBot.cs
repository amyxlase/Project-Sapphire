using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkBot : NetworkBehaviour
{
    private CharacterController m_BotController;

    void Start() {
        m_BotController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //AI code should go here, these steps are a rough guideline but totally optional

        //1. Collect information on where other players are, health, etc

        //2. Logic to decide action (walk in direction of players, avoid obstacles, shoot, etc)

        //3. Execute choen action
    }

    private void FixedUpdate() {
        float speed = 55;
        Vector3 vel = transform.up;
        Vector3 gravity = Physics.gravity;
        Vector3 vSpeed = Vector3.zero;
        
        vSpeed += gravity * speed * Time.deltaTime;
        vel = vSpeed;
        m_BotController.Move(vel * Time.deltaTime);
        
    }
}
