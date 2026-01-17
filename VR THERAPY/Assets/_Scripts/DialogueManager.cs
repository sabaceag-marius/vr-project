using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }


    [SerializeField]
    private TextMeshProUGUI simpleResonseText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        simpleResonseText.text = string.Empty;
    }

    public void SetSimpleResponse(string text)
    {
        StartCoroutine(SimpleResponseCoroutine(text));
    }

    private IEnumerator SimpleResponseCoroutine(string text)
    {
        simpleResonseText.text = text;

        yield return new WaitForSeconds(2);

        simpleResonseText.text = string.Empty;
    }
}
