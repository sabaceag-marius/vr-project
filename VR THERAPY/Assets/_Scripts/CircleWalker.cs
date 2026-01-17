using UnityEngine;

public class CircleWalker : MonoBehaviour
{
    [Header("Circle Settings")]
    public float radius = 5.0f;    
    public float speed = 1.0f;      

    void Update()
    {
        float angle = Time.time * speed;


        float x = transform.position.x + Mathf.Cos(angle) * radius;
        float z = transform.position.z + Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, transform.position.y, z);

        // Calculate Rotation (Face Forward)
        // To walk forward we look along the tangent of the circle.
        Vector3 direction = new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle));

        transform.forward = direction;
    }
}