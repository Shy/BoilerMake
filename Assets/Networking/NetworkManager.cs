using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
    string GameKey = "BoilerMake_2014_Sword";
    bool Refereshing = false;

    public NetworkView[] DelegatedNetworkViews;

    HostData [] Hosts;
	// Use this for initialization
    void StartServer()
    {
        Network.InitializeServer(4, 25001, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GameKey, "BoilerMake_2014_Sword", "Hackathon Project for BoilerMake");
    }

    void RefreshHostList()
    {
        MasterServer.RequestHostList(GameKey);
        Refereshing = true;
    }

	void Start () 
    {
	    
	}

    void Update()
    {
        if(Refereshing)
        {
            if(MasterServer.PollHostList().Length > 0)
            {
                Refereshing = false;
                Debug.Log(MasterServer.PollHostList().Length);
                Hosts = MasterServer.PollHostList();
            }
        }
    }

    [RPC]
    void TransferDelegated(NetworkViewID id)
    {
        Debug.Log("Transfering to Client");
        foreach (NetworkView v in DelegatedNetworkViews)
        {
            v.viewID = id;
        }
    }

    void CaptureOwnership()
    {
        NetworkViewID newid = Network.AllocateViewID();

        networkView.RPC("TransferDelegated", RPCMode.All, newid);
        
        foreach (NetworkView v in DelegatedNetworkViews)
        {
            v.viewID = newid;
        }
    }

    void  OnMasterServerEvent(MasterServerEvent mse)
    {
        if(mse==MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("Registration Succeeded");
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server On"); 
    }

    void OnConnectedToServer()
    {
        CaptureOwnership();
    }

    void OnGUI()
    {//Change Player Settings to Run in Background
        float btnX = Screen.width * .05f;
        float btnY = Screen.width * .05f;
        float btnW = Screen.width * .1f;
        float btnH = Screen.width * .1f;
        float border = 4;

        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server"))
            {
                Debug.Log("Starting Server");
                StartServer();
            }
            if (GUI.Button(new Rect(btnX, btnY + (border + btnH), btnW, btnH), "Refresh Hosts"))
            {
                Debug.Log("Refreshing");
                RefreshHostList();
            }
            if (GUI.Button(new Rect(btnX, btnY + 2*(border + btnH), btnW, btnH), "ConnectLan"))
            {
                Network.Connect("169.254.144.254", 25001);
            }
            if (Hosts != null)
            {
                for (int i = 0; i < Hosts.Length; i++)
                {
                    if (GUI.Button(new Rect(btnX * 1.5f + btnW, btnY * 1.2f + (btnH * i), btnW * 3, btnH * .5f), Hosts[i].gameName))
                    {
                        Network.Connect(Hosts[i]);
                    }
                }
            }
        }
    }
}
