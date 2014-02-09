using UnityEngine;
using System.Collections;

public class Overhead : MonoBehaviour 
{
    void OnConnectedToServer()
    {
        gameObject.SetActive(false);
    }

    void OnServerInitialized()
    {
        gameObject.SetActive(false);
    }

}
