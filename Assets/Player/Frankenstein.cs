using UnityEngine;
using System.Collections;

public class Frankenstein : MonoBehaviour 
{
    Vector3 RespawnPos;
    Quaternion RespawnRot;

    Vector3 StartPos;
    Quaternion StartRot;
    public Transform Body;
    public Transform OtherBody;

    Frankenstein OtherFrank;

    Player player;
    Injury injury;
	// Use this for initialization
	void Start () 
    {
        OtherFrank = OtherBody.GetComponentInChildren<Frankenstein>();

        StartPos = transform.localPosition;
        StartRot = transform.localRotation;
        Body = transform.root;

        RespawnPos = Body.transform.position;
        RespawnRot = Body.transform.rotation;

        injury = Body.GetChild(0).GetComponent<Injury>();
        player = Body.GetComponent<Player>();
	}

    float RecapitationTimer = 3.0f;

    void Relocate()
    {
        Body.transform.position = RespawnPos;
        Body.transform.rotation = RespawnRot;
    }

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
                {
                    Recapitate();
                    Relocate();
                    OtherFrank.Relocate();
                }
            }

            if (player.Type == Player.PlayerType.Host && Input.GetKeyDown(KeyCode.D))
            {
                Recapitate();
            }
            if (player.Type == Player.PlayerType.Client && Input.GetKeyDown(KeyCode.F))
            {
                Recapitate();
            }
            if (player.Type == Player.PlayerType.Host && Input.GetKeyDown(KeyCode.W))
            {
                Relocate();
            }
            if (player.Type == Player.PlayerType.Client && Input.GetKeyDown(KeyCode.E))
            {
                Relocate();
            }

        }
	}
}
