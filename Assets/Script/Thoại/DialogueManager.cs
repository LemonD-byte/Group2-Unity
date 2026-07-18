using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Gắn vào 1 GameObject "DialogueManager" trong Canvas.
/// Gọi từ script khác: DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel1_Scene1(), () => { ... });
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public KeyCode advanceKey = KeyCode.Space;

    private Queue<DialogueLine> sentences = new Queue<DialogueLine>();
    private System.Action onDialogueComplete;

    void Awake()
    {
        Instance = this;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(advanceKey))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(List<DialogueLine> lines, System.Action onComplete = null)
    {
        if (lines == null || lines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        onDialogueComplete = onComplete;
        sentences.Clear();
        foreach (var line in lines) sentences.Enqueue(line);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            onDialogueComplete?.Invoke();
            return;
        }

        DialogueLine line = sentences.Dequeue();
        if (nameText != null) nameText.text = line.speakerName;
        if (dialogueText != null) dialogueText.text = line.sentence;
    }
}
