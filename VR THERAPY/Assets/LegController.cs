using UnityEngine;

public class LegController : MonoBehaviour
{
    [Header("--- LEFT LEG ---")]
    public Transform hipL;
    public Transform kneeL;

    [Header("--- RIGHT LEG ---")]
    public Transform hipR;
    public Transform kneeR;

    [Header("Movement Settings")]
    public float speed = 3.0f;
    public float hipAngle = 45.0f;
    public float kneeAngle = 30.0f;

    void Update()
    {
        float swing = Mathf.Sin(Time.time * speed);

        if (hipL)
            hipL.localRotation = Quaternion.Euler(-swing * hipAngle, 0, 0);

        if (kneeL)
        {
            float currentKneeAngle = -Mathf.Abs(swing) * kneeAngle;
            kneeL.localRotation = Quaternion.Euler(currentKneeAngle, 0, 0);
        }

        if (hipR)
            hipR.localRotation = Quaternion.Euler(swing * hipAngle, 0, 0);

        if (kneeR)
        {
            float currentKneeAngle = -Mathf.Abs(swing) * kneeAngle;
            kneeR.localRotation = Quaternion.Euler(currentKneeAngle, 0, 0);
        }
    }
}