using UnityEngine;

public class Dialoguetrigger : MonoBehaviour
{

    [SerializeField]DialogueBox dialogueBox; //dialogue box reference
    [SerializeField] DialogueString dialogueString; //dialogue string reference

    [SerializeField] bool oneTimeTrigger; //if the trigger should only be used once
    [HideInInspector] public bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        triggered = true;
        if (other.CompareTag("Player") && DialogueBox.inDialogue == false)
        {
            dialogueBox.activateDialogue(dialogueString.DialogueLines); //send dialogue
            if (oneTimeTrigger == true)
            {
                Destroy(gameObject); //destroy trigger if it is a one time trigger
            }
        }
    }

 
}

