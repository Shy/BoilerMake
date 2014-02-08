using UnityEngine;
using System.Collections;

public class PhysUtil
{
    public static void ForceTrack(Rigidbody body, Vector3 position, Quaternion rotation, float maxaccel, float maxangleaccel, float velocitycancel)
    {
        Vector3 targetvel = (position - body.transform.position) / Time.deltaTime;
        float targetvelmag = targetvel.magnitude;

        float velmag = body.velocity.magnitude;

        Vector3 diffvel = targetvel - (velocitycancel * body.velocity);

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

    public static float SegmentSegment (Vector3 p0, Vector3 p1, Vector3 s0, Vector3 s1, out Vector3 c0, out Vector3 c1)
    {
        Vector3 u = p1 - p0;
        Vector3 v = s1 - s0;
        Vector3 w = p0 - s0;
        float a = Vector3.Dot(u, u);         // always >= 0
        float b = Vector3.Dot(u, v);
        float c = Vector3.Dot(v, v);         // always >= 0
        float d = Vector3.Dot(u, w);
        float e = Vector3.Dot(v, w);
        float D = a * c - b * b;        // always >= 0
        float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
        float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

        // compute the line parameters of the two closest points
        if (D < float.Epsilon)
        { // the lines are almost parallel
            sN = 0.0f;         // force using point P0 on segment S1
            sD = 1.0f;         // to prevent possible division by 0.0 later
            tN = e;
            tD = c;
        }
        else
        {                 // get the closest points on the infinite lines
            sN = (b * e - c * d);
            tN = (a * e - b * d);
            if (sN < 0.0)
            {        // sc < 0 => the s=0 edge is visible
                sN = 0.0f;
                tN = e;
                tD = c;
            }
            else if (sN > sD)
            {  // sc > 1  => the s=1 edge is visible
                sN = sD;
                tN = e + b;
                tD = c;
            }
        }

        if (tN < 0.0)
        {            // tc < 0 => the t=0 edge is visible
            tN = 0.0f;
            // recompute sc for this edge
            if (-d < 0.0)
                sN = 0.0f;
            else if (-d > a)
                sN = sD;
            else
            {
                sN = -d;
                sD = a;
            }
        }
        else if (tN > tD)
        {      // tc > 1  => the t=1 edge is visible
            tN = tD;
            // recompute sc for this edge
            if ((-d + b) < 0.0)
                sN = 0;
            else if ((-d + b) > a)
                sN = sD;
            else
            {
                sN = (-d + b);
                sD = a;
            }
        }
        // finally do the division to get sc and tc
        sc = (Mathf.Abs(sN) < float.Epsilon ? 0.0f : sN / sD);
        tc = (Mathf.Abs(tN) < float.Epsilon ? 0.0f : tN / tD);

        // get the difference of the two closest points
        Vector3 dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

        c0 = p0 + u * sc;
        c1 = s0 + v * tc;
        return dP.magnitude;   // return the closest distance
    }
}
