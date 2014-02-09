using UnityEngine;
using System.Collections;

public class Overhead : MonoBehaviour 
{
    void OnConnectedToServer()
    {
        //gameObject.SetActive(false);
        gameObject.active = false;
    }

    void OnServerInitialized()
    {
        //gameObject.SetActive(false);
        gameObject.active = false;
    }

}
