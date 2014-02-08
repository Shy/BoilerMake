using UnityEngine;

public struct Orientation
{
    public Vector3 Position;
    public Quaternion Rotation;

    public Orientation(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

