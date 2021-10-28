using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class FPSNetworkManager : NetworkManager
{
    public GameObject playerSpawn;
    public GameObject botSpawn;
    public GameObject botPrefab;

    public int botCount = 0;
    public int playerCount = 0;

    [AddComponentMenu("")]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        //Move spawn rightward
        Transform start = playerSpawn.transform;
        start.position += Vector3.right * numPlayers;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        playerCount++;

        //Activate various things
        FPSNetworkPlayer playerScript = player.GetComponent<FPSNetworkPlayer>();
        playerScript.enabled = true;
        NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(conn);

        //Turn off death screen
        GameObject Dead = GameObject.Find("Death");
        Dead.SetActive(false);
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
        if(botCount < playerCount * 5) {
            addBot();
        }
    }

    public override void OnStartServer() {
        for(int i = 0; i < 10; i++) {
           addBot();
        }
    }

}
