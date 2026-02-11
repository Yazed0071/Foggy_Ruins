using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;   // assign your panel/background here
    [SerializeField] private TextMeshProUGUI textComponent;

    [Header("Dialogue")]
    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed = 0.05f;

    private int index = 0;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        if (textComponent == null)
        {
            Debug.LogError("DialogueManager: textComponent is not assigned.");
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            EndDialogue();
            return;
        }

        textComponent.text = string.Empty;
        StartDialogue();
    }

    private void Update()
    {
        if (lines == null || lines.Length == 0) return;
    }

    public void StartDialogue()
    {
        index = 0;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;

        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        typingCoroutine = null;
    }

    private void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}