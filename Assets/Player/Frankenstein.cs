using UnityEngine;
using System.Collections;

public class Frankenstein : MonoBehaviour 
{
    Vector3 StartPos;
    Quaternion StartRot;
    public Transform Body;
    Player player;
    Injury injury;
	// Use this for initialization
	void Start () 
    {
        StartPos = transform.localPosition;
        StartRot = transform.localRotation;
        Body = transform.root;

        injury = Body.GetChild(0).GetComponent<Injury>();
        player = Body.GetComponent<Player>();
	}

    float RecapitationTimer = 3.0f;



    void ReAttach()
    {
        if (!injury.Decapitated)
            return;

        RecapitationTimer = 3;

        injury.fountain.particleSystem.Stop();

        Transform Container = Body.GetChild(0);

        Transform OVRcam = transform.FindChild("OVRCameraController");

        OVRcam.parent = Container;

        OVRcam.GetComponent<OVRCameraController>().FollowOrientation = Body;

        OVRcam.localPosition = new Vector3(0,1.74f,0);
        OVRcam.localRotation = new Quaternion();

        transform.parent = OVRcam.GetChild(0);
        transform.localPosition = StartPos;
        transform.localRotation = StartRot;

        networkView.stateSynchronization = NetworkStateSynchronization.Off;
        networkView.observed = transform;

        Destroy(rigidbody);

        injury.Decapitated = false;
        
    }

    [RPC]
    void RemoteRecapitate()
    {
        ReAttach();
    }

    void Recapitate()
    {
        if (Network.isServer)
        {
            networkView.RPC("RemoteRecapitate", RPCMode.OthersBuffered);
            ReAttach();
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if (Network.isServer)
        {
            if (injury.Decapitated)
            {
                RecapitationTimer -= Time.deltaTime;
                if (RecapitationTimer < 0)
                    Recapitate();
            }

            if (player.Type == Player.PlayerType.Host && Input.GetKeyDown(KeyCode.D))
            {
                Recapitate();
            }
            if (player.Type == Player.PlayerType.Client && Input.GetKeyDown(KeyCode.F))
            {
                Recapitate();
            }
        }
	}
}
