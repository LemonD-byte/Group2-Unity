using System;
using UnityEngine;

/// <summary>1 dòng thoại: Tên người nói + Nội dung.</summary>
[Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 4)]
    public string sentence;

    public DialogueLine(string speaker, string text)
    {
        speakerName = speaker;
        sentence = text;
    }
}
