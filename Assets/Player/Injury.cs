using UnityEngine;
using System.Collections;

public class Injury : MonoBehaviour 
{
    GameObject blood;
	public GameObject fountain;
    public GameObject bloodbase;
	public Transform headPos;

    Transform Head;

    bool Decapitated = false;
	// Use this for initialization
	void Start () 
    {
        blood = (GameObject)Instantiate(bloodbase);
		fountain = (GameObject)Instantiate (bloodbase);
		fountain.transform = headPos; 
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
        Debug.Log("Sword - Body Collision");
        Debug.Log(info.collider.gameObject.name);
        if (info.collider.gameObject.layer == 9)
		{
			audio.Play ();
            Debug.Log("Blooood");
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
                      
                          Decapitate();
                       
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

        Head.gameObject.AddComponent<Rigidbody>();

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
            networkView.RPC("RemoteDecapitate", RPCMode.AllBuffered);
            RemoveHead();
        }
    }
}
