using UnityEngine;
using System.Collections;

public class Frankenstein : MonoBehaviour 
{
    Vector3 StartPos;
    Quaternion StartRot;
    public Transform Body;
    Player player;
	// Use this for initialization
	void Start () 
    {
        StartPos = transform.localPosition;
        StartRot = transform.localRotation;
        Body = transform.root;
        player = Body.GetComponent<Player>();
	}

    void ReAttach()
    {
        Transform Container = Body.GetChild(0);

        Transform OVRcam = transform.FindChild("OVRCameraController");

        OVRcam.parent = Container;

        OVRcam.GetComponent<OVRCameraController>().FollowOrientation = Body;

        OVRcam.localPosition = new Vector3(0,1.74f,0);
        OVRcam.localRotation = new Quaternion();

        transform.parent = OVRcam.GetChild(0);
        transform.localPosition = StartPos;
        transform.localRotation = StartRot;

        Destroy(rigidbody);
        
    }

	// Update is called once per frame
	void Update () 
    {
        if (Network.isServer)
        {
            if (player.Type == Player.PlayerType.Host && Input.GetKeyDown(KeyCode.D))
            {
            }
        }
	}
}
