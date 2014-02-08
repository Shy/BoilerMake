using UnityEngine;
using System.Collections;

public class ResetProp : MonoBehaviour {

    Vector3 InitialPosition;
    Quaternion InitialRotation;
	// Use this for initialization
	void Start () 
    {
        InitialPosition = transform.position;
        InitialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = InitialPosition;
            transform.rotation = InitialRotation;
        }
	}
}
