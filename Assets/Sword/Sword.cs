using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour 
{
    public Transform Player;

    SwordController Controller;

    float VelocityCounter = 1.0f;
    float VelocityRegen = 2.0f;

    Vector3 TargetPos;
    Quaternion TargetRot;

    Vector3 CurrentPoint;
    Vector3 LastPoint;

    Vector3 CurrentTip;
    Vector3 LastTip;

    float Lastdt;
	// Use this for initialization
	void Start () 
    {
        TargetPos = transform.position;
        TargetRot = transform.rotation;

        Controller = GetComponent<SwordController>();

        if (Controller == null)
        {
            Debug.LogError("NO SWORD CONTROLLER");
        }

        rigidbody.maxAngularVelocity = 10;
        rigidbody.centerOfMass = new Vector3(0, 0, 0);
        rigidbody.inertiaTensor = new Vector3(1, 1, 1);
	}
	
	// Update is called once per frame
	void Update () 
    {
        LastPoint = CurrentPoint;
        CurrentPoint = transform.position;

        LastTip = CurrentTip;
        CurrentTip = transform.position + transform.rotation * new Vector3(0, 0.3720226f, 0.6694739f);

        Lastdt = Time.deltaTime;
	}

    public Vector3 GetProjectedTip(float dt)
    {
        Vector3 diff = CurrentTip - LastTip;
        return CurrentTip + diff * dt / Lastdt;
    }

    public Vector3 GetProjectedPoint(float dt)
    {
        Vector3 diff = CurrentPoint - LastPoint;
        return CurrentPoint + diff * dt / Lastdt;
    }

    public Vector3 GetSwordVelocity()
    {
        return (CurrentTip - CurrentPoint) / 2 - (LastTip - LastPoint) / 2;
    }


    void FixedUpdate()
    {
        if (Network.isServer)
        {
            Orientation orientation = Controller.GetOrientation(Player);

            if (Controller.Ready())
            {
                TargetPos = orientation.Position;
                TargetRot = orientation.Rotation;
            }
        }
        
        PhysUtil.ForceTrack(rigidbody, TargetPos, TargetRot, 50, 50, VelocityCounter);
        VelocityCounter += Time.deltaTime;
        VelocityCounter = Mathf.Clamp(VelocityCounter, 0, 1);
    }

	void OnCollisionEnter(Collision info)
    {
		if (info.collider.gameObject.layer == 9) 
        {
			//Debug.Log("Collision");
			audio.Play ();	
		}
	}
    void OnCollisionStay(Collision info)
    {
        //Debug.Log("DAFDJADF");
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        stream.Serialize(ref TargetPos);
        stream.Serialize(ref TargetRot);
    }


}
