using UnityEngine;
using System.Collections;

public class Injury : MonoBehaviour 
{
    GameObject blood;
	public GameObject fountain;
    public GameObject bloodbase;
	public Transform headPos;

    Transform Head;



    [HideInInspector]
    public bool Decapitated = false;
	// Use this for initialization
	void Start () 
    {
        blood = (GameObject)Instantiate(bloodbase);
		fountain = (GameObject)Instantiate (bloodbase);
		fountain.transform.parent = headPos;
        fountain.transform.localPosition = new Vector3(0, 0, 0);
        fountain.transform.localRotation = Quaternion.AxisAngle(new Vector3(1, 0, 0), -Mathf.PI / 2);
		blood.particleSystem.Stop ();
		fountain.particleSystem.Stop();

        Head = transform.FindChild("OVRCameraController/CameraLeft/Head");
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision info)
    {
        if (info.collider.gameObject.layer == 9)
		{
			audio.Play ();
            blood.particleSystem.Play();

            //audio.Play ();
        }

        if (Network.isServer)
        {
            foreach (ContactPoint p in info.contacts)
            {
                if (p.otherCollider == Head.collider || p.thisCollider == Head.collider)
                {
                    Collider hitter = (p.otherCollider == Head.collider ? p.thisCollider : p.otherCollider);
                    if (hitter.gameObject.layer == 9)
                    {
                        //Its a sword!
                        //if (hitter.transform.root.rigidbody.velocity.magnitude > 1)
                        //    Decapitate();

                        Vector3 svel = hitter.transform.root.rigidbody.velocity;

                        Vector3 asvel = hitter.transform.root.rigidbody.angularVelocity;

                        Vector3 diff = Head.transform.position - hitter.transform.root.position;

                        if (Mathf.Abs(asvel.y) > 2 * Mathf.PI)
                        {
                            Decapitate();
                            Debug.Log("ANGULAR DECAP " + asvel.y);
                            break;
                        }

                        float vdotd = Vector3.Dot(diff.normalized, svel);

                        if (vdotd > 2)
                        {
                            Debug.Log("LINEAR DECAP");
                            Decapitate();
                            break;
                        }
                    }
                }
            }
        }
    }

    void OnCollisionStay(Collision info)
    {
        if (info.collider.gameObject.layer == 9)
        {
            blood.transform.position = info.contacts[0].point;
            Vector3 n = info.contacts[0].normal;
            if (info.contacts[0].thisCollider == info.collider)
            {
            }
            else
            {
                n = -n;
            }
            blood.transform.LookAt(blood.transform.position + n);
            //Debug.Log("DAFDJADF");
        }
    }
    void OnCollisionExit(Collision info)
    {
        blood.particleSystem.Stop();
    }

    void RemoveHead()
    {
        if (Decapitated)
            return;
		fountain.particleSystem.Play();
        Decapitated = true;
        Vector3 whpos = Head.position;
        Quaternion whrot = Head.rotation;

        Head.parent = null;

        Transform OVRcam = transform.FindChild("OVRCameraController");

        if (OVRcam != null)
            OVRcam.parent = Head;

        OVRcam.GetComponent<OVRCameraController>().FollowOrientation = Head;

        Head.gameObject.AddComponent<Rigidbody>();

        Head.rigidbody.mass = 10;
        Head.rigidbody.AddForce(Head.rotation * new Vector3(0, 1, 1));

        Head.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
        Head.networkView.observed = Head.rigidbody;



    }

    [RPC]
    void RemoteDecapitate()
    {
        RemoveHead();
    }

    void Decapitate()
    {
        if (Network.isServer)
        {
            networkView.RPC("RemoteDecapitate", RPCMode.OthersBuffered);
            RemoveHead();
        }
    }
}
