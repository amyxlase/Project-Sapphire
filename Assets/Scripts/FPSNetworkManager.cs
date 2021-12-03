using System.Collections;
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

        //Spawn gun
        GameObject gun = Instantiate(handGunPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(gun);

        //Spawn player
        Transform start = playerSpawn.transform;
        start.position += Vector3.right * numPlayers;
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

        /*Transform gunDestination = playerScript.PistolDestination.transform;
        GameObject gun = Instantiate(handGunPrefab, Vector3.zero, Quaternion.identity);
        playerScript.gun = gun.GetComponent<Gun>();
        NetworkServer.Spawn(gun);
        gun.transform.parent = gunDestination;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;*/

        NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(conn);

        //Turn off death screen
        //GameObject Dead = GameObject.Find("Death");
        //Dead.SetActive(false);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

    [Server]
    public void addBot() {

        //Spawn gun
        GameObject gun = Instantiate(handGunPrefab, Vector3.zero, Quaternion.identity);
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