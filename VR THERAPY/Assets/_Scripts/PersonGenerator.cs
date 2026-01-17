using System.Linq;
using UnityEngine;

public class PersonGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    private void Start()
    {
        if (prefabs == null)
            return;

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

        Instantiate(prefab, transform);
    }
}