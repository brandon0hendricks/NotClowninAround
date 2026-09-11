using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuSplit : MonoBehaviour
{
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    [SerializeField] PauseMenu pauseScript;

    void OnEnable()
    {
        StartCoroutine(Screenshot());
    }

    void OnDisable()
    {
        if (leftImage.sprite != null)
        {
            leftImage.sprite = null;
            leftImage.color = new Color(1f, 1f, 1f, 1f);
        }

        if (rightImage.sprite != null)
        {
            rightImage.sprite = null;
            rightImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    private IEnumerator Screenshot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D preScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D screenshot = new Texture2D(preScreenshot.width, preScreenshot.height, TextureFormat.RGBA32, false);
        screenshot.SetPixels(preScreenshot.GetPixels());
        screenshot.Apply();
        Destroy(preScreenshot);

        int half = screenshot.width / 2;
        Texture2D leftHalf = new Texture2D(half, screenshot.height);

        leftHalf.SetPixels(screenshot.GetPixels(0, 0, half, screenshot.height));
        leftHalf.Apply();

        Sprite sprite = Sprite.Create(leftHalf, new Rect(0, 0, leftHalf.width, leftHalf.height), new Vector2(0.5f, 0.5f));
        leftImage.sprite = sprite;

        Texture2D rightHalf = new Texture2D(half, screenshot.height);

        rightHalf.SetPixels(screenshot.GetPixels(half, 0, half, screenshot.height));
        rightHalf.Apply();

        Sprite sprite2 = Sprite.Create(rightHalf, new Rect(0, 0, rightHalf.width, rightHalf.height), new Vector2(0.5f, 0.5f));
        rightImage.sprite = sprite2;
    }

    private void AnimReferenceBegin()
    {
        pauseScript.AnimationBegin();
    }

    private void AnimationReferenceEnd()
    {
        pauseScript.AnimationEnd();
    }    
}
