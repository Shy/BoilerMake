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
	// Use this for initialization
	void Start () 
    {
        Character = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        SixenseInput.Controller controller = SixenseInput.GetController(SixenseHands.LEFT);
        if (controller == null || !controller.Enabled)
            return;

        Vector3 move = new Vector3(controller.JoystickX, 0, controller.JoystickY) * MoveSpeed;
        move = transform.rotation * move;
        Character.SimpleMove(move);

        if (controller.GetButton(SixenseButtons.THREE))
        {
            Vector3 angle = transform.rotation.ToEulerAngles();
            angle.y -=  RotationSpeed * Time.deltaTime;
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
