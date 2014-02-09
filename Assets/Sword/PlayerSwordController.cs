using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PlayerSwordController : SwordController
{
    public enum PlayerType
    {
        Host,
        Client
    }
    public PlayerType Type;

    SixenseHands PlayerHand
    {
        get { return (Type == PlayerType.Host ? SixenseHands.LEFT : SixenseHands.RIGHT); }
    }

    enum SyncMode
    {
        Starting,
        Chest,
        Waiting,
        ArmLength,
        Done
    }
    SyncMode Mode = SyncMode.Starting;

    Vector3 ChestPosition;
    Vector3 ArmLengthPosition;
    float Scale;

    public PlayerSwordController()
    {
    }

    public override bool Ready() 
    {
        return Mode == SyncMode.Done;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            Mode = SyncMode.Starting;
        }
    }

    public override Orientation GetOrientation(Transform anchor) 
    {
        if (Network.isServer)
        {
            SixenseInput.Controller controller = SixenseInput.GetController(PlayerHand);
            if (controller == null || !controller.Enabled)
                return new Orientation(new Vector3(), new Quaternion());

            //Debug.Log("Player");

            if (Mode == SyncMode.Starting)
            {
                if (controller.Trigger <= 0)
                {
                    Mode = SyncMode.Chest;
                }
                return new Orientation(new Vector3(), new Quaternion());
            }
            if (Mode == SyncMode.Chest)
            {
                if (controller.Trigger > 0.5)
                {
                    ChestPosition = controller.Position;
                    Mode = SyncMode.Waiting;
                }
                return new Orientation(new Vector3(), new Quaternion());
            }
            else if (Mode == SyncMode.Waiting)
            {
                if (controller.Trigger <= 0)
                {
                    Mode = SyncMode.ArmLength;
                }
                return new Orientation(new Vector3(), new Quaternion());
            }
            else if (Mode == SyncMode.ArmLength)
            {
                if (controller.Trigger > 0.5)
                {
                    ArmLengthPosition = controller.Position;
                    Mode = SyncMode.Done;
                }
                return new Orientation(new Vector3(), new Quaternion());
            }

            Scale = (ArmLengthPosition - ChestPosition).magnitude;

            return new Orientation(anchor.position + new Vector3(0, 1.54f, 0) + anchor.rotation * ((controller.Position - ChestPosition) / Scale), //Magical arm constant
                                    anchor.rotation * controller.Rotation);
        }
        return new Orientation(new Vector3(), new Quaternion());

    }



    public void OnGUI()
    {
        if ((Network.isServer && Type == PlayerType.Host) || (Network.isClient && Type == PlayerType.Client))
        {
            if (Mode == SyncMode.Chest)
            {
                GUIHelper.StereoMessage("Place Controller At Chest and Pull Trigger");
                //uint boxWidth = 300;
                //uint boxHeight = 24;
                //string boxText = "Place Controller At Chest and Pull Trigger";
                //GUI.Box(new Rect(((Screen.width / 2) - (boxWidth / 2)), ((Screen.height / 2) - (boxHeight / 2)) - 48, boxWidth, boxHeight), boxText);
            }
            if (Mode == SyncMode.ArmLength)
            {
                GUIHelper.StereoMessage("Place Controller At Arms Length and Pull Trigger");
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int mode = (int)Mode;
        stream.Serialize(ref mode);
        Mode = (SyncMode)mode;
    }
}

