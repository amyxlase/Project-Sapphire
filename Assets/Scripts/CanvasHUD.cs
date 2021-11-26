using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHUD : MonoBehaviour
{
	public GameObject PanelStart;
	public GameObject PanelStop;
    public GameObject PanelEnd;
    public GameObject Background;

	public Button buttonHost, buttonServer, buttonClient, buttonStop, buttonHome;

	public InputField inputFieldAddress;

	public Text serverText;
	public Text clientText;

    public string nickname;



    private void Start()
    {

        //Hide UI
        GameObject leaderboard = GameObject.Find("LeaderBoard");
        leaderboard.SetActive(false);

        //Update the canvas text if you have manually changed network managers address from the game object before starting the game scene
        if (NetworkManager.singleton.networkAddress != "localhost") { inputFieldAddress.text = NetworkManager.singleton.networkAddress; }

        //Adds a listener to the main input field and invokes a method when the value changes.
        inputFieldAddress.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        //Make sure to attach these Buttons in the Inspector
        buttonHost.onClick.AddListener(ButtonOnline);
        buttonServer.onClick.AddListener(ButtonHostLocal);
        buttonClient.onClick.AddListener(ButtonClientLocal);
        buttonStop.onClick.AddListener(ButtonStop);
        //buttonStop returns to home screen, buttonHome should too
        buttonHome.onClick.AddListener(ButtonStop);

        //This updates the Unity canvas, we have to manually call it every change, unlike legacy OnGUI.
        SetupCanvas();
    }

    // Invoked when the value of the text field changes.
    public void ValueChangeCheck()
    {
        this.nickname = inputFieldAddress.text;
    }

    public void ButtonOnline()
    {
        NetworkManager.singleton.networkAddress = "empyreangcsserver.com";
        NetworkManager.singleton.StartClient();
        SetupCanvas();
    }

    public void ButtonClientLocal() {

        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartClient();
        SetupCanvas();
        PanelStop.SetActive(false);
        Background.SetActive(false);
    }

    public void ButtonHostLocal()
    {
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartServer();
        SetupCanvas();
    }

    public void ButtonStop()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        SetupCanvas();
    }

    public void SetupCanvas()
    {
        // Here we will dump majority of the canvas UI that may be changed.
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                PanelStart.SetActive(false);
                PanelStop.SetActive(true);
                PanelEnd.SetActive(false);
                clientText.text = "Connecting to " + NetworkManager.singleton.networkAddress + "..";
            }
            else
            {
                PanelStart.SetActive(true);
                PanelStop.SetActive(false);
                PanelEnd.SetActive(false);
            }
        }
        else
        {
            PanelStart.SetActive(false);
            PanelStop.SetActive(false);
            PanelEnd.SetActive(false);
            Background.SetActive(false);

            // server / client status message
            if (NetworkServer.active)
            {
                serverText.text = "Server: active. Transport: " + Transport.activeTransport;
            }
            if (NetworkClient.isConnected)
            {
                clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
            }
        }
    }

    void Update() {

        //Hide in game
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!PanelEnd.gameObject.activeInHierarchy)
            {
                PanelStop.SetActive(!PanelStop.gameObject.activeInHierarchy);
                Background.SetActive(!Background.activeInHierarchy);
            }
        }
    }
}