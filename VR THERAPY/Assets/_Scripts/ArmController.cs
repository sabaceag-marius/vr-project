using UnityEngine;

public class ArmController : MonoBehaviour
{
    [Header("--- LEFT ARM ---")]
    public Transform shoulderL;
    public Transform elbowL;

    [Header("--- RIGHT ARM ---")]
    public Transform shoulderR;  
    public Transform elbowR;     

    [Header("Movement Settings")]
    public float speed = 3.0f;
    public float shoulderMaxAngle = 45.0f;
    public float elbowMaxAngle = 30.0f;

    void Update()
    {
        float swing = Mathf.Sin(Time.time * speed);

        if (shoulderL)
            shoulderL.localRotation = Quaternion.Euler(swing * shoulderMaxAngle, 0, 0);

        if (elbowL)
            elbowL.localRotation = Quaternion.Euler(Mathf.Abs(swing) * elbowMaxAngle, 0, 0);

        if (shoulderR)
            shoulderR.localRotation = Quaternion.Euler(-swing * shoulderMaxAngle, 0, 0);

        if (elbowR)
        {
            elbowR.localRotation = Quaternion.Euler(Mathf.Abs(swing) * elbowMaxAngle, 0, 0);
        }
    }
}