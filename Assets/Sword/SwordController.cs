using UnityEngine;

public class SwordController : MonoBehaviour
{
    public virtual bool Ready() { return false; }
    public virtual Orientation GetOrientation(Transform anchor) { return new Orientation(); }
}

