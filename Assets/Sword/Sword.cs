using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour 
{
    public Transform Player;

    Vector3 ChestPosition;
    Vector3 ArmLengthPosition;


    Vector3 TargetPosition;
    Quaternion TargetRotation;

    float Scale;

    enum SyncMode
    {
        Starting,
        Chest,
        Waiting,
        ArmLength,
        Done
    }
    SyncMode Mode = SyncMode.Starting;

	// Use this for initialization
	void Start () 
    {
        rigidbody.maxAngularVelocity = 10;
	}
	
	// Update is called once per frame
	void Update () 
    {
        SixenseInput.Controller controller = SixenseInput.GetController(SixenseHands.LEFT);
        if (controller == null || !controller.Enabled)
            return;

        if (Mode == SyncMode.Starting)
        {
            if (controller.Trigger <= 0)
            {
                Mode = SyncMode.Chest;
            }
            return;
        }
        if (Mode == SyncMode.Chest)
        {
            if (controller.Trigger > 0.5)
            {
                ChestPosition = controller.Position;
                Mode = SyncMode.Waiting;
            }
            return;
        }
        else if (Mode == SyncMode.Waiting)
        {
            if (controller.Trigger <= 0)
            {
                Mode = SyncMode.ArmLength;
            }
            return;
        }
        else if (Mode == SyncMode.ArmLength)
        {
            if (controller.Trigger > 0.5)
            {
                ArmLengthPosition = controller.Position;
                Mode = SyncMode.Done;
            }
            return;
        }

        Scale = (ArmLengthPosition - ChestPosition).magnitude;


        TargetPosition = Player.position + new Vector3(0, 1.54f, 0) + Player.rotation * ((controller.Position - ChestPosition) / Scale); //* (0.8382f); //Magical arm constant
        TargetRotation = Player.rotation * controller.Rotation;

       
	}

    void FixedUpdate()
    {
        PhysUtil.ForceTrack(rigidbody, TargetPosition, TargetRotation, 1000, 1000);
    }

    void OnGUI()
    {
        if (Mode == SyncMode.Chest)
        {
            uint boxWidth = 300;
            uint boxHeight = 24;
            string boxText = "Place Controller At Chest and Pull Trigger";
            GUI.Box(new Rect(((Screen.width / 2) - (boxWidth / 2)), ((Screen.height / 2) - (boxHeight / 2)) -48, boxWidth, boxHeight), boxText);
        }
        if (Mode == SyncMode.ArmLength)
        {
            uint boxWidth = 300;
            uint boxHeight = 24;
            string boxText = "Place Controller At Arms Length and Pull Trigger";
            GUI.Box(new Rect(((Screen.width / 2) - (boxWidth / 2)), ((Screen.height / 2) - (boxHeight / 2)) - 48, boxWidth, boxHeight), boxText);
        }
    }
}
