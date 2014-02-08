using UnityEngine;
using System.Collections;

public class AISwordController : SwordController 
{
    public Transform TargetSword;
    Sword PlayerSword;

    ValueSmoother TargetMidPoint;
    ValueSmoother TargetPoint;
    ValueSmoother TargetTip;
    ValueSmoother TargetVelocity;

    float ParryTime;
    Vector3 ParryPosition;
    Vector3 ParryVector;

    float ParrySpeed = 15.0f;


    void Start()
    {
        TargetMidPoint = new ValueSmoother(20);
        TargetVelocity = new ValueSmoother(20);
        TargetTip = new ValueSmoother(20);
        TargetPoint = new ValueSmoother(20);
        ParryTime = 0;

        PlayerSword = TargetSword.GetComponent<Sword>();
    }

    void Update()
    {
        Vector3 a, b;

        if (PhysUtil.SegmentSegment(transform.position, transform.position + transform.rotation * new Vector3(0, 0.3720226f, 0.6694739f),
                                    TargetSword.position, TargetSword.position + TargetSword.rotation * new Vector3(0, 0.3720226f, 0.6694739f), out a, out b) > 0.1)
        {
            
        }

        Debug.DrawLine(a, b);
    }

    public override bool Ready()
    {
        return true;
    }

    private static Vector3 ClosestPoint(Vector3 v, Vector3 w, Vector3 p)
    {
        float l2 = (v - w).sqrMagnitude;
        if (l2 == 0)
            return v;

        float t = Vector3.Dot(p - v, w - v) / l2;
        if (t < 0) return v;
        if (t > 1) return w;
        return v + t * (w - v);
    }

    

    public override Orientation GetOrientation(Transform anchor)
    {
        Debug.DrawLine(PlayerSword.GetProjectedPoint(0.25f), PlayerSword.GetProjectedTip(0.25f));

        Vector3 p = PlayerSword.GetProjectedPoint(0.25f);
        Vector3 t = PlayerSword.GetProjectedTip(0.25f);
        Vector3 m = (p + t) / 2;


        if (!float.IsNaN(m.x))
        {
            TargetMidPoint.Add(m);
            TargetVelocity.Add(PlayerSword.GetSwordVelocity()/Time.deltaTime);
            TargetTip.Add(t);
            TargetPoint.Add(p);
        }

        Vector3 pos;
        Quaternion rot;

        Vector3 chest = anchor.position + new Vector3(0, 1.54f, 0);

        Vector3 TargetPos = chest + new Vector3(0, -0.4f, 0.4f);
        Vector3 TargetParry = new Vector3(0, -1, 1).normalized;


        if (!float.IsNaN(m.x))
        {

            if (ParryTime == 0)
            {
                Vector3 v = TargetVelocity.GetValue();
                //Debug.Log(dist);
                m = TargetMidPoint.GetValue();
                p = TargetPoint.GetValue();
                t = TargetTip.GetValue();
                float dist = Mathf.Min((p - anchor.position).magnitude, (t - anchor.position).magnitude);

                if (dist < 3f && v.magnitude > 1 && Vector3.Dot(v, chest - m) > 0)
                {

                    Vector3 offset = m - chest;
                    if (offset.magnitude > 0.8)
                    {
                        offset = offset.normalized * 0.8f;
                    }
                    m = chest + offset;

                    ParryPosition = m;

                    Debug.DrawLine(chest, m);
                    ParryVector = Vector3.Cross(v, t - p).normalized;

                    if (ParryVector.y < 0)
                        ParryVector *= -1;

                    Debug.DrawLine(m + ParryVector, m - ParryVector);


                    ParryTime = 0.5f;
                }
            }
            else
            {
                ParryTime -= Time.deltaTime;
                if (ParryTime < 0)
                    ParryTime = 0;

                //TargetPos = ParryPosition - ParryVector * 0.5f;

                //TargetParry = ParryVector;
            }
            
        }


        Vector3 diff = (TargetPos - transform.position);
        if (diff.magnitude > ParrySpeed * Time.deltaTime)
        {
            diff = diff.normalized * ParrySpeed * Time.deltaTime;
        }
        pos = (transform.position + diff) - chest;

        Vector3 currentparry = (transform.rotation * new Vector3(0, 0.3720226f, 0.6694739f)).normalized;
        Vector3 tdiff = TargetParry - currentparry;
        if (tdiff.magnitude > ParrySpeed / 2)
        {
            tdiff = tdiff.normalized * ParrySpeed / 2;
        }
        Vector3 finalparry = (currentparry + tdiff).normalized;
        rot = Quaternion.LookRotation(finalparry);
        

        return new Orientation(chest + anchor.rotation * pos, //Magical arm constant
                                anchor.rotation * rot);
    }

}
