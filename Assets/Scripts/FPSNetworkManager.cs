﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class FPSNetworkManager : NetworkManager
{
    public GameObject playerSpawn;
    public GameObject botSpawn;
    public GameObject botPrefab;
    public GameObject handGunPrefab;

    public int botCount = 0;
    public int playerCount = 0;

    [AddComponentMenu("")]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        Debug.Log("0: on server add later");

        //Move spawn rightward
        Transform start = playerSpawn.transform;
        start.position += Vector3.right * numPlayers;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        playerCount++;

        //Activate various things
        FPSNetworkPlayer playerScript = player.GetComponent<FPSNetworkPlayer>();
        playerScript.enabled = true;

        Debug.Log("1: Activated fpsnetworkplayerscript");

        playerScript.HUD = GameObject.Find("Canvas").transform.GetChild(6).gameObject;
        playerScript.HUD.SetActive(true);

        Debug.Log("2: Activated HUD");

        Transform gunDestination = playerScript.PistolDestination.transform;
        GameObject gun = Instantiate(handGunPrefab, Vector3.zero, Quaternion.identity);
        playerScript.gun = gun.GetComponent<Gun>();
        NetworkServer.Spawn(gun);

        Debug.Log("3: spawned gun");

        gun.transform.parent = gunDestination;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;

        Debug.Log("4: moved gun");

        NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(conn);

        Debug.Log("5: moved gun");

        //Turn off death screen
        GameObject Dead = GameObject.Find("Death");
        Dead.SetActive(false);

        Debug.Log("6: moved gun");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

    [Server]
    public void addBot() {
        Transform botStart = botSpawn.transform;
        Vector3 positionOffset = new Vector3(Random.Range(1, 20), Random.Range(1, 20), Random.Range(1, 20));
        GameObject bot = Instantiate(botPrefab, botStart.position + positionOffset, botStart.rotation);
        NetworkServer.Spawn(bot);
        botCount++;
    }

    private void Update() {
        if(botCount < playerCount) {
            addBot();
        }
    }

    public override void OnStartServer() {
        for(int i = 0; i < 5; i++) {
           addBot();
        }
    }

}
