using System.Collections;
using UnityEngine;

public class PersonInteractor : MonoBehaviour, IInteractable
{
    [SerializeField]
    protected string description;

    [SerializeField]
    protected GameObject player;
    public string GetDescription()
    {
        return description;
    }

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationSpeed = 120f;
    public virtual void Interact()
    {
        StopAllCoroutines();
        StartCoroutine(FacePlayerSmoothly(player.transform));

        Debug.Log("Get interacted idiot");
    }

    IEnumerator FacePlayerSmoothly(Transform playerTransform)
    {
        Vector3 direction = playerTransform.position - transform.position;

        
        direction.y = 0;

        // Create the target rotation (where we want to end up)
       
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Loop until the NPC is facing the player (within 0.1 degrees)
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                // Rotate a small step this frame
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.rotation = targetRotation;
        }
    }
}
