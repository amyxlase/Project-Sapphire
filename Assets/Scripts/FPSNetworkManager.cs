using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class FPSNetworkManager : NetworkManager
{
    public GameObject playerSpawn;
    public GameObject botSpawn;
    public GameObject botPrefab;
    public GameObject pistolPrefab;
    public GameObject riflePrefab;

    public int botCount = 0;
    public int playerCount = 0;

    [AddComponentMenu("")]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        //Spawn gun
        GameObject gun = Instantiate(pistolPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(gun);

        //Spawn player
        Transform start = playerSpawn.transform;
        start.position += Vector3.right * 3 * (numPlayers % 5);
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        playerCount++;

        //Activate various things
        FPSNetworkPlayer playerScript = player.GetComponent<FPSNetworkPlayer>();
        playerScript.enabled = true;

        playerScript.HUD = player.transform.GetChild(2).GetChild(0).gameObject;
        playerScript.HUD.SetActive(true);

        playerScript.gun = gun.GetComponent<Gun>();
        PickUp gunPickup = gun.GetComponent<PickUp>();
        gunPickup.transferParent(playerScript);
        gunPickup.owner = player;

        NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

    [Server]
    public void addBot() {

        //Spawn gun
        GameObject gun = Instantiate(riflePrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(gun);

        //Spawn bot
        Transform botStart = botSpawn.transform;
        Vector3 positionOffset = new Vector3(Random.Range(1, 20), Random.Range(1, 20), Random.Range(1, 20));
        GameObject bot = Instantiate(botPrefab, botStart.position + positionOffset, botStart.rotation);
        NetworkServer.Spawn(bot);

        //Configure bot
        botCount++;
        FPSNetworkBot script = bot.GetComponent<FPSNetworkBot>();
        script.enabled = true;

        //Configure gun
        PickUp gunPickup = gun.GetComponent<PickUp>();
        gunPickup.transferParentBot(script);
        gunPickup.owner = bot;
    }

    private void Update() {
        if(playerCount > 0 && botCount < 5) {
            addBot();
        }
    }

    public override void OnStartServer() {
        for(int i = 0; i < 5; i++) {
           addBot();
        }
    }

}