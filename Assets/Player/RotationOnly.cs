using UnityEngine;
using System.Collections;

public class RotationOnly : MonoBehaviour 
{
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Quaternion rot = transform.rotation;
        stream.Serialize(ref rot);
        transform.rotation = rot;
    }

}
