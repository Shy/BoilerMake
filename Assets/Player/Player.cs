﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public enum PlayerType
    {
        Host,
        Client
    }
    public PlayerType Type;
    public float MoveSpeed = 12;
    public float RotationSpeed = Mathf.PI;
    CharacterController Character;


    void EnableOculusCameras()
    {
        Camera[] cameras = GetComponentsInChildren<Camera>();
        foreach (Camera c in cameras)
            c.enabled = true;

        OVRCameraController camcontroller = GetComponentInChildren<OVRCameraController> ();
        camcontroller.enabled = true;

        OVRDevice device = GetComponentInChildren<OVRDevice>();
        device.enabled = true;

        OVRCamera[] ovrcameras = GetComponentsInChildren<OVRCamera>();
        foreach (OVRCamera c in ovrcameras)
            c.enabled = true;

        OVRLensCorrection[] lens = GetComponentsInChildren<OVRLensCorrection>();
        foreach (OVRLensCorrection l in lens)
            l.enabled = true;
    }


    void OnConnectedToServer()
    {
        if (Type == PlayerType.Client)
        {
            EnableOculusCameras();
        }
    }

    void OnServerInitialized()
    {
        if (Type == PlayerType.Host)
        {
            EnableOculusCameras();
        }
    }


    SixenseHands PlayerHand
    {
        get { return (Type == PlayerType.Host ? SixenseHands.LEFT : SixenseHands.RIGHT); }
    }

	// Use this for initialization
	void Start () 
    {
        Character = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Network.isServer)
        {
            SixenseInput.Controller controller = SixenseInput.GetController(PlayerHand);
            if (controller == null || !controller.Enabled)
                return;

            Vector3 move = new Vector3(controller.JoystickX, 0, controller.JoystickY) * MoveSpeed;
            move = transform.rotation * move;
            Character.SimpleMove(move);

            if (controller.GetButton(SixenseButtons.THREE))
            {
                Vector3 angle = transform.rotation.ToEulerAngles();
                angle.y -= RotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.EulerAngles(angle);
            }
            else if (controller.GetButton(SixenseButtons.FOUR))
            {
                Vector3 angle = transform.rotation.ToEulerAngles();
                angle.y += RotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.EulerAngles(angle);
            }
        }
	}
}
