using UnityEngine;
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
        {
            if (c != null)
            {
                Debug.Log("camera");
                c.enabled = true;
            }
            else
            {
                Debug.Log("FAILED CAMERA");
            }
        }

        OVRCameraController camcontroller = GetComponentInChildren<OVRCameraController> ();
        if (camcontroller != null)
        {
            Debug.Log("controller");
            camcontroller.enabled = true;
        }
        else
        {
            Debug.Log("FAILED CAMERA CONTROLLER");
        }

        OVRDevice device = GetComponentInChildren<OVRDevice>();
        if (device != null)
        {
            Debug.Log("device");
            device.enabled = true;
        }
        else
        {
            Debug.Log("FAILED DEVICE");
        }

        AudioListener listener = GetComponentInChildren<AudioListener>();
        if (listener != null)
        {
            Debug.Log("listener");
            listener.enabled = true;
        }
        else
        {
            Debug.Log("FAILED LISTENER");
        }

        OVRCamera[] ovrcameras = GetComponentsInChildren<OVRCamera>();
        foreach (OVRCamera c in ovrcameras)
        {
            if (c != null)
            {
                Debug.Log("ovrcameras");
                c.enabled = true;
            }
            else
            {
                Debug.Log("FAILED LISTENER");
            }
        }

        OVRLensCorrection[] lens = GetComponentsInChildren<OVRLensCorrection>();
        foreach (OVRLensCorrection l in lens)
        {
            if (l != null)
            {
                Debug.Log("lens");
                l.enabled = true;
            }
            else
            {
                Debug.Log("FAILED LENS");
            }
        }

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
        HandleOculusResets();


        if (Type == PlayerType.Client && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("REMOVE HEAD");
        }

        if (Network.isServer)
        {
            SixenseInput.Controller controller = SixenseInput.GetController(PlayerHand);
            if (controller == null || !controller.Enabled)
                return;

            Vector3 move = new Vector3(0, 0, controller.JoystickY) * MoveSpeed;

            if(controller.Trigger > 0.2) {
				move.x = controller.JoystickX * MoveSpeed;
			} else {
				Vector3 angle = transform.rotation.ToEulerAngles();
				angle.y += controller.JoystickX * RotationSpeed * Time.deltaTime;
				transform.rotation = Quaternion.EulerAngles(angle);
			}

			move = transform.rotation * move;
            Character.SimpleMove(move);

            
        }


        
	}

	

    void HandleOculusResets()
    {
        if (Input.GetKey(KeyCode.B) && (Network.isClient && Type == PlayerType.Client ||  Network.isServer && Type == PlayerType.Host))
        {
            OVRDevice.ResetOrientation(0);
            Debug.Log("Resetting Oculus");
        }
    }



   
}
