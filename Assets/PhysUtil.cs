using UnityEngine;
using System.Collections;

public class PhysUtil
{
    public static void ForceTrack(Rigidbody body, Vector3 position, Quaternion rotation, float maxaccel, float maxangleaccel)
    {


        Vector3 targetvel = (position - body.transform.position) / Time.deltaTime;
        float targetvelmag = targetvel.magnitude;

        float velmag = body.velocity.magnitude;

        Vector3 diffvel = targetvel - body.velocity;

        if (diffvel.magnitude > maxaccel)
        {
            diffvel *= maxaccel / diffvel.magnitude;
        }

        Vector3 f = (diffvel) * body.mass;
        body.AddForce(f, ForceMode.Impulse);


        Quaternion targetangluarvel = rotation * (Quaternion.Inverse(body.transform.rotation));
        float targetavelangle; Vector3 targetavelaxis;
        targetangluarvel.ToAxisAngle(out targetavelaxis, out targetavelangle);

        if (targetavelangle > Mathf.PI * 2 - targetavelangle) //Be careful not to take the long way around the circle.
        {
            targetavelaxis *= -1;
            targetavelangle = 2 * Mathf.PI - targetavelangle;
        }

        if (targetavelangle == 0)
            return;

        targetavelangle /= Time.deltaTime;

        Vector3 targetw = targetavelaxis.normalized * targetavelangle;

        Vector3 wf = (targetw - body.angularVelocity);

        if (wf.magnitude > maxangleaccel)
        {
            wf *= maxangleaccel / wf.magnitude;
        }

        Quaternion qi = body.transform.rotation * body.inertiaTensorRotation;
        Vector3 t = qi * Vector3.Scale(body.inertiaTensor, (Quaternion.Inverse(qi) * wf));


        body.AddTorque(t, ForceMode.Impulse);
    }
}
