using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    //Components
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private TextMeshProUGUI continueText;
    private const string continueTextString = "Enter >";
    [HideInInspector] public static bool inDialogue = false;

    private float textSpeed = 0.02f;
    private bool continueDialogue = false;

    public bool ending = false;

    public void activateDialogue(string[] textToDisplay) //Start Dialogue
    {
        StartCoroutine(enableDialogue(textToDisplay));
    }

    private IEnumerator enableDialogue(string[] textToDisplay)
    {
        ending = false;
        inDialogue = true;
        dialogueBox.enabled = true;
        Debug.Log("Dialogue Box Enabled");
        while (dialogueBox.color.a < 1) //turn alpha to 1
        {
            dialogueBox.color = new Color(dialogueBox.color.r, dialogueBox.color.g, dialogueBox.color.b, dialogueBox.color.a + 0.02f);
            dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, dialogueText.color.a + 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        continueDialogue = false; //failsafe to reset continue dialogue
        for (int i = 0; i < textToDisplay.Length; i++) //display Text
        {
            continueText.text = " "; //display continue text
            char[] chars = textToDisplay[i].ToCharArray(); //characters to be displayed;
            for(int b = 0; b < chars.Length; b++)
            {
                dialogueText.text += chars[b];
                if(continueDialogue == true)  //this will end the text display and show the full text if the player clicks before the text is fully displayed
                {
                    dialogueText.text = "";
                    dialogueText.text += textToDisplay[i];
                    break;
                }
                yield return new WaitForSeconds(textSpeed);
            }
            continueDialogue = false; //reset continue dialogue
            continueText.text = continueTextString; //display continue text
            yield return new WaitForSeconds(.25f);
            //wait for input here
            while (continueDialogue == false) //wait for input before continuing to next dialogue
            {
                yield return null;
            }
            continueDialogue = false; //reset continue dialogue
            dialogueText.text = ""; //clear text;
        }
        StartCoroutine(disableDialogue());
    }

    private IEnumerator disableDialogue() //turn off dialogue box
    {
        continueText.text = " "; //display continue text
        while (dialogueBox.color.a > 0) //turn alpha to 0
        {
            dialogueBox.color = new Color(dialogueBox.color.r, dialogueBox.color.g, dialogueBox.color.b, dialogueBox.color.a - 0.02f);
            dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, dialogueText.color.a - 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        dialogueBox.enabled = false;
        inDialogue = false;
        ending = true;
    }

    void OnContinueDialogue() //call this method to continue dialogue
    {
        continueDialogue = true;
    }


}
