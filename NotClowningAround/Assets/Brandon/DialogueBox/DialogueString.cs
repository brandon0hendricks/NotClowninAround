using UnityEngine;

[CreateAssetMenu(fileName = "DialogueString", menuName = "Scriptable Objects/DialogueString")]
public class DialogueString : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] DialogueLines;

}
